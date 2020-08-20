namespace LostTech.Cloud.GCP {
    using System;
    using System.Threading.Tasks;

    using RichardSzalay.MockHttp;

    using Xunit;
    public class GoogleCloudProviderTests {
        [Fact]
        public async Task CorrectlyParsesResponse() {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://metadata.google.internal/computeMetadata/v1/instance/?recursive=true&alt=json")
                    .WithHeaders("Metadata-Flavor", "Google")
                    .Respond("application/json", SampleResponse);
            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://metadata.google.internal/computeMetadata/v1/");
            var provider = new GoogleComputeInstanceInfoProvider(httpClient);
            var info = await provider.GetInstanceInfoAsync();
            Assert.Equal(0, info.AttachedDevices.Count);
            Assert.Equal("f1-micro", info.Size);
            Assert.Equal("GCP/Compute", info.Environment);
            Assert.Equal("824486570239", info.OwnerID);
        }

        const string SampleResponse = @"
{
    ""description"": """",
    ""id"": 8927746261769023583,
    ""legacyEndpointAccess"": {
        ""0.1"": 0,
        ""v1beta1"": 0
    },
    ""machineType"": ""projects/824486570239/machineTypes/f1-micro"",
    ""maintenanceEvent"": ""NONE"",
    ""name"": ""cii-test"",
    ""preempted"": ""FALSE"",
    ""remainingCpuTime"": -1,
    ""zone"": ""projects/824486570239/zones/us-west1-b""
}";
    }
}
