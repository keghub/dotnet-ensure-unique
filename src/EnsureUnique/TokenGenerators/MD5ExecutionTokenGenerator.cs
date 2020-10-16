using System;
using System.Diagnostics;

namespace EMG.Tools.EnsureUnique.TokenGenerators
{
    /// <summary>
    /// An implementation of <see cref="IExecutionTokenGenerator" /> that calculates the MD5 hash of the <see cref="ProcessStartInfo.FileName" /> and <see cref="ProcessStartInfo.Arguments" />.
    /// </summary>
    public class MD5ExecutionTokenGenerator : IExecutionTokenGenerator
    {
        /// <summary>
        /// Calculates the MD5 hash of the <see cref="ProcessStartInfo.FileName" /> and <see cref="ProcessStartInfo.Arguments" />.
        /// </summary>
        /// <param name="startInfo">A representation of the process to generate the token for.</param>
        /// <returns>A stringified version of the MD5 hash.</returns>
        public string GenerateToken(ProcessStartInfo startInfo)
        {
            _ = startInfo ?? throw new ArgumentNullException(nameof(startInfo));

            return $"{startInfo.FileName} {startInfo.Arguments}".MD5();
        }
    }
}
