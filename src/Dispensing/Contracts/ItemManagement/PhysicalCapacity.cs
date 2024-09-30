using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a quantity of an item that can be fitted within a kind of storage space within a
    /// given facility.
    /// </summary>
    [Serializable]
    public class PhysicalCapacity : Entity<Guid>
    {
        #region Constructors

        public PhysicalCapacity()
        {
        }

        public PhysicalCapacity(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PhysicalCapacity(Guid key)
        {
            return FromKey(key);
        }

        public static PhysicalCapacity FromKey(Guid key)
        {
            return new PhysicalCapacity(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the storage space size.
        /// </summary>
        public StorageSpaceSize StorageSpaceSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum quantity of an item within a facility that fits within a
        /// kind of storage space.
        /// </summary>
        [Obsolete("Use PhysicalMaximumQuantity instead.")]
        public int? MaximumQuantity { get; set; }

        /// <summary>
        /// Gets or sets the quantity that is the default maximum that a storage space of a given size can contain
        /// </summary>
        public int? PhysicalMaximumQuantity { get; set; }


        /// <summary>
        /// Gets or sets the default optimum quantity of an item that a storage space of a given size should contain
        /// </summary>
        public int? ParQuantity { get; set; }

        /// <summary>
        /// Gets or sets the default quantity at or below which a storage space of a given size should be refilled
        /// during a normal refill cycle
        /// </summary>
        public int? RefillPointQuantity { get; set; }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            return Equals(obj as PhysicalCapacity);
        }

        private bool Equals(PhysicalCapacity other)
        {
            var storageSpacesAreEqual = Equals(StorageSpaceSize, other.StorageSpaceSize);
            var maxQuantitiesAreEqual = other.MaximumQuantity.Equals(MaximumQuantity) ||
                                        (!MaximumQuantity.HasValue && !other.MaximumQuantity.HasValue);
            var physicalMaxQuantitiesAreEqual = other.PhysicalMaximumQuantity.Equals(PhysicalMaximumQuantity) ||
                                                (!PhysicalMaximumQuantity.HasValue &&
                                                 !other.PhysicalMaximumQuantity.HasValue);
            return storageSpacesAreEqual && maxQuantitiesAreEqual && physicalMaxQuantitiesAreEqual;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var code = StorageSpaceSize.InternalCode.ToInternalCode().GetHashCode() + (MaximumQuantity ?? 0);
                return code;
            }
        }
    }
}
