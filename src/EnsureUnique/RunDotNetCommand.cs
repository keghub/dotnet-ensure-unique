using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace EMG.Tools.EnsureUnique
{
    public class RunDotNetCommand : Command
    {
        public RunDotNetCommand() : base("dotnet", "Executes a dotnet program")
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

        private async static Task ExecuteCommandAsync(RunCommandArguments arguments, IHost host)
        {
            var executor = host.Services.GetRequiredService<IProcessExecutor>();

            await executor.ExecuteProcess(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = GetArguments(arguments),
                CreateNoWindow = true
            });
        }

        private static string GetArguments(RunCommandArguments arguments)
        {
            return arguments.ProgramArguments switch 
            {
                null => arguments.PathToProgram.FullName,
                _ => $"{arguments.PathToProgram.FullName} {arguments.ProgramArguments}"
            };
        }
    }
}
