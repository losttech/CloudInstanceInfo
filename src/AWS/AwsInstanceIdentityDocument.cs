namespace LostTech.Cloud.AWS {
    using System;
    using System.Text.Json.Serialization;

    [Serializable]
    public class AwsInstanceIdentityDocument {
        [JsonPropertyName("accountId")]
        public string AccountID { get; set; }
        public string InstanceType { get; set; }
        public string Region { get; set; }
    }
}
