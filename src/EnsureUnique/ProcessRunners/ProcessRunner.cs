using System.Diagnostics;

namespace EMG.Tools.EnsureUnique.ProcessRunners
{
    /// <summary>
    /// An implementation of <see cref="IProcessRunner" /> that uses <see cref="System.Diagnostics.Process" /> to run the process.
    /// </summary>
    public class ProcessRunner : IProcessRunner
    {
        /// <inheritdoc/>
        public int RunProcess(ProcessStartInfo startInfo)
        {
            var process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();

            process.WaitForExit();

            return process.ExitCode;
        }
    }
}
