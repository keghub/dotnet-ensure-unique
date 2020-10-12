using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EMG.Tools.EnsureUnique
{
    public interface IConcurrencyService
    {
        Task<bool> TryAcquireLockAsync(string uniqueToken, CancellationToken cancellationToken = default);

        Task ReleaseLockAsync(string uniqueToken, CancellationToken cancellationToken = default);
    }

    public class DummyConcurrencyService : IConcurrencyService
    {
        public Task ReleaseLockAsync(string uniqueToken, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<bool> TryAcquireLockAsync(string uniqueToken, CancellationToken cancellationToken = default) => Task.FromResult(true);
    }

    public class S3ConcurrencyService : IConcurrencyService
    {
        private readonly IAmazonS3 _s3;
        private readonly S3ConcurrencyServiceOptions _options;
        private readonly ILogger<S3ConcurrencyService> _logger;

        public S3ConcurrencyService(IAmazonS3 s3, IOptions<S3ConcurrencyServiceOptions> options, ILogger<S3ConcurrencyService> logger)
        {
            _s3 = s3 ?? throw new System.ArgumentNullException(nameof(s3));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new AmazonS3Exception(nameof(options));
        }

        public async Task ReleaseLockAsync(string uniqueToken, CancellationToken cancellationToken = default)
        {
            if (await DoesLockObjectExist(uniqueToken, cancellationToken))
            {
                await DeleteLockObject(uniqueToken, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<bool> TryAcquireLockAsync(string uniqueToken, CancellationToken cancellationToken = default)
        {
            if (await DoesLockObjectExist(uniqueToken, cancellationToken))
            {
                return false;
            }

            await CreateLockObject(uniqueToken, cancellationToken);
            
            return true;
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

    public class S3ConcurrencyServiceOptions
    {
        public string BucketName { get; set; }

        public string FilePrefix { get; set; }
    }
}
