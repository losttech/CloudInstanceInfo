namespace LostTech.Cloud.AWS {
    using System;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class AwsCloudInstanceInfoProvider : ICloudInstanceInfoProvider {
        internal readonly HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://169.254.169.254/latest/") };

        static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public async Task<CloudInstanceInfo> GetInstanceInfoAsync(CancellationToken cancellation = default) {
            void CancelPending() {
                this.httpClient.CancelPendingRequests();
            }

            using var cancellationRegistration = cancellation.Register(CancelPending);

            var tokenRequest = new HttpRequestMessage(HttpMethod.Put, "api/token");
            tokenRequest.Headers.Add("X-aws-ec2-metadata-token-ttl-seconds", "180");
            var tokenResponse = await this.httpClient.SendAsync(tokenRequest,
                HttpCompletionOption.ResponseContentRead, cancellation).ConfigureAwait(false);
            tokenResponse.EnsureSuccessStatusCode();

            string token = await tokenResponse.Content.ReadAsStringAsync();

            var infoRequest = new HttpRequestMessage(HttpMethod.Get, "dynamic/instance-identity/document");
            infoRequest.Headers.Add("X-aws-ec2-metadata-token", token);
            var infoResponse = await this.httpClient.SendAsync(infoRequest,
                HttpCompletionOption.ResponseContentRead, cancellation).ConfigureAwait(false);
            infoResponse.EnsureSuccessStatusCode();

            using var responseStream = await infoResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var response = await JsonSerializer.DeserializeAsync<AwsInstanceIdentityDocument>(
                responseStream, jsonSerializerOptions, cancellation).ConfigureAwait(false);

            return new CloudInstanceInfo {
                Environment = "AWS/EC2",
                OwnerID = response.AccountID,
                Size = response.InstanceType,
            };
        }

        public AwsCloudInstanceInfoProvider() { }
        internal AwsCloudInstanceInfoProvider(HttpClient httpClient) {
            this.httpClient = httpClient;
        }
    }
}
