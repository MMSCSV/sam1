using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public sealed class ExternalSystemActor : Actor
    {
        public ExternalSystemActor(Guid externalSystemKey)
            : base(ActorType.ExternalSystem, externalSystemKey)
        { }
    }
}
