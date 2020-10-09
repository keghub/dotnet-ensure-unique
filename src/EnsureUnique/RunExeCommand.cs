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

            Handler = CommandHandler.Create<FileInfo, IHost>(ExecuteCommandAsync);
        }

        private async static Task ExecuteCommandAsync(FileInfo path, IHost host)
        {
            var executor = host.Services.GetRequiredService<IProcessExecutor>();

            await executor.ExecuteProcess(new ProcessStartInfo
            {
                FileName = path.FullName,
                CreateNoWindow = true
            });
        }
    }
}
