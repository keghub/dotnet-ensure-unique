using System.Threading.Tasks;
using AutoFixture.Idioms;
using EMG.Tools.EnsureUnique.Concurrency;
using NUnit.Framework;

namespace Tests.Concurrency
{
    [TestFixture]
    public class DummyConcurrencyServiceTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(DummyConcurrencyService).GetConstructors());
        }

        [Test, CustomAutoData]
        public async Task TryAcquireLockAsync_returns_true(DummyConcurrencyService sut, string token)
        {
            var result = await sut.TryAcquireLockAsync(token);

            Assert.That(result, Is.True);
        }

        [Test, CustomAutoData]
        public async Task ReleaseLockAsync_always_returns(DummyConcurrencyService sut, string token)
        {
            await sut.ReleaseLockAsync(token);
        }
    }
}