using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class CountInventoryListEntryEntity : IContractConvertible<CountInventoryListEntry>
    {
        #region Implementation of IContractConvertible<CountInventoryListEntry>

        public CountInventoryListEntry ToContract()
        {
            return new CountInventoryListEntry(Key)
                {
                    ItemTransactionKey = ItemTransactionKey,
                    Suspend = SuspendFlag,
                    StorageSpaceItemKey = StorageSpaceItemKey,
                    LastModified = LastModifiedBinaryValue.ToArray()
                };
        }

        #endregion
    }
}
