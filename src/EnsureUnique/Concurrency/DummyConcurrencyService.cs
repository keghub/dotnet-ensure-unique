using System.Threading;
using System.Threading.Tasks;

namespace EMG.Tools.EnsureUnique.Concurrency
{
    /// <summary>
    /// A dummy implementation of <see cref="IConcurrencyService"/>.
    /// </summary>
    public class DummyConcurrencyService : IConcurrencyService
    {
        /// <returns>Always <see cref="Task.CompletedTask" />.</returns>
        public Task ReleaseLockAsync(string uniqueToken, CancellationToken cancellationToken = default) => Task.CompletedTask;

        /// <returns>Always <see langword="true"/>.</returns>
        public Task<bool> TryAcquireLockAsync(string uniqueToken, CancellationToken cancellationToken = default) => Task.FromResult(true);
    }
}
