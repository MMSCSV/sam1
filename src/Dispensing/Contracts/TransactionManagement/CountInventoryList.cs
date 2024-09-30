using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a list of storage space items that are to have the inventory counted.
    /// </summary>
    public class CountInventoryList : Entity<Guid>
    {
        #region Constructors

        public CountInventoryList()
        {
        }

        public CountInventoryList(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator CountInventoryList(Guid key)
        {
            return FromKey(key);
        }

        public static CountInventoryList FromKey(Guid key)
        {
            return new CountInventoryList(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a dispensing device.
        /// </summary>
        public Guid DispensingDeviceKey { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a user account.
        /// </summary>
        public Guid UserAccountKey { get; set; }

        /// <summary>
        /// Gets or sets the entries associated with a count inventory list.
        /// </summary>
        public CountInventoryListEntry[] Entries { get; set; }

        /// <summary>
        /// Gets or sets the time and date when a count inventory list is discarded by the user.
        /// </summary>
        public DateTime? FinishUTCDateTime { get; set; }

        /// <summary>
        /// Gets or sets the FinishOrComplete flag
        /// </summary>
        public bool FinishOrCompleteFlag { get; set; }
        
        #endregion
    }
}
