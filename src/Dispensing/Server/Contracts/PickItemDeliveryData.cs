using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Server.Contracts
{
    [DataContract(Namespace = "http://schemas.carefusion.com/2011/05/dispensing/contracts")]
    public class PickItemDeliveryData
    {
        [DataMember]
        public Guid FulfillmentFacilityKey { get; set; }

        [DataMember]
        public Guid? DispensingDeviceKey { get; set; }

        [DataMember]
        public Guid ItemKey { get; set; }

        [DataMember]
        public Guid? StorageSpaceKey { get; set; }
        
        [DataMember]
        public decimal PickQuantity { get; set; }

        [DataMember]
        public DateTime? EarliestExpirationDate { get; set; }
        
        [DataMember]
        public List<ItemDeliveryScanData> Scans { get; set; }

        [DataMember]
        public bool OnDemand { get; set; }

        [DataMember]
        public Guid LocationKey { get; set; }

        [DataMember]
        public Guid ItemTransactionKey { get; set; }
    }
}
