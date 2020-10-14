using System.IO;

namespace EMG.Tools.EnsureUnique
{
    /// <summary>
    /// A model class used to bind arguments and options for <see cref="RunExeCommand" /> and <see cref="RunDotNetCommand" />.
    /// </summary>
    public class RunCommandArguments
    {
        /// <summary>
        /// The path of the program to execute.
        /// </summary>
        public FileInfo PathToProgram { get; set; }

        /// <summary>
        /// The additional arguments to be forwarded to the program to execute.
        /// </summary>
        public string ProgramArguments { get; set; }
    }
}
