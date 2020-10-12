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
    public class RunExeCommand : Command
    {
        public RunExeCommand() : base("exe", "Executes a program")
        {
            Add(new Argument<FileInfo>("path", "Path to the program to execute")
            {

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
                FileName = arguments.PathToProgram.FullName,
                Arguments = arguments.ProgramArguments,
                CreateNoWindow = true
            });
        }
    }
}
