using System.Diagnostics;
using AutoFixture.Idioms;
using EMG.Tools.EnsureUnique.ProcessRunners;
using NUnit.Framework;

namespace Tests.ProcessRunners
{
    [TestFixture]
    [TestOf(typeof(ProcessRunner))]
    public class ProcessRunnerTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ProcessRunner).GetConstructors());
        }
    }
}