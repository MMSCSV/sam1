using System;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public class StorageSpaceState : Entity<Guid>
    {
        #region Constructors

        public StorageSpaceState()
        {
        }

        public StorageSpaceState(bool closed, bool locked, bool failed)
        {
            this.Closed = closed;
            this.Locked = locked;
            this.Failed = failed;
        }

        public StorageSpaceState(bool closed, bool locked, bool failed, Guid storageSpaceKey)
        {
            this.Closed = closed;
            this.Locked = locked;
            this.Failed = failed;
            this.StorageSpaceKey = storageSpaceKey;
        }

        public StorageSpaceState(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator StorageSpaceState(Guid key)
        {
            return FromKey(key);
        }

        public static StorageSpaceState FromKey(Guid key)
        {
            return new StorageSpaceState(key);
        }

        #endregion

        #region Public Properties

        [DataMember]
        public Guid StorageSpaceKey { get; set; }

        [DataMember]
        public bool Closed { get; set; }

        [DataMember]
        public bool Locked { get; set; }

        [DataMember]
        public bool Failed { get; set; }

        [DataMember]
        public bool Defrost { get; set; }

        [DataMember]
        public bool OnBattery { get; set; }

        [DataMember]
        public bool FailureRequiresMaintenance { get; set; }

        [DataMember]
        public StorageSpaceFailureReason FailureReason { get; set; }

        [DataMember]
        public string StorageSpaceShortName { get; set; }

        [DataMember]
        public string StorageSpaceAbbreviatedName { get; set; }

        #endregion
    }
}
