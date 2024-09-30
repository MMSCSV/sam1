using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a category that indicates the degree of restriction regarding the 
    /// usage of the mdeications on override.
    /// </summary>
    [Serializable]
    public class OverrideGroup : Entity<Guid>
    {
        #region Constructors

        public OverrideGroup()
        {

        }

        public OverrideGroup(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator OverrideGroup(Guid key)
        {
            return FromKey(key);
        }

        public static OverrideGroup FromKey(Guid key)
        {
            return new OverrideGroup(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the localizable code that identifies an override group.
        /// </summary>
        /// <remarks>
        /// Required and must be unique within the system.
        /// </remarks>
        public string DisplayCode { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a override group.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to control the sort order.
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a override group
        /// is active.
        /// </summary>
        public bool IsActive { get; set; }

        #endregion
    }
}
