using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public sealed class ActiveDirectoryDomainActor : Actor
    {
        public ActiveDirectoryDomainActor(Guid activeDirectoryDomainKey)
            : base(ActorType.ActiveDirectoryDomain, activeDirectoryDomainKey)
        { }
    }
}
