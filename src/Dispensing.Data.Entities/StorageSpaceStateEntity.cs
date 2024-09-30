using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class StorageSpaceStateEntity : IContractConvertible<StorageSpaceState>
    {
        #region IContractConvertible<StorageSpaceState> Members

        public StorageSpaceState ToContract()
        {
            StorageSpaceFailureReason failureReason = null;
            if (StorageSpaceFailureReasonInternalCode != null)
            {
                failureReason = StorageSpaceFailureReasonEntity != null
                    ? StorageSpaceFailureReasonEntity.ToContract()
                    : StorageSpaceFailureReasonInternalCode.FromInternalCode<StorageSpaceFailureReasonInternalCode>();
            }

            return new StorageSpaceState(Key)
            {
               Closed = ClosedFlag,
               Defrost = DefrostFlag,
               Failed = FailedFlag,
               Locked = LockedFlag,
               OnBattery = OnBatteryFlag,
               FailureRequiresMaintenance = FailureRequiresMaintenanceFlag,
               FailureReason = failureReason,
               StorageSpaceKey = StorageSpaceKey,
               LastModified = LastModifiedBinaryValue.ToArray(),
            };
        }

        #endregion
    }
}
