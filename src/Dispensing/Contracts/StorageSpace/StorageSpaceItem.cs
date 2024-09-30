using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents an association of storage space and an item such that either the storage
    /// space holds the item or the storage space is due to hold such an item.
    /// </summary>
    [Serializable]
    public class StorageSpaceItem : Entity<Guid>
    {
        #region Constructors

        public StorageSpaceItem()
        {
        }

        public StorageSpaceItem(Guid key)
        {
            Key = key;
        }

        public StorageSpaceItem(Guid key, Guid snapshotKey)
        {
            Key = key;
            SnapshotKey = snapshotKey;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator StorageSpaceItem(Guid key)
        {
            return FromKey(key);
        }

        public static StorageSpaceItem FromKey(Guid key)
        {
            return new StorageSpaceItem(key);
        }

        #endregion

        public Guid SnapshotKey { get; set; }

        /// <summary>
        /// Gets or sets the storage space.
        /// </summary>
        public Guid StorageSpaceKey { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of an item.
        /// </summary>
        public Guid ItemKey { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of an item.
        /// </summary>
        public Guid ItemSnapshotKey { get; set; }

        /// <summary>
        /// Gets or sets flag that indicates whether a storage space was ejected
        /// while having inventory in it at the dispensing device
        /// (TBD whether this flag is set to 1 on eject of loaded CUBIE with inventory count of zero)
        /// </summary>
        public bool DispensingDeviceEjectWithInventory { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time that a given item is last dispensed
        /// from a given storage space.
        /// </summary>
        public DateTime? LastDispenseUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time that a given item is last dispensed
        /// from a given storage space.
        /// </summary>
        public DateTime? LastDispenseDateTime { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time that a given item is last inventoried
        /// from a given storage space.
        /// </summary>
        public DateTime? LastInventoryUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time that a given item is last inventoried
        /// from a given storage space.
        /// </summary>
        public DateTime? LastInventoryDateTime { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time that a given item is last loaded
        /// or refilled from a given storage space.
        /// </summary>
        public DateTime? LastLoadRefillUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time that a given item is last loaded
        /// or refilled from a given storage space.
        /// </summary>
        public DateTime? LastLoadRefillDateTime { get; set; }

        /// <summary>
        /// Gets or sets the last loaded
        /// or refilled successful scan from a given storage space.
        /// </summary>
        public bool LastLoadRefillSuccessfulScan { get; set; }

        /// <summary>
        /// Gets or sets the last load/refill user.
        /// </summary>
        public Guid? LastLoadRefillActorKey { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time that a given item is last RxChecked
        /// from a given storage space.
        /// </summary>
        public DateTime? LastRxCheckUTCDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time that a given item is last RxChecked
        /// from a given storage space.
        /// </summary>
        public DateTime? LastRxCheckDateTime { get; set; }

        /// <summary>
        /// Gets or sets UTC date and time that a given item is loaded in a given storage space
        /// </summary>
        public DateTime? LoadUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets local date and time that a given item is loaded in a given storage space
        /// </summary>
        public DateTime? LoadDateTime { get; set; }

        /// <summary>
        /// Gets or sets UTC date and time that a CUBIE's inventory is known because
        /// some action took place at dispensing device or mobile dock
        /// </summary>
        public DateTime? LastKnownUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets local date and time that a CUBIE's inventory is known because
        /// some action took place at dispensing device or mobile dock
        /// </summary>
        public DateTime? LastKnownDateTime { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies a child storage space item status.
        /// </summary>
        public StorageSpaceItemStatusInternalCode StorageSpaceItemStatus { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an association of an item and astorage
        /// space is originally created at the server rather than at the dispensing device.
        /// </summary>
        public bool PendedAtServer { get; set; }

        /// <summary>
        /// Gets or sets the optimum quantity of an item that a storage space should contain.
        /// </summary>
        public decimal? ParQuantity { get; set; }

        /// <summary>
        /// Gets or sets the quantity at or below which a storage space should be refilled during a normal
        /// refill cycle.
        /// </summary>
        public decimal? RefillPointQuantity { get; set; }

        /// <summary>
        /// Gets or sets the quantity at which the current quantity should never fall below.
        /// </summary>
        public decimal? CriticalLowQuantity { get; set; }

        /// <summary>
        /// Gets or sets the quantity that is the maximum that a storage space can contain.
        /// </summary>
        public decimal? PhysicalMaximumQuantity { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item should normally be kept
        /// within a dispensing device.
        /// </summary>
        public bool IsStandardStockWithinDispensingDevice { get; set; }

        /// <summary>
        /// Gets or sets the unit of measure that is used on issue (dispense) of an item.
        /// </summary>
        public Guid? IssueUnitOfMeasureKey { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether we track (on both dispense and load/refill)
        /// the earliest expiration date for the inventory in a storage space.
        /// </summary>
        public bool OutdateTracking { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a user may dispense a portion of an item's
        /// dosage form.
        /// </summary>
        public bool DispenseFraction { get; set; }

        /// <summary>
        /// Gets or sets the system bus device key.
        /// </summary>
        public Guid? SystemBusDeviceKey { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates that the record was modified by an external system.
        /// </summary>
        public bool FromExternalSystem { get; set; }

        /// <summary>
        /// Gets or sets the quantity of anitem that is contained in a storage space per the
        /// (facility) item-level unit of issue.
        /// </summary>
        public decimal? InventoryQuantity { get; set; }

        /// <summary>
        /// Gets or sets the earliest date at which at least one item in a storage space is due to expire.
        /// </summary>
        public DateTime? EarliestNextExpirationDate { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", StorageSpaceKey, ItemKey);
        }
        
    }
}
