using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents the member of an item equivalency set.
    /// </summary>
    [Serializable]
    public class ItemEquivalency : Entity<Guid>
    {
        #region Constructors

        public ItemEquivalency()
        {
        }

        public ItemEquivalency(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator ItemEquivalency(Guid key)
        {
            return FromKey(key);
        }

        public static ItemEquivalency FromKey(Guid key)
        {
            return new ItemEquivalency(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of an item that in an equivalent-item quantity thereof is, in conjunction with
        /// any other items in the set, equivalent to another item.
        /// </summary>
        public Guid ItemKey { get; set; }

        public string ItemId { get; set; }

        public string DisplayName { get; set; }

        public string PureGenericName { get; set; }

        public string BrandName { get; set; }

        public string DosageFormCode { get; set; }

        /// <summary>
        /// Gets or sets the quantity of an equivalent item that makes up an item equivalency set.
        /// </summary>
        public short Quantity { get; set; }

        #endregion
    }
}
