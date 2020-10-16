using System.Diagnostics;

namespace EMG.Tools.EnsureUnique.ProcessRunners
{
    /// <summary>
    /// Abstraction over process execution.
    /// </summary>
    public interface IProcessRunner
    {
        /// <summary>
        /// Run the process specified in the <paramref name="startInfo"/>.
        /// </summary>
        /// <param name="startInfo">The description of the process to run.</param>
        /// <returns>The exit code of the process.</returns>
        int RunProcess(ProcessStartInfo startInfo);
    }
}
