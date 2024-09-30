using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class TransactionSessionEntity : IContractConvertible<TransactionSession>
    {
        #region IContractConvertible<TransactionSession> Members

        public TransactionSession ToContract()
        {
            return new TransactionSession(Key)
                {
                    TransactionSessionType = TransactionSessionTypeInternalCode.FromInternalCode<TransactionSessionTypeInternalCode>(),
                    StartLocalDateTime = SessionStartLocalDateTime,
                    StartUtcDateTime = SessionStartUTCDateTime,
                    EndLocalDateTime = SessionEndLocalDateTime,
                    EndUtcDateTime = SessionEndUTCDateTime,
                    AbnormalEnd = AbnormalEndFlag,
                    UserAccountKey = UserAccountKey,
                    FacilityKey = FacilityKey,
                    DispensingDeviceKey = DispensingDeviceKey,
                    FirstRecoveryAttemptLocalDateTime = FirstRecoveryAttemptLocalDateTime,
                    FirstRecoveryAttemptUtcDateTime = FirstRecoveryAttemptUTCDateTime,
                    LastRecoveryAttemptLocalDateTime = LastRecoveryAttemptLocalDateTime,
                    LastRecoveryAttemptUtcDateTime = LastRecoveryAttemptUTCDateTime,
                    UncrecoveredTransaction = UnrecoveredTransactionFlag,
                    RecoveryDataRecordQuantity = RecoveryDataRecordQuantity,
                    DoNotSendOutbound = DoNotSendOutboundFlag
                };
        }

        #endregion
    }
}
