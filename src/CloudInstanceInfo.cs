namespace LostTech.Cloud {
    using System;
    using System.Collections.Generic;
    [Serializable]
    public sealed class CloudInstanceInfo {
        public string Size { get; set; }
        public string Environment { get; set; }
        /// <summary>
        /// A unique publicly accessible identifier for the owner of this resource.
        /// </summary>
        public string? OwnerID { get; set; }
        /// <remarks>This list might be incomplete</remarks>>
        public ICollection<AttachedDevice> AttachedDevices { get; } = new List<AttachedDevice>();

        /// <summary>
        /// When GUIDs need to be represented by a string, this format is used.
        /// </summary>
        public const string GuidFormat = "D";
    }
}
