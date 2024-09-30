using System;

namespace CareFusion.Dispensing.Contracts
{
    public class StorageSpaceInventory : Entity<Guid>
    {
        #region Constructors

        public StorageSpaceInventory()
        {
        }

        public StorageSpaceInventory(Guid key)
        {
            Key = key;
        }

        public StorageSpaceInventory(Guid key, byte[] lastModified)
        {
            Key = key;
            LastModified = lastModified;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator StorageSpaceInventory(Guid key)
        {
            return FromKey(key);
        }

        public static StorageSpaceInventory FromKey(Guid key)
        {
            return new StorageSpaceInventory(key);
        }

        #endregion

        public Guid StorageSpaceItemKey { get; set; }

        public decimal? InventoryQuantity { get; set; }

        public decimal? StrengthInventoryQuantity { get; set; }

        public Guid? StrengthUnitOfMeasureKey { get; set; }

        public decimal? VolumeInventoryQuantity { get; set; }

        public Guid? VolumeUnitOfMeasureKey { get; set; }

        public DateTime? EarliestNextExpirationDate { get; set; }
    }
}
