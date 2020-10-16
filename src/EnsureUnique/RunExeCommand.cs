using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EMG.Tools.EnsureUnique
{
    /// <summary>
    /// A <see cref="Command" /> that triggers the execution of an executable file.
    /// </summary>
    public class RunExeCommand : Command
    {
        /// <summary>
        /// Constucts an instance of <see cref="RunExeCommand" />.
        /// </summary>
        public RunExeCommand()
            : base("exe", "Executes a program")
        {
            Add(new Argument<FileInfo>("path", "Path to the program to execute")
            {
                Name = nameof(RunCommandArguments.PathToProgram)
            });

            AddOption(CommonOptions.BucketNameOption);

            AddOption(CommonOptions.FilePrefixOption);

            AddOption(CommonOptions.TokenOption);

            AddOption(CommonOptions.ProgramArguments);

            Handler = CommandHandler.Create<RunCommandArguments, IHost>(ExecuteCommandAsync);
        }

        private static async Task ExecuteCommandAsync(RunCommandArguments arguments, IHost host)
        {
            _ = arguments ?? throw new ArgumentNullException(nameof(arguments));

            _ = arguments.PathToProgram ?? throw new ArgumentException($"{nameof(RunCommandArguments.PathToProgram)} should not be null.", nameof(arguments));

            var executor = host.Services.GetRequiredService<IProcessExecutor>();

            await executor.ExecuteProcess(new ProcessStartInfo
            {
                FileName = arguments.PathToProgram.FullName,
                Arguments = arguments.ProgramArguments,
                CreateNoWindow = true
            }).ConfigureAwait(false);
        }
    }
}
