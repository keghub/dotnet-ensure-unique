namespace EMG.Tools.EnsureUnique.Concurrency
{
    /// <summary>
    /// Configurable options for <see cref="S3ConcurrencyService" />.
    /// </summary>
    public class S3ConcurrencyServiceOptions
    {
        /// <summary>
        /// The bucket where to store the lock object.
        /// </summary>
        public string? BucketName { get; set; }

        /// <summary>
        /// The prefix to be prepended to the file key used as lock object.
        /// </summary>
        public string? FilePrefix { get; set; }
    }
}
