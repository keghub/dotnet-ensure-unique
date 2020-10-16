using System.CommandLine;
using EMG.Tools.EnsureUnique.Concurrency;
using EMG.Tools.EnsureUnique.TokenGenerators;
using Microsoft.Extensions.Logging;

namespace EMG.Tools.EnsureUnique
{
    /// <summary>
    /// A set of <see cref="Option{T}"/> to be shared across different commands.
    /// </summary>
    public static class CommonOptions
    {
        /// <summary>
        /// A shared option that can be used to specify the verbosity of the logging.
        /// It supports the aliases <c>--verbosity</c> and <c>-v</c>.
        /// Valid values are taken from the enumeration <see cref="LogLevel" />.
        /// </summary>
        public static readonly Option<LogLevel> VerboseOption = new Option<LogLevel>(new[] { "--verbosity", "-v" }, description: "Specify the log verbosity", getDefaultValue: () => LogLevel.Error);

        /// <summary>
        /// A shared option that can be used to specify the name of the bucket to be used by <see cref="S3ConcurrencyService" />. It uses the <c>--bucket</c> alias. The option is required.
        /// </summary>
        public static readonly Option<string> BucketNameOption = new Option<string>(new[] { "--bucket" }, description: "Specify the AWS S3 bucket to use to check the lock")
        {
            Name = nameof(S3ConcurrencyServiceOptions.BucketName)
        };

        /// <summary>
        /// A shared option that can be used to specify the file prefix to be used by <see cref="S3ConcurrencyService" />. It uses the <c>--prefix</c> alias.
        /// </summary>
        public static readonly Option<string> FilePrefixOption = new Option<string>(new[] { "--prefix" }, description: "Specify the file prefix to be used when creating a file on AWS S3")
        {
            Name = nameof(S3ConcurrencyServiceOptions.FilePrefix)
        };

        /// <summary>
        /// A shared option that can be used to specify the token to be used to ensure uniqueness. It uses the <c>--token</c> alias.
        /// </summary>
        public static readonly Option<string> TokenOption = new Option<string>("--token", description: "Specify the token to be used to ensure uniqueness")
        {
            Name = nameof(TokenOptions.Token)
        };

        /// <summary>
        /// A shared option that can be used to specify the additional arguments to be passed to the program being executed. It uses the <c>--args</c> alias.
        /// </summary>
        public static readonly Option<string> ProgramArguments = new Option<string>("--args", description: "Specify arguments to be passed to the program")
        {
            Name = nameof(RunCommandArguments.ProgramArguments)
        };
    }
}
