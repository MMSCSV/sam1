using System;
using System.Collections.Generic;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a collection of items that, along with a quantity thereof each item, are equivalent
    /// to another item.
    /// </summary>
    [Serializable]
    public class ItemEquivalencySet: EntitySet<Guid, ItemEquivalency>
    {
        #region Constructors

        public ItemEquivalencySet()
        {
        }

        public ItemEquivalencySet(Guid key)
            : base(key)
        {
            
        }

        public ItemEquivalencySet(Guid key, IEnumerable<ItemEquivalency> items)
            : base(key, items)
        {
        }

        public ItemEquivalencySet(IEnumerable<ItemEquivalency> items)
            : base(items)
        {
        }

        #endregion

        #region Operator Overloads

        public static implicit operator ItemEquivalencySet(Guid key)
        {
            return FromKey(key);
        }

        public static ItemEquivalencySet FromKey(Guid key)
        {
            return new ItemEquivalencySet(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the value that indicates whether an item equivalency set is approved.
        /// </summary>
        public bool Approved { get; set; }

        public Guid ItemKey { get; set; }

        #endregion
    }
}
