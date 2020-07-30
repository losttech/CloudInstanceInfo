namespace LostTech.Cloud {
    using System;
    using System.Collections.Generic;
    [Serializable]
    public sealed class CloudInstanceInfo {
        public string Size { get; set; }
        public string Environment { get; set; }
        public ICollection<AttachedDevice> AttachedDevices { get; } = new List<AttachedDevice>();
    }
}
