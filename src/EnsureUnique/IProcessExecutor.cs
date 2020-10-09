using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EMG.Tools.EnsureUnique
{
    public interface IProcessExecutor
    {
        Task ExecuteProcess(ProcessStartInfo startInfo);
    }

    public class DefaultProcessExecutor : IProcessExecutor
    {
        private readonly ILogger<DefaultProcessExecutor> _logger;

        public DefaultProcessExecutor(ILogger<DefaultProcessExecutor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public Task ExecuteProcess(ProcessStartInfo startInfo)
        {
            var process = new Process
            {
                StartInfo = startInfo
            };

            _logger.LogTrace($"Starting: {startInfo.FileName} {startInfo.Arguments}");

            process.Start();

            process.WaitForExit();

            _logger.Log(GetLogLevelForExitCode(process.ExitCode), "Exit code: {EXITCODE}", process.ExitCode);

            return Task.CompletedTask;
        }

        private LogLevel GetLogLevelForExitCode(int exitCode)
        {
            return exitCode switch
            {
                0 => LogLevel.Trace,
                _ => LogLevel.Error
            };
        }
    }
}
