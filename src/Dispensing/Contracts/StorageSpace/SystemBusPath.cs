using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    [CollectionDataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public class SystemBusPath : List<SystemBusDevice>
    {
        public SystemBusPath()
        {
        }

        [DataMember]
        public bool IsFullyQualified { get; set; }
    }
}
