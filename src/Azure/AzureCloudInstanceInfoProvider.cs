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
            using var responseStream = await this.httpClient.GetStreamAsync("metadata/instance?api-version=2019-06-01").ConfigureAwait(false);
            cancellationRegistration.Dispose();

            var response = await JsonSerializer.DeserializeAsync<AzureInstance>(responseStream, jsonSerializerOptions, cancellationToken: cancellation).ConfigureAwait(false);
            return new CloudInstanceInfo {
                Environment = $"{response.Compute.Provider}/{response.Compute.AzEnvironment}",
                Size = response.Compute.VirtualMachineSize,
            };
        }

        public AzureCloudInstanceInfoProvider() { }
        internal AzureCloudInstanceInfoProvider(HttpClient httpClient) {
            this.httpClient = httpClient;
        }
    }
}
