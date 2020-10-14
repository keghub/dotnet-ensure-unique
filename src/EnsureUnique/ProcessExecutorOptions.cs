namespace EMG.Tools.EnsureUnique
{
    /// <summary>
    /// A set of configurable options used by <see cref="DefaultProcessExecutor" />.
    /// </summary>
    public class ProcessExecutorOptions
    {
        /// <summary>
        /// The token to be used to guarantee unique executions.
        /// </summary>
        public string Token { get; set; }
    }
}
