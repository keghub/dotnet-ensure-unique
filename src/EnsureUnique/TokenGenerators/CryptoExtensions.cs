using System;
using System.Text;

namespace EMG.Tools.EnsureUnique.TokenGenerators
{
    /// <summary>
    /// A set of cryptographic extensions.
    /// </summary>
    public static class CryptoExtensions
    {
        /// <summary>
        /// Calculates the MD5 hash of the given input.
        /// </summary>
        /// <param name="str">The string to hash.</param>
        /// <returns>A stringified version of the hash of the input.</returns>
        public static string MD5(this string str)
        {
            _ = str ?? throw new ArgumentNullException(nameof(str));

#pragma warning disable CA5351
            using var hasher = System.Security.Cryptography.MD5.Create();

            var bytes = hasher.ComputeHash(Encoding.Default.GetBytes(str));
#pragma warning restore CA5351

            var guid = new Guid(bytes);

            return guid.ToString("N");
        }
    }
}
