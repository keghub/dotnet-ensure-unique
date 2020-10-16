using System;
using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace EMG.Tools.EnsureUnique.TokenGenerators
{
    /// <summary>
    /// An implementation of <see cref="IExecutionTokenGenerator" /> that returns a given token if available, or forwards the request to the inner token generator.
    /// </summary>
    public class FixedTokenExecutionTokenGeneratorAdapter : IExecutionTokenGenerator
    {
        private readonly TokenOptions _options;
        private readonly IExecutionTokenGenerator _inner;

        /// <summary>
        /// Constructs an adapter used to override the token generation process if a token is specified.
        /// </summary>
        /// <param name="inner">The adaptee.</param>
        /// <param name="options">The fixed token, if available.</param>
        public FixedTokenExecutionTokenGeneratorAdapter(IExecutionTokenGenerator inner, IOptions<TokenOptions> options)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc/>
        public string GenerateToken(ProcessStartInfo startInfo)
        {
            _ = startInfo ?? throw new ArgumentNullException(nameof(startInfo));

            return _options.Token ?? _inner.GenerateToken(startInfo);
        }
    }
}
