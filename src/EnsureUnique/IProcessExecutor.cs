using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EMG.Tools.EnsureUnique.Concurrency;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        private readonly ILogger<DefaultProcessExecutor> _logger;
        private readonly ProcessExecutorOptions _options;

        /// <summary>
        /// Constructs a new instance of <see cref="DefaultProcessExecutor" />.
        /// </summary>
        /// <param name="concurrencyService">An implementation of <see cref="IConcurrencyService" />.</param>
        /// <param name="options">A set of configurable options.</param>
        /// <param name="logger">The logger.</param>
        public DefaultProcessExecutor(IConcurrencyService concurrencyService, IOptions<ProcessExecutorOptions> options, ILogger<DefaultProcessExecutor> logger)
        {
            _concurrencyService = concurrencyService ?? throw new ArgumentNullException(nameof(concurrencyService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Encapsulates the execution of the process described by <paramref name="startInfo"/> with an instance of <see cref="IConcurrencyService" />.
        /// </summary>
        /// <param name="startInfo">Contains the information about the process to start.</param>
        public async Task ExecuteProcess(ProcessStartInfo startInfo)
        {
            _ = startInfo ?? throw new ArgumentNullException(nameof(startInfo));

            var token = _options.Token ?? GetExecutionToken(startInfo);

            if (await _concurrencyService.TryAcquireLockAsync(token).ConfigureAwait(false))
            {
                _logger.LogDebug("Lock on token {TOKEN} acquired", token);

                try
                {
                    var process = new Process
                    {
                        StartInfo = startInfo
                    };

                    _logger.LogDebug("Starting: {FILENAME} {ARGS} with token: {TOKEN}", startInfo.FileName, startInfo.Arguments, token);

                    process.Start();

                    process.WaitForExit();

                    _logger.Log(GetLogLevelForExitCode(process.ExitCode), "Exit code: {EXITCODE}", process.ExitCode);
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

#pragma warning disable CA5350
        private static string GetExecutionToken(ProcessStartInfo startInfo)
        {
            using var md5 = SHA1.Create();

            var bytes = md5.ComputeHash(Encoding.Default.GetBytes($"{startInfo.FileName} {startInfo.Arguments}"));

            var guid = new Guid(bytes);

            return guid.ToString("N");
        }
#pragma warning restore CA5350
    }
}
