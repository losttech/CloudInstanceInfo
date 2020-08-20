namespace LostTech.Cloud.GCP {
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class GoogleComputeInstanceInfoProvider : ICloudInstanceInfoProvider {
        readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://metadata.google.internal/computeMetadata/v1/") };

        static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public async Task<CloudInstanceInfo> GetInstanceInfoAsync(CancellationToken cancellation = default) {
            void CancelPending() {
                this.httpClient.CancelPendingRequests();
            }

            using var cancellationRegistration = cancellation.Register(CancelPending);

            var infoRequest = new HttpRequestMessage(HttpMethod.Get, "instance/?recursive=true&alt=json");
            infoRequest.Headers.Add("Metadata-Flavor", "Google");
            using var infoResponse = await this.httpClient.SendAsync(infoRequest,
                HttpCompletionOption.ResponseContentRead, cancellation).ConfigureAwait(false);
            infoResponse.EnsureSuccessStatusCode();

            using var responseStream = await infoResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var response = await JsonSerializer.DeserializeAsync<GoogleCloudInstanceMetadata>(
                responseStream, jsonSerializerOptions, cancellation).ConfigureAwait(false);

            string[] machineTypeParts = response.MachineType.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (machineTypeParts.Length != 4
                || machineTypeParts[0] != "projects" || machineTypeParts[2] != "machineTypes")
                throw new FormatException($"Unrecognized format of {nameof(response.MachineType)}: {response.MachineType}");

            return new CloudInstanceInfo {
                Environment = "GCP/Compute",
                Size = machineTypeParts[3],
                // project ID
                OwnerID = machineTypeParts[1],
            };
        }

        public GoogleComputeInstanceInfoProvider() { }
        internal GoogleComputeInstanceInfoProvider(HttpClient httpClient) {
            this.httpClient = httpClient;
        }
    }
}
