using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents the assoication of a count inventory list and a count inventory entry. 
    /// </summary>
    public class CountInventoryListEntry : Entity<Guid>
    {
        #region Constructors

        public CountInventoryListEntry()
        {
        }

        public CountInventoryListEntry(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator CountInventoryListEntry(Guid key)
        {
            return FromKey(key);
        }

        public static CountInventoryListEntry FromKey(Guid key)
        {
            return new CountInventoryListEntry(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a storage space item.
        /// </summary>
        public Guid StorageSpaceItemKey { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether list entry is the one at which a suspend occurs.
        /// </summary>
        public bool Suspend { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of an item transaction.
        /// </summary>
        public Guid? ItemTransactionKey { get; set; }

        #endregion
    }
}
