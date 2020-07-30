namespace LostTech.Cloud.Azure {
    using System;
    using System.Text.Json.Serialization;

    [Serializable]
    public sealed class AzureInstance {
        [JsonPropertyName("compute")]
        public AzureInstanceCompute Compute { get; set; }
    }
}
