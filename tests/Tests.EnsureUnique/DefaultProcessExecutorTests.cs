using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Tools.EnsureUnique;
using EMG.Tools.EnsureUnique.Concurrency;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    [TestOf(typeof(DefaultProcessExecutor))]
    public class DefaultProcessExecutorTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(DefaultProcessExecutor).GetConstructors());
        }

        [Test, CustomAutoData]
        public async Task ExecuteProcess_tries_to_acquire_lock([Frozen] IConcurrencyService concurrencyService, DefaultProcessExecutor sut, ProcessStartInfo startInfo)
        {
            await sut.ExecuteProcess(startInfo);

            Mock.Get(concurrencyService).Verify(p => p.TryAcquireLockAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()));
        }

        [Test, CustomAutoData]
        public void ExecuteProcess_is_guarded_from_nulls(DefaultProcessExecutor sut)
        {
            Assert.That(() => sut.ExecuteProcess(null), Throws.ArgumentNullException);
        }

        [Test, CustomAutoData]
        public async Task ExecuteProcess_releases_lock([Frozen] IConcurrencyService concurrencyService, DefaultProcessExecutor sut, ProcessStartInfo startInfo)
        {
            await sut.ExecuteProcess(startInfo);

            Mock.Get(concurrencyService).Verify(p => p.ReleaseLockAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()));
        }

        // [Test, CustomAutoData]
        // public async Task Token_is_used_to_acquire_lock_if_assigned([Frozen] IConcurrencyService concurrencyService, DefaultProcessExecutor sut, ProcessStartInfo startInfo, string token)
        // {
        //     options.Token = token;

        //     await sut.ExecuteProcess(startInfo);

        //     Mock.Get(concurrencyService).Verify(p => p.TryAcquireLockAsync(token, It.IsAny<CancellationToken>()));
        // }

        // [Test, CustomAutoData]
        // public async Task Token_is_used_when_releasing_lock_if_assigned([Frozen] IConcurrencyService concurrencyService, DefaultProcessExecutor sut, ProcessStartInfo startInfo, string token)
        // {
        //     options.Token = token;

        //     await sut.ExecuteProcess(startInfo);

        //     Mock.Get(concurrencyService).Verify(p => p.ReleaseLockAsync(token, It.IsAny<CancellationToken>()));
        // }
    }
}