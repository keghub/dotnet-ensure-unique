using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using EMG.Tools.EnsureUnique.Concurrency;
using Moq;
using NUnit.Framework;

namespace Tests.Concurrency
{
    [TestFixture]
    public class S3ConcurrencyServiceTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(S3ConcurrencyService).GetConstructors());
        }

        [Test, CustomAutoData]
        public async Task TryAcquireLockAsync_checks_if_file_exists([Frozen] S3ConcurrencyServiceOptions options, [Frozen] IAmazonS3 s3, S3ConcurrencyService sut, string token)
        {
            Mock.Get(s3)
                .Setup(p => p.GetObjectMetadataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetObjectMetadataResponse { HttpStatusCode = HttpStatusCode.OK });

            await sut.TryAcquireLockAsync(token);

            Mock.Get(s3).Verify(p => p.GetObjectMetadataAsync(options.BucketName, It.Is<string>(s => s.StartsWith(options.FilePrefix) && s.EndsWith(token)), It.IsAny<CancellationToken>()));
        }

        [Test, CustomAutoData]
        public async Task TryAcquireLockAsync_creates_file_if_not_exists([Frozen] S3ConcurrencyServiceOptions options, [Frozen] IAmazonS3 s3, S3ConcurrencyService sut, string token)
        {
            Mock.Get(s3)
                .Setup(p => p.GetObjectMetadataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AmazonS3Exception("NotFound") { StatusCode = HttpStatusCode.NotFound });

            await sut.TryAcquireLockAsync(token);

            Mock.Get(s3).Verify(p => p.PutObjectAsync(It.Is<PutObjectRequest>(r => r.BucketName == options.BucketName && r.Key.StartsWith(options.FilePrefix) && r.Key.EndsWith(token) && r.ContentBody == string.Empty), It.IsAny<CancellationToken>()));
        }

        [Test, CustomAutoData]
        public async Task ReleaseLockAsync_checks_if_file_exists([Frozen] S3ConcurrencyServiceOptions options, [Frozen] IAmazonS3 s3, S3ConcurrencyService sut, string token)
        {
            Mock.Get(s3)
                .Setup(p => p.GetObjectMetadataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetObjectMetadataResponse { HttpStatusCode = HttpStatusCode.OK });

            await sut.ReleaseLockAsync(token);

            Mock.Get(s3).Verify(p => p.GetObjectMetadataAsync(options.BucketName, It.Is<string>(s => s.StartsWith(options.FilePrefix) && s.EndsWith(token)), It.IsAny<CancellationToken>()));
        }

        [Test, CustomAutoData]
        public async Task ReleaseLockAsync_removes_file_if_exists([Frozen] S3ConcurrencyServiceOptions options, [Frozen] IAmazonS3 s3, S3ConcurrencyService sut, string token)
        {
            Mock.Get(s3)
                .Setup(p => p.GetObjectMetadataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetObjectMetadataResponse { HttpStatusCode = HttpStatusCode.OK });

            await sut.ReleaseLockAsync(token);

            Mock.Get(s3).Verify(p => p.DeleteObjectAsync(options.BucketName, It.Is<string>(s => s.StartsWith(options.FilePrefix) && s.EndsWith(token)), It.IsAny<CancellationToken>()));
        }
    }
}