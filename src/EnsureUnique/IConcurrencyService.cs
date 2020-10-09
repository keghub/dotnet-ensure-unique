using System.Threading.Tasks;
using Amazon.S3;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EMG.Tools.EnsureUnique
{
    public interface IConcurrencyService
    {
        Task<bool> TryAcquireLockAsync(string uniqueToken);

        Task ReleaseLockAsync(string uniqueToken);
    }

    public class DummyConcurrencyService : IConcurrencyService
    {
        public Task ReleaseLockAsync(string uniqueToken) => Task.CompletedTask;

        public Task<bool> TryAcquireLockAsync(string uniqueToken) => Task.FromResult(true);
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

        public Task ReleaseLockAsync(string uniqueToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> TryAcquireLockAsync(string uniqueToken)
        {
            throw new System.NotImplementedException();
        }
    }

    public class S3ConcurrencyServiceOptions
    {
        public string BucketName { get; set; }

        public string FilePrefix { get; set; }
    }
}
