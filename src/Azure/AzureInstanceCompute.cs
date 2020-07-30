namespace LostTech.Cloud.Azure {
    using System;
    using System.Text.Json.Serialization;
    [Serializable]
    public class AzureInstanceCompute {
        public string AzEnvironment { get; set; }
        public string OSType { get; set; }
        public string Provider { get; set; }
        [JsonPropertyName("subscriptionId")]
        public Guid SubscriptionID { get; set; }
        [JsonPropertyName("vmId")]
        public Guid VirtualMachineID { get; set; }
        [JsonPropertyName("vmSize")]
        public string VirtualMachineSize { get; set; }

        public sealed class OSTypes {
            public const string Windows = "Windows";
            public const string Linux = "Linux";
        }

        public sealed class Providers {
            public const string Compute = "Microsoft.Compute";
        }

        public sealed class Environments {
            public const string PublicCloud = "AzurePublicCloud";
        }
    }
}