using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public enum ActorType 
    { 
        [EnumMember]
        [InternalCode("USER")]
        User, 

        [EnumMember]
        [InternalCode("EXTSYS")]
        ExternalSystem, 

        [EnumMember]
        [InternalCode("ADDOMAIN")]
        ActiveDirectoryDomain
    }

    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    [KnownType(typeof(UserActor))]
    [KnownType(typeof(ExternalSystemActor))]
    public abstract class Actor
    {
        [DataMember(Name = "ActorType")]
        private ActorType _actorType;
        [DataMember(Name = "Key")]
        private Guid _key;

        #region Constructors

        protected Actor(ActorType actorType, Guid key)
        {
            _actorType = actorType;
            _key = key;
        }

        #endregion

        #region Operator Overloads

        public static explicit operator Guid?(Actor actor)
        {
            return actor != null ? actor.Key : default(Guid?);
        }

        #endregion

        public Guid Key
        {
            get { return _key; }
        }

        public ActorType ActorType 
        {
            get { return _actorType; }
        }
    }
}
