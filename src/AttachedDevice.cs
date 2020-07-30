namespace LostTech.Cloud {
    using System;
    [Serializable]
    public class AttachedDevice {
        public string Type { get; set; }
        public string Name { get; set; }
        public decimal Size { get; set; }
        public string SizeUnits { get; set; }
    }
}
