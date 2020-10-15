using System.CommandLine;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EMG.Tools.EnsureUnique;
using EMG.Tools.EnsureUnique.Concurrency;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class RunExeCommandTests
    {
        [Test, CustomAutoData]
        public void Constructor_adds_Path_argument (RunExeCommand sut)
        {
            Assert.That(sut.Arguments, Has.Exactly(1).InstanceOf<Argument<FileInfo>>().With.Property(nameof(Argument.Name)).EqualTo(nameof(RunCommandArguments.PathToProgram)));
        }

        [Test]
        [InlineCustomAutoData(nameof(S3ConcurrencyServiceOptions.BucketName))]
        [InlineCustomAutoData(nameof(S3ConcurrencyServiceOptions.FilePrefix))]
        [InlineCustomAutoData(nameof(ProcessExecutorOptions.Token))]
        [InlineCustomAutoData(nameof(RunCommandArguments.ProgramArguments))]
        public void Constructor_adds_options(string optionName, RunExeCommand sut)
        {
            Assert.That(sut.Options, Has.Exactly(1).InstanceOf<Option<string>>().With.Property(nameof(Option.Name)).EqualTo(optionName));
        }

        [Test]
        [CustomAutoData]
        public async Task Bucket_option_is_correctly_parsed(RunExeCommand sut, CommandHandlerSpy commandHandler, string program, string bucket)
        {
            sut.Handler = commandHandler;

            await sut.InvokeAsync($"exe {program} --bucket {bucket}");

            Assert.That(commandHandler.InvocationContext.ParseResult.ValueForOption(CommonOptions.BucketNameOption), Is.EqualTo(bucket));
        }

        [Test]
        [CustomAutoData]
        public async Task FilePrefix_option_is_correctly_parsed(RunExeCommand sut, CommandHandlerSpy commandHandler, string program, string filePrefix)
        {
            sut.Handler = commandHandler;

            await sut.InvokeAsync($"exe {program} --prefix {filePrefix}");

            Assert.That(commandHandler.InvocationContext.ParseResult.ValueForOption(CommonOptions.FilePrefixOption), Is.EqualTo(filePrefix));
        }

        [Test]
        [CustomAutoData]
        public async Task Token_option_is_correctly_parsed_when_added(RunExeCommand sut, CommandHandlerSpy commandHandler, string program, string token)
        {
            sut.Handler = commandHandler;

            await sut.InvokeAsync($"exe {program} --token {token}");

            Assert.That(commandHandler.InvocationContext.ParseResult.ValueForOption(CommonOptions.TokenOption), Is.EqualTo(token));
        }

        [Test]
        [CustomAutoData]
        public async Task Token_option_is_null_when_not_added(RunExeCommand sut, CommandHandlerSpy commandHandler, string program)
        {
            sut.Handler = commandHandler;

            await sut.InvokeAsync($"exe {program}");

            Assert.That(commandHandler.InvocationContext.ParseResult.ValueForOption(CommonOptions.TokenOption), Is.Null);
        }

        [Test]
        [CustomAutoData]
        public async Task PathToProgram_is_correctly_parsed(RunExeCommand sut, CommandHandlerSpy commandHandler, string program)
        {
            sut.Handler = commandHandler;

            await sut.InvokeAsync($"exe {program}");

            Assert.That(commandHandler.InvocationContext.ParseResult.ValueForArgument(sut.Arguments.First() as Argument<FileInfo>).ToString(), Is.EqualTo(program));
        }
    }
}