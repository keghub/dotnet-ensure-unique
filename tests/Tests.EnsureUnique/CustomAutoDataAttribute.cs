using System;
using System.Diagnostics;
using Amazon.S3.Model;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;
using EMG.Tools.EnsureUnique;
using EMG.Tools.EnsureUnique.TokenGenerators;

namespace Tests
{
    public class CustomAutoDataAttribute : AutoDataAttribute
    {
        public CustomAutoDataAttribute() : base(FixtureHelper.CreateFixture) { }
    }

    public class InlineCustomAutoDataAttribute : InlineAutoDataAttribute
    {
        public InlineCustomAutoDataAttribute(params object[] arguments) : base(FixtureHelper.CreateFixture, arguments) { }
    }

    public static class FixtureHelper
    {
        public static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            fixture.Customize(new AutoMoqCustomization
            {
                ConfigureMembers = true,
                GenerateDelegates = true
            });

            fixture.Register((string program, string args) => new ProcessStartInfo { FileName = program, Arguments = args });

            fixture.Inject(new TokenOptions());

            fixture.Customize<GetObjectMetadataResponse>(c => c.Without(p => p.RequestCharged));

            fixture.Customize<PutObjectResponse>(c => c.Without(p => p.RequestCharged));

            fixture.Customize<DeleteObjectResponse>(c => c.Without(p => p.RequestCharged));

            return fixture;
        }
    }
}