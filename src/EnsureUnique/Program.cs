using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EMG.Tools.EnsureUnique
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var rootCommand = new RootCommand("Execute a program and ensure there are no concurrent executions")
            {
                new RunDotNetCommand(),
                new RunExeCommand()
            };

            rootCommand.AddGlobalOption(VerboseOption);

            await new CommandLineBuilder(rootCommand)
                        .UseHost(_ => Host.CreateDefaultBuilder(), ConfigureHost)
                        .UseDefaults()
                        .Build()
                        .InvokeAsync(args);
        }

        private static Option<LogLevel> VerboseOption = new Option<LogLevel>(new[] { "--verbosity", "-v" }, description: "Specify the log verbosity", getDefaultValue: () => LogLevel.Error);

        private static void ConfigureHost(IHostBuilder host)
        {
            host.ConfigureLogging(ConfigureLogging);

            host.ConfigureServices(ConfigureServices);
        }

        private static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder logging)
        {
            if (context.Properties.TryGetValue(typeof(InvocationContext), out var obj) &&
                obj is InvocationContext ctx &&
                ctx.ParseResult.HasOption(VerboseOption) &&
                ctx.ParseResult.ValueForOption(VerboseOption) is LogLevel logLevel)
            {
                logging.SetMinimumLevel(logLevel);
            }
        }

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddSingleton<IProcessExecutor, DefaultProcessExecutor>();
        }
    }
}
