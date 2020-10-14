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
    /// A <see cref="Command" /> that triggers the execution of a .NET Core program using the <c>dotnet</c> launcher.
    /// </summary>
    public class RunDotNetCommand : Command
    {
        /// <summary>
        /// Constucts an instance of <see cref="RunDotNetCommand" />.
        /// </summary>
        public RunDotNetCommand()
            : base("dotnet", "Executes a dotnet program")
        {
            Add(new Argument<FileInfo>("path", "Path to the program to execute using 'dotnet'")
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
            var executor = host.Services.GetRequiredService<IProcessExecutor>();

            await executor.ExecuteProcess(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = GetArguments(arguments),
                CreateNoWindow = true
            }).ConfigureAwait(false);
        }

        private static string GetArguments(RunCommandArguments arguments)
        {
            _ = arguments ?? throw new ArgumentNullException(nameof(arguments));

            _ = arguments.PathToProgram ?? throw new ArgumentException($"{nameof(RunCommandArguments.PathToProgram)} should not be null.", nameof(arguments));

            return arguments.ProgramArguments switch
            {
                null => arguments.PathToProgram.FullName,
                _ => $"{arguments.PathToProgram.FullName} {arguments.ProgramArguments}"
            };
        }
    }
}
