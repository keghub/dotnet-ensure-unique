using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
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

            rootCommand.AddGlobalOption(CommonOptions.VerboseOption);

            await new CommandLineBuilder(rootCommand)
                        .UseHost(_ => Host.CreateDefaultBuilder(), ConfigureHost)
                        .UseDefaults()
                        .Build()
                        .InvokeAsync(args);
        }

        private static void ConfigureHost(IHostBuilder host)
        {
            host.ConfigureLogging(ConfigureLogging);

            host.ConfigureServices(ConfigureServices);
        }

        private static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder logging)
        {
            if (context.TryGetOptionValue<LogLevel>(CommonOptions.VerboseOption, out LogLevel logLevel))
            {
                logging.SetMinimumLevel(logLevel);
            }
        }

        private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddOptions();

            services.Configure<ProcessExecutorOptions>(options =>
            {
                if (context.TryGetOptionValue<string>(CommonOptions.TokenOption, out string token))
                {
                    options.Token = token;
                }
            });

            services.Configure<S3ConcurrencyServiceOptions>(options =>
            {
                if (context.TryGetOptionValue<string>(CommonOptions.BucketNameOption, out string bucketName))
                {
                    options.BucketName = bucketName;
                }

                if (context.TryGetOptionValue<string>(CommonOptions.FilePrefixOption, out string filePrefix))
                {
                    options.FilePrefix = filePrefix;
                }
            });

            services.AddSingleton<IProcessExecutor, DefaultProcessExecutor>();

            services.AddSingleton<IConcurrencyService, S3ConcurrencyService>();

            services.AddDefaultAWSOptions(context.Configuration.GetAWSOptions());

            services.AddAWSService<IAmazonS3>();
        }
    }
}
