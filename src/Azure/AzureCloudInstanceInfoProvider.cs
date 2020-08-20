namespace LostTech.Cloud.Azure {
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;
    public sealed class AzureCloudInstanceInfoProvider : ICloudInstanceInfoProvider {
        internal readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://169.254.169.254") };

        static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public async Task<CloudInstanceInfo> GetInstanceInfoAsync(CancellationToken cancellation = default) {
            void CancelPending() {
                this.httpClient.CancelPendingRequests();
            }
            using var cancellationRegistration = cancellation.Register(CancelPending);

            var infoRequest = new HttpRequestMessage(HttpMethod.Get, "metadata/instance?api-version=2019-06-01");
            infoRequest.Headers.Add("Metadata", "true");
            using var infoResponse = await this.httpClient.SendAsync(infoRequest,
                HttpCompletionOption.ResponseContentRead, cancellation).ConfigureAwait(false);
            infoResponse.EnsureSuccessStatusCode();

            using var responseStream = await infoResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var response = await JsonSerializer.DeserializeAsync<AzureInstance>(responseStream, jsonSerializerOptions, cancellationToken: cancellation).ConfigureAwait(false);
            return new CloudInstanceInfo {
                Environment = $"{response.Compute.Provider}/{response.Compute.AzEnvironment}",
                Size = response.Compute.VirtualMachineSize,
                OwnerID = response.Compute.SubscriptionID.ToString(CloudInstanceInfo.GuidFormat),
            };
        }

        public AzureCloudInstanceInfoProvider() { }
        internal AzureCloudInstanceInfoProvider(HttpClient httpClient) {
            this.httpClient = httpClient;
        }
    }
}
