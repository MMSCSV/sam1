using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public class DateTimePair
    {
        [DataMember]
        public DateTime? UtcDateTime { get; set; }

        [DataMember]
        public DateTime? LocalDateTime { get; set; }
    }
}
