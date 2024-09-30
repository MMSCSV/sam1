using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class StorageSpaceInventoryEntity : IContractConvertible<StorageSpaceInventory>
    {
        #region IContractConvertible<StorageSpaceItem> Members
    
        public StorageSpaceInventory ToContract()
        {   
            return new StorageSpaceInventory(Key, LastModifiedBinaryValue.ToArray())
            {
                StorageSpaceItemKey = StorageSpaceItemKey,
                InventoryQuantity = InventoryQuantity,
                StrengthInventoryQuantity = StrengthInventoryQuantity,
                StrengthUnitOfMeasureKey = StrengthUOMKey,
                VolumeInventoryQuantity = VolumeInventoryQuantity,
                VolumeUnitOfMeasureKey = VolumeUOMKey,
                EarliestNextExpirationDate = EarliestNextExpirationDate,
            };
        }

        #endregion
    }
}
