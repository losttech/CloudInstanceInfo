namespace LostTech.Cloud.AWS {
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using RichardSzalay.MockHttp;
    using Xunit;

    public class AwsProviderTest
    {
        [Fact]
        public async Task CorrectlyParsesResponse() {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Put, "http://169.254.169.254/2018-11-29/api/token")
                .WithHeaders("X-aws-ec2-metadata-token-ttl-seconds", "180")
                .Respond("text/plain", SampleToken);
            mockHttp.When("http://169.254.169.254/2018-11-29/dynamic/instance-identity/document")
                .WithHeaders("X-aws-ec2-metadata-token", SampleToken)
                .Respond("application/json", SampleResponse);
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://169.254.169.254/2018-11-29/");

            var provider = new AwsCloudInstanceInfoProvider(httpClient);
            var info = await provider.GetInstanceInfoAsync();

            Assert.Equal(0, info.AttachedDevices.Count);
            Assert.Equal("m1.small", info.Size);
            Assert.Equal("AWS/EC2", info.Environment);
            Assert.Equal("123456789abc", info.OwnerID);
        }

        const string SampleToken = "TestToken";
        const string SampleResponse = @"
{
    ""devpayProductCodes"" : null,
    ""privateIp"" : ""10.1.2.3"",
    ""region"" : ""us-east-1"",
    ""kernelId"" : ""aki-12345678"",
    ""ramdiskId"" : null,
    ""availabilityZone"" : ""us-east-1a"",
    ""accountId"" : ""123456789abc"",
    ""version"" : ""2010-08-31"",
    ""instanceId"" : ""i-12345678"",
    ""billingProducts"" : null,
    ""architecture"" : ""x86_64"",
    ""imageId"" : ""ami-12345678"",
    ""pendingTime"" : ""2014-01-23T45:01:23Z"",
    ""instanceType"" : ""m1.small""
}";
    }
}
