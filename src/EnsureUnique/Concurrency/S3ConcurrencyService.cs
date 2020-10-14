using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EMG.Tools.EnsureUnique.Concurrency
{
    /// <summary>
    /// An implementation of <see cref="IConcurrencyService"/> that uses objects in Amazon S3 as locking mechanism.
    /// </summary>
    public class S3ConcurrencyService : IConcurrencyService
    {
        private readonly IAmazonS3 _s3;
        private readonly S3ConcurrencyServiceOptions _options;
        private readonly ILogger<S3ConcurrencyService> _logger;

        /// <summary>
        /// Constructs a <see cref="S3ConcurrencyService" />.
        /// </summary>
        /// <param name="s3">A valid client for Amazon S3.</param>
        /// <param name="options">Configurable options: bucket name and file key prefix.</param>
        /// <param name="logger">Logger.</param>
        public S3ConcurrencyService(IAmazonS3 s3, IOptions<S3ConcurrencyServiceOptions> options, ILogger<S3ConcurrencyService> logger)
        {
            _s3 = s3 ?? throw new System.ArgumentNullException(nameof(s3));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new AmazonS3Exception(nameof(options));
        }

        /// <summary>
        /// Releases the lock by deleting the item for the specified <paramref name="uniqueToken" />.
        /// </summary>
        /// <param name="uniqueToken"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ReleaseLockAsync(string uniqueToken, CancellationToken cancellationToken = default)
        {
            if (await DoesLockObjectExist(uniqueToken, cancellationToken).ConfigureAwait(false))
            {
                await DeleteLockObject(uniqueToken, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Tries to acquire a lock by checking if an item for the same <paramref name="uniqueToken" /> exists already.
        /// If not, it acquires the lock by creating the item.
        /// </summary>
        /// <param name="uniqueToken">The token to be used as locking object.</param>
        /// <param name="cancellationToken">The cancellation token used to cancel the ongoing operation.</param>
        /// <returns><see langword="true" /> if the lock was acquired. <see langword="false" /> if the lock could not be acquired.</returns>
        public async Task<bool> TryAcquireLockAsync(string uniqueToken, CancellationToken cancellationToken = default)
        {
            try
            {
                if (await DoesLockObjectExist(uniqueToken, cancellationToken).ConfigureAwait(false))
                {
                    return false;
                }

                await CreateLockObject(uniqueToken, cancellationToken).ConfigureAwait(false);

                return true;
            }
            catch (AmazonS3Exception)
            {
                return false;
            }
        }

        private string GetItemKey(string uniqueToken)
        {
            return _options.FilePrefix switch
            {
                null => uniqueToken,
                _ => $"{_options.FilePrefix}/{uniqueToken}"
            };
        }

        private async Task<bool> DoesLockObjectExist(string uniqueToken, CancellationToken cancellationToken)
        {
            var itemKey = GetItemKey(uniqueToken);

            _logger.LogDebug("Testing if {TOKEN} exists", uniqueToken);

            try
            {
                var response = await _s3.GetObjectMetadataAsync(_options.BucketName, itemKey, cancellationToken).ConfigureAwait(false);

                _logger.LogDebug("Token {TOKEN} found: {ITEMKEY}", uniqueToken, itemKey);

                return true;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogDebug("Token {TOKEN} not found: {ITEMKEY}", uniqueToken, itemKey);

                return false;
            }
        }

        private Task CreateLockObject(string uniqueToken, CancellationToken cancellationToken)
        {
            var itemKey = GetItemKey(uniqueToken);

            _logger.LogDebug("Creating lock object for token {TOKEN} token: {ITEMKEY}", uniqueToken, itemKey);

            return _s3.PutObjectAsync(new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = itemKey,
                ContentBody = string.Empty
            }, cancellationToken);
        }

        private Task DeleteLockObject(string uniqueToken, CancellationToken cancellationToken)
        {
            var itemKey = GetItemKey(uniqueToken);

            _logger.LogDebug("Deleting lock object for token {TOKEN} token: {ITEMKEY}", uniqueToken, itemKey);

            return _s3.DeleteObjectAsync(_options.BucketName, itemKey, cancellationToken);
        }
    }
}
