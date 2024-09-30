using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Server.Contracts
{
    [DataContract(Namespace = "http://schemas.carefusion.com/2011/05/dispensing/contracts")]
    public class DispensingDeviceItemInventoryData
    {
        [DataMember]
        public Guid DispensingDeviceKey { get; set; }

        [DataMember]
        public string DispensingDeviceName { get; set; }
        [DataMember]
        public Guid ItemKey { get; set; }

        [DataMember]
        public decimal InventoryQuantity { get; set; }

        [DataMember]
        public bool BlindCount { get; set; }

        [DataMember]
        public bool DispenseFraction { get; set; }

        [DataMember]
        public Guid AreaKey { get; set; }

        [DataMember]
        public string AreaName { get; set; }

        [DataMember]
        public bool HasRemovePermission { get; set; }

        [DataMember]
        public bool SameAreaAsCurrentDevice { get; set; }
    }
}
