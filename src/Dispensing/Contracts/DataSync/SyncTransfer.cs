using System;
using System.Runtime.Serialization;
using ProtoBuf;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    [DataContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    [ProtoContract(UseProtoMembersOnly = true)]
    public class SyncTransfer : Entity<Guid>
    {
        #region Contructors

        public SyncTransfer()
        {
        }

        public SyncTransfer(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator SyncTransfer(Guid key)
        {
            return FromKey(key);
        }

        public static SyncTransfer FromKey(Guid key)
        {
            return new SyncTransfer(key);
        }

        #endregion

        #region Contract Properties

        [DataMember]
        [ProtoMember(1)]
        public Guid DispensingDeviceKey { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public SyncTransferTypeInternalCode TransferType { get; set; }

        [DataMember]
        [ProtoMember(3)]
        public SyncTransferStatusInternalCode TransferStatus { get; set; }

        [DataMember]
        [ProtoMember(4)]
        public DateTime StartUtcDateTime { get; set; }

        [DataMember]
        [ProtoMember(5)]
        public DateTime StartLocalDateTime { get; set; }

        [DataMember]
        [ProtoMember(6)]
        public int NumberOfChangesRetrieved { get; set; }

        [DataMember]
        [ProtoMember(7)]
        public int NumberOfChangesApplied { get; set; }

        [DataMember]
        [ProtoMember(8)]
        public int TimeToRetrieveChanges { get; set; }

        [DataMember]
        [ProtoMember(9)]
        public int TimeToApplyChanges { get; set; }

        [DataMember]
        [ProtoMember(10)]
        public int TotalTime { get; set; }

        [DataMember]
        [ProtoMember(11)]
        public string ErrorDetails { get; set; }

        #endregion

        public string TransferTypeDisplayString 
        { 
            get 
            { 
                return TransferType.ToInternalCode(); 
            } 
        }

        public string TransferStatusDisplayString 
        { 
            get 
            { 
                return TransferStatus.ToInternalCode(); 
            } 
        }
    }
}
