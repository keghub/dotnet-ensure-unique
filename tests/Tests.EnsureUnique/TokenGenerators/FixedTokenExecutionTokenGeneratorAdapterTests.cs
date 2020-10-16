using System.Diagnostics;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Tools.EnsureUnique.TokenGenerators;
using Moq;
using NUnit.Framework;

namespace Tests.TokenGenerators
{
    [TestFixture]
    [TestOf(typeof(FixedTokenExecutionTokenGeneratorAdapter))]
    public class FixedTokenExecutionTokenGeneratorAdapterTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(FixedTokenExecutionTokenGeneratorAdapter).GetConstructors());
        }

        [Test, CustomAutoData]
        public void GenerateToken_forwards_to_inner_generator_if_token_is_null([Frozen] TokenOptions options, [Frozen] IExecutionTokenGenerator inner, FixedTokenExecutionTokenGeneratorAdapter sut, ProcessStartInfo startInfo)
        {
            options.Token = null;

            _ = sut.GenerateToken(startInfo);

            Mock.Get(inner).Verify(p => p.GenerateToken(startInfo));
        }

        [Test, CustomAutoData]
        public void GenerateToken_returns_from_inner_generator_if_token_is_null([Frozen] TokenOptions options, [Frozen] IExecutionTokenGenerator inner, FixedTokenExecutionTokenGeneratorAdapter sut, ProcessStartInfo startInfo, string token)
        {
            Mock.Get(inner).Setup(p => p.GenerateToken(It.IsAny<ProcessStartInfo>())).Returns(token);

            options.Token = null;

            var result = sut.GenerateToken(startInfo);

            Assert.That(result, Is.EqualTo(token));
        }

        [Test, CustomAutoData]
        public void GenerateToken_skips_inner_generator_if_token_is_not_null([Frozen] TokenOptions options, [Frozen] IExecutionTokenGenerator inner, FixedTokenExecutionTokenGeneratorAdapter sut, ProcessStartInfo startInfo, string token)
        {
            options.Token = token;

            _ = sut.GenerateToken(startInfo);

            Mock.Get(inner).Verify(p => p.GenerateToken(It.IsAny<ProcessStartInfo>()), Times.Never);
        }

        [Test, CustomAutoData]
        public void GenerateToken_returns_token_if_token_is_not_null([Frozen] TokenOptions options, FixedTokenExecutionTokenGeneratorAdapter sut, ProcessStartInfo startInfo, string token)
        {
            options.Token = token;

            var result = sut.GenerateToken(startInfo);

            Assert.That(result, Is.EqualTo(token));
        }
    }
}