using System.Linq;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class CountInventoryListEntity : IContractConvertible<CountInventoryList>
    {
        #region Implementation of IContractConvertible<CountInventoryList>

        public CountInventoryList ToContract()
        {
            CountInventoryListEntry[] entries = CountInventoryListEntryEntities
                .Select(cile => cile.ToContract())
                .ToArray();

            return new CountInventoryList(Key)
                {
                    DispensingDeviceKey = DispensingDeviceKey,
                    Entries = entries,
                    UserAccountKey = UserAccountKey,
                    LastModified = LastModifiedBinaryValue.ToArray(),
                    FinishUTCDateTime = FinishUTCDateTime,
                    FinishOrCompleteFlag = FinishOrCompleteFlag
                };

        }

        #endregion
    }
}
