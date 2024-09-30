using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Server.Contracts
{
    [DataContract(Namespace = "http://schemas.carefusion.com/2011/05/dispensing/contracts")]
    public class ItemDeliveryScanData
    {
        [DataMember]
        public string ScanCode { get; set; }

        [DataMember]
        public bool Override { get; set; }

        [DataMember]
        public string ProductId { get; set; }

        [DataMember]
        public string LotId { get; set; }

        [DataMember]
        public string SerialId { get; set; }

        [DataMember]
        public DateTime? ExpirationDate { get; set; }

        [DataMember]
        public bool IsDayNull { get; set; }
    }
}
