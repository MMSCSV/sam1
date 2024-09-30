using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class SyncTransferEntity : IContractConvertible<SyncTransfer>
    {
        public SyncTransfer ToContract()
        {
            return new SyncTransfer(Key)
            {
                StartUtcDateTime = TransferStartUTCDateTime,
                StartLocalDateTime = TransferStartLocalDateTime,
                NumberOfChangesRetrieved = RetrieveRecordQuantity.GetValueOrDefault(),
                TimeToRetrieveChanges = RetrieveDurationAmount.GetValueOrDefault(),
                NumberOfChangesApplied = ApplyRecordQuantity.GetValueOrDefault(),
                TimeToApplyChanges = ApplyDurationAmount.GetValueOrDefault(),
                TotalTime = TotalDurationAmount.GetValueOrDefault(),
                ErrorDetails = ErrorText,
                TransferType = SyncTransferTypeInternalCode.FromInternalCode<SyncTransferTypeInternalCode>(),
                TransferStatus = SyncTransferStatusInternalCode.FromInternalCode<SyncTransferStatusInternalCode>()
            };
        }
    }
}
