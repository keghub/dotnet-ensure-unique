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

            });

            Handler = CommandHandler.Create<FileInfo, IHost>(ExecuteCommandAsync);
        }

        private async static Task ExecuteCommandAsync(FileInfo path, IHost host)
        {
            var executor = host.Services.GetRequiredService<IProcessExecutor>();

            await executor.ExecuteProcess(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = path.FullName,
                CreateNoWindow = true
            });
        }
    }
}
