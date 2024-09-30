using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public sealed class UserActor : Actor
    {
        public UserActor(Guid userAccountKey)
            : base(ActorType.User, userAccountKey)
        { }
    }
}
