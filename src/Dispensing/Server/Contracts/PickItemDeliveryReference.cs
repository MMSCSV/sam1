using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Server.Contracts
{
    [DataContract(Namespace = "http://schemas.carefusion.com/2011/05/dispensing/contracts")]
    public class PickItemDeliveryReferenceData
    {
        [DataMember]
        public Guid ItemDeliveryKey { get; set; }

        [DataMember]
        public Guid ItemDeliveryStateKey { get; set; }

        [DataMember]
        public string TransactionId { get; set; }

        [DataMember]
        public Guid LocationKey { get; set; }

        [DataMember]
        public Guid ItemKey { get; set; }

        [DataMember]
        public Guid ItemTransactionKey { get; set; }
    }
}
