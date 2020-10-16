using System;
using System.Diagnostics;
using System.Threading.Tasks;
using EMG.Tools.EnsureUnique.Concurrency;
using EMG.Tools.EnsureUnique.ProcessRunners;
using EMG.Tools.EnsureUnique.TokenGenerators;
using Microsoft.Extensions.Logging;

namespace EMG.Tools.EnsureUnique
{
    /// <summary>
    /// Implementers of this interface execute and track a process described by <see cref="ProcessStartInfo" />.
    /// </summary>
    public interface IProcessExecutor
    {
        /// <summary>
        /// Executes and tracks a process described by <paramref name="startInfo"/>.
        /// </summary>
        Task ExecuteProcess(ProcessStartInfo startInfo);
    }

    /// <summary>
    /// The default implementation of the <see cref="IProcessExecutor"/> interface.
    /// </summary>
    public class DefaultProcessExecutor : IProcessExecutor
    {
        private readonly IConcurrencyService _concurrencyService;
        private readonly IProcessRunner _processRunner;
        private readonly IExecutionTokenGenerator _executionTokenGenerator;
        private readonly ILogger<DefaultProcessExecutor> _logger;

        /// <summary>
        /// Constructs a new instance of <see cref="DefaultProcessExecutor" />.
        /// </summary>
        /// <param name="concurrencyService">An implementation of <see cref="IConcurrencyService" />.</param>
        /// <param name="processRunner">An implementation of <see cref="IProcessRunner" />.</param>
        /// <param name="executionTokenGenerator">An implementation of <see cref="IExecutionTokenGenerator" /> used to generate execution tokens.</param>
        /// <param name="logger">The logger.</param>
        public DefaultProcessExecutor(
            IConcurrencyService concurrencyService,
            IProcessRunner processRunner,
            IExecutionTokenGenerator executionTokenGenerator,
            ILogger<DefaultProcessExecutor> logger)
        {
            _concurrencyService = concurrencyService ?? throw new ArgumentNullException(nameof(concurrencyService));
            _processRunner = processRunner ?? throw new ArgumentNullException(nameof(processRunner));
            _executionTokenGenerator = executionTokenGenerator ?? throw new ArgumentNullException(nameof(executionTokenGenerator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Wraps the execution of the process described by <paramref name="startInfo"/> with an instance of <see cref="IConcurrencyService" />.
        /// </summary>
        /// <param name="startInfo">Contains the information about the process to start.</param>
        public async Task ExecuteProcess(ProcessStartInfo startInfo)
        {
            _ = startInfo ?? throw new ArgumentNullException(nameof(startInfo));

            var token = _executionTokenGenerator.GenerateToken(startInfo);

            if (await _concurrencyService.TryAcquireLockAsync(token).ConfigureAwait(false))
            {
                _logger.LogDebug("Lock on token {TOKEN} acquired", token);

                try
                {
                    _logger.LogDebug("Starting: {FILENAME} {ARGS} with token: {TOKEN}", startInfo.FileName, startInfo.Arguments, token);

                    var exitCode = _processRunner.RunProcess(startInfo);

                    _logger.Log(GetLogLevelForExitCode(exitCode), "Exit code: {EXITCODE}", exitCode);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while executing '{FILENAME} {ARGS}'", startInfo.FileName, startInfo.Arguments);
                    throw;
                }
                finally
                {
                    _logger.LogDebug("Releasing lock on token {TOKEN}", token);

                    await _concurrencyService.ReleaseLockAsync(token).ConfigureAwait(false);

                    _logger.LogDebug("Lock on token {TOKEN} released", token);
                }
            }
        }

        private static LogLevel GetLogLevelForExitCode(int exitCode)
        {
            return exitCode switch
            {
                0 => LogLevel.Debug,
                _ => LogLevel.Error
            };
        }
    }
}
