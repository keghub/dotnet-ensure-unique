using System.Diagnostics;
using System.Security.Cryptography;
using AutoFixture.Idioms;
using EMG.Tools.EnsureUnique.TokenGenerators;
using NUnit.Framework;

namespace Tests.TokenGenerators
{
    [TestFixture]
    [TestOf(typeof(MD5ExecutionTokenGenerator))]
    public class MD5ExecutionTokenGeneratorTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(MD5ExecutionTokenGenerator).GetConstructors());
        }

        [Test, CustomAutoData]
        public void GenerateToken_returns_MD5_hash_of_startInfo(MD5ExecutionTokenGenerator sut, ProcessStartInfo startInfo)
        {
            var result = sut.GenerateToken(startInfo);

            Assert.That(result, Is.EqualTo($"{startInfo.FileName} {startInfo.Arguments}".MD5()));
        }

        [Test, CustomAutoData]
        public void GenerateToken_does_not_accept_nulls(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(MD5ExecutionTokenGenerator).GetMethod(nameof(MD5ExecutionTokenGenerator.GenerateToken)));
        }
    }
}