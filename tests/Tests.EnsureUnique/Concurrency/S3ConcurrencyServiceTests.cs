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
using static Tests.Concurrency.S3Expectations;

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

            Mock.Get(s3).Verify(p => p.GetObjectMetadataAsync(options.BucketName, ObjectName(options.FilePrefix, token), It.IsAny<CancellationToken>()));
        }

        [Test, CustomAutoData]
        public async Task TryAcquireLockAsync_creates_file_if_not_exists([Frozen] S3ConcurrencyServiceOptions options, [Frozen] IAmazonS3 s3, S3ConcurrencyService sut, string token)
        {
            Mock.Get(s3)
                .Setup(p => p.GetObjectMetadataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new AmazonS3Exception("NotFound") { StatusCode = HttpStatusCode.NotFound });

            await sut.TryAcquireLockAsync(token);

            Mock.Get(s3).Verify(p => p.PutObjectAsync(PutRequest(options, token), It.IsAny<CancellationToken>()));
        }

        [Test, CustomAutoData]
        public async Task ReleaseLockAsync_checks_if_file_exists([Frozen] S3ConcurrencyServiceOptions options, [Frozen] IAmazonS3 s3, S3ConcurrencyService sut, string token)
        {
            Mock.Get(s3)
                .Setup(p => p.GetObjectMetadataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetObjectMetadataResponse { HttpStatusCode = HttpStatusCode.OK });

            await sut.ReleaseLockAsync(token);

            Mock.Get(s3).Verify(p => p.GetObjectMetadataAsync(options.BucketName, ObjectName(options.FilePrefix, token), It.IsAny<CancellationToken>()));
        }

        [Test, CustomAutoData]
        public async Task ReleaseLockAsync_removes_file_if_exists([Frozen] S3ConcurrencyServiceOptions options, [Frozen] IAmazonS3 s3, S3ConcurrencyService sut, string token)
        {
            Mock.Get(s3)
                .Setup(p => p.GetObjectMetadataAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetObjectMetadataResponse { HttpStatusCode = HttpStatusCode.OK });

            await sut.ReleaseLockAsync(token);

            Mock.Get(s3).Verify(p => p.DeleteObjectAsync(options.BucketName, ObjectName(options.FilePrefix, token), It.IsAny<CancellationToken>()));
        }
    }

    public static class S3Expectations
    {
        public static string ObjectName(string prefix, string token) => Match.Create<string>(s => s.StartsWith(prefix) && s.EndsWith(token));

        public static PutObjectRequest PutRequest(S3ConcurrencyServiceOptions options, string token) => Match.Create<PutObjectRequest>(r => r.BucketName == options.BucketName && r.Key.StartsWith(options.FilePrefix) && r.Key.EndsWith(token) && r.ContentBody == string.Empty);
    }
}