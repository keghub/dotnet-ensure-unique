using System.CommandLine;
using Microsoft.Extensions.Logging;

namespace EMG.Tools.EnsureUnique
{
    public static class CommonOptions
    {
        public static Option<LogLevel> VerboseOption = new Option<LogLevel>(new[] { "--verbosity", "-v" }, description: "Specify the log verbosity", getDefaultValue: () => LogLevel.Error);

        public static Option<string> BucketNameOption = new Option<string>(new[] { "--bucket" }, description: "Specify the AWS S3 bucket to use to check the lock")
        {
            Name = "BucketName",
            IsRequired = true
        };

        public static Option<string> FilePrefixOption = new Option<string>(new[] { "--prefix" }, description: "Specify the file prefix to be used when creating a file on AWS S3")
        {
            Name = "FilePrefix"
        };

        public static Option<string> TokenOption = new Option<string>("--token", description: "Specify the token to be used to ensure uniqueness")
        {
            Name = "ExecutionToken"
        };

        public static Option<string> ProgramArguments = new Option<string>("--args", description: "Specify arguments to be passed to the program")
        {
            Name = nameof(RunCommandArguments.ProgramArguments)
        };
    }
}
