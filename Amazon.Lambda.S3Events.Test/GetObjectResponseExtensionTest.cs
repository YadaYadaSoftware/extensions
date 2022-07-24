using Amazon.Lambda.S3Events;
using Amazon.S3.Model;
using FluentAssertions;
using Xunit;

namespace YadaYada.Lambda
{
    public class GetObjectResponseExtensionTest
    {
        [Fact]
        public void TryGetUserAgentTest()
        {
            GetObjectResponse response = new GetObjectResponse();
            
            response.TryGetUserAgent(out string userAgent).Should().BeFalse();
            const string myuseragent = "myuseragent";
            response.Metadata.Add(S3GetObjectResponseExtensions.Constants.MetadataKeys.UserAgent, myuseragent);
            response.TryGetUserAgent(out userAgent).Should().BeTrue();
            userAgent.Should().Be(myuseragent);
        }
    }
}
