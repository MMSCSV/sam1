using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    public class TransactionSession: Entity<Guid>
    {
        #region Constructors

        public TransactionSession()
        {
        }

        public TransactionSession(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator TransactionSession(Guid key)
        {
            return FromKey(key);
        }

        public static TransactionSession FromKey(Guid key)
        {
            return new TransactionSession(key);
        }

        #endregion

        #region Public Properties

        public TransactionSessionTypeInternalCode TransactionSessionType { get; set; }

        public DateTime StartLocalDateTime { get; set; }

        public DateTime StartUtcDateTime { get; set; }

        public DateTime? EndLocalDateTime { get; set; }

        public DateTime? EndUtcDateTime { get; set; }

        public bool AbnormalEnd { get; set; }

        public Guid UserAccountKey { get; set; }

        public Guid FacilityKey { get; set; }

        public Guid? DispensingDeviceKey { get; set; }

        public DateTime? FirstRecoveryAttemptUtcDateTime { get; set; }

        public DateTime? FirstRecoveryAttemptLocalDateTime { get; set; }

        public DateTime? LastRecoveryAttemptUtcDateTime { get; set; }

        public DateTime? LastRecoveryAttemptLocalDateTime { get; set; }

        public bool UncrecoveredTransaction { get; set; }

        public int? RecoveryDataRecordQuantity { get; set; }

        public bool DoNotSendOutbound { get; set; }

        public bool Mobile { get; set; }

        public int? ItemTransactionRecordQuantity { get; set; }

        #endregion
    }
}
