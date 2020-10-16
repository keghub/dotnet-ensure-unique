using System.Diagnostics;

namespace EMG.Tools.EnsureUnique.TokenGenerators
{
    /// <summary>
    /// Implementers of this interface can be used to create a unique token given a <see cref="ProcessStartInfo" />.
    /// </summary>
    public interface IExecutionTokenGenerator
    {
        /// <summary>
        /// Calculates a unique execution token for a given <see cref="ProcessStartInfo" />.
        /// </summary>
        /// <param name="startInfo">A representation of the process to generate the token for.</param>
        /// <returns>An hash based on the given <see cref="ProcessStartInfo" />.</returns>
        string GenerateToken(ProcessStartInfo startInfo);
    }
}
