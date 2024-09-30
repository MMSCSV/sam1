using System.Data.Linq;
using System.Data.Linq.Mapping;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class StorageSpaceItemEntity : IContractConvertible<StorageSpaceItem>
    {
        private EntityRef<StorageSpaceInventoryEntity> _storageSpaceInventoryEntity;

        partial void OnCreated()
        {
            _storageSpaceInventoryEntity = default(EntityRef<StorageSpaceInventoryEntity>);
        }

        #region Public Properties


        [Association(Name = "StorageSpaceItemEntity_StorageSpaceInventoryEntity", Storage = "_storageSpaceInventoryEntity", ThisKey = "Key", OtherKey = "StorageSpaceItemKey")]
        public StorageSpaceInventoryEntity StorageSpaceInventoryEntity
        {
            get
            {
                return _storageSpaceInventoryEntity.Entity;
            }
            set
            {
                StorageSpaceInventoryEntity previousValue = _storageSpaceInventoryEntity.Entity;
                if (previousValue != value ||
                    _storageSpaceInventoryEntity.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();

                    _storageSpaceInventoryEntity.Entity = value;

                    SendPropertyChanged("StorageSpaceInventoryEntity");
                }
            }
        }

        #endregion

        #region IContractConvertible<StorageSpaceItem> Members

        public StorageSpaceItem ToContract()
        {
            // Get the storage space Inventory reference
            StorageSpaceInventory storageSpaceInventory = (StorageSpaceInventoryEntity != null)
                ? StorageSpaceInventoryEntity.ToContract()
                : Key;

            return new StorageSpaceItem(Key, StorageSpaceItemSnapshotKey)
                       {
                           StorageSpaceKey = StorageSpaceKey,
                           ItemKey = ItemKey,
                           LastDispenseDateTime = LastDispenseLocalDateTime,
                           LastDispenseUtcDateTime = LastDispenseUTCDateTime,
                           LastInventoryDateTime = LastInventoryLocalDateTime,
                           LastInventoryUtcDateTime = LastInventoryUTCDateTime,
                           LastLoadRefillDateTime = LastLoadRefillLocalDateTime,
                           LastLoadRefillUtcDateTime = LastLoadRefillUTCDateTime,
                           PendedAtServer = PendedAtServerFlag,
                           StorageSpaceItemStatus =
                               StorageSpaceItemStatusInternalCode.FromInternalCode<StorageSpaceItemStatusInternalCode>(),
                           ParQuantity = ParQuantity,
                           RefillPointQuantity = RefillPointQuantity,
                           CriticalLowQuantity = CriticalLowQuantity,
                           PhysicalMaximumQuantity = PhysicalMaximumQuantity,
                           IsStandardStockWithinDispensingDevice = StandardStockWithinDispensingDeviceFlag,
                           IssueUnitOfMeasureKey = IssueUOMKey,
                           OutdateTracking = OutdateTrackingFlag,
                           DispenseFraction = DispenseFractionFlag,
                           SystemBusDeviceKey = SystemBusDeviceKey,
                           FromExternalSystem = FromExternalSystemFlag,
                           InventoryQuantity = storageSpaceInventory.InventoryQuantity,
                           EarliestNextExpirationDate = storageSpaceInventory.EarliestNextExpirationDate,
                           LastModified = LastModifiedBinaryValue.ToArray(),
                       };
        }

        #endregion
    }
}
