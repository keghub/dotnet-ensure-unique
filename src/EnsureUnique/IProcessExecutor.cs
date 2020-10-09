using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EMG.Tools.EnsureUnique
{
    public interface IProcessExecutor
    {
        Task ExecuteProcess(ProcessStartInfo startInfo);
    }

    public class DefaultProcessExecutor : IProcessExecutor
    {
        private readonly IConcurrencyService _concurrencyService;
        private readonly ILogger<DefaultProcessExecutor> _logger;
        private readonly ProcessExecutorOptions _options;

        public DefaultProcessExecutor(IConcurrencyService concurrencyService, IOptions<ProcessExecutorOptions> options, ILogger<DefaultProcessExecutor> logger)
        {
            _concurrencyService = concurrencyService ?? throw new ArgumentNullException(nameof(concurrencyService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }
        public async Task ExecuteProcess(ProcessStartInfo startInfo)
        {
            var token = _options.Token ?? GetExecutionToken(startInfo);

            if (await _concurrencyService.TryAcquireLockAsync(token))
            {
                var process = new Process
                {
                    StartInfo = startInfo
                };

                _logger.LogTrace($"Starting: {startInfo.FileName} {startInfo.Arguments} with token: {token}");

                process.Start();

                process.WaitForExit();

                _logger.Log(GetLogLevelForExitCode(process.ExitCode), "Exit code: {EXITCODE}", process.ExitCode);

                await _concurrencyService.ReleaseLockAsync(token);
            }
        }

        private LogLevel GetLogLevelForExitCode(int exitCode)
        {
            return exitCode switch
            {
                0 => LogLevel.Trace,
                _ => LogLevel.Error
            };
        }

        private string GetExecutionToken(ProcessStartInfo startInfo)
        {
            using var md5 = MD5.Create();

            var bytes = md5.ComputeHash(Encoding.Default.GetBytes($"{startInfo.FileName} {startInfo.Arguments}"));

            var guid = new Guid(bytes);

            return guid.ToString("N");
        }
    }

    public class ProcessExecutorOptions
    {
        public string Token { get; set; }
    }
}
