using System.Threading;
using System.Threading.Tasks;

namespace EMG.Tools.EnsureUnique.Concurrency
{
    /// <summary>
    /// Provides locking services.
    /// </summary>
    public interface IConcurrencyService
    {
        /// <summary>
        /// Tries to acquire a lock for the specificied <paramref name="uniqueToken"/>.
        /// </summary>
        /// <param name="uniqueToken">The token to be used as locking object.</param>
        /// <param name="cancellationToken">The cancellation token used to cancel the ongoing operation.</param>
        /// <returns><see langword="true" /> if the lock was acquired. <see langword="false" /> if the lock could not be acquired.</returns>
        Task<bool> TryAcquireLockAsync(string uniqueToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Releases the lock for the specified <paramref name="uniqueToken"/>.
        /// </summary>
        /// <param name="uniqueToken">The token to be used as locking object.</param>
        /// <param name="cancellationToken">The cancellation token used to cancel the ongoing operation.</param>
        Task ReleaseLockAsync(string uniqueToken, CancellationToken cancellationToken = default);
    }
}
