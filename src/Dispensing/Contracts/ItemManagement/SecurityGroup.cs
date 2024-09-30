using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a category that indicates the degree of restriction regarding the usage of items.
    /// </summary>
    [Serializable]
    public class SecurityGroup : Entity<Guid>
    {
        #region Constructors

        public SecurityGroup()
        {
            // NOTE: Default is MED
            BusinessDomain = BusinessDomainInternalCode.MED;
        }

        public SecurityGroup(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator SecurityGroup(Guid key)
        {
            return FromKey(key);
        }

        public static SecurityGroup FromKey(Guid key)
        {
            return new SecurityGroup(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the internal code that identifies a business domain.
        /// </summary>
        /// <remarks>
        /// This property is not updatable.
        /// </remarks>
        public BusinessDomainInternalCode BusinessDomain { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies a security group.
        /// </summary>
        /// <remarks>
        /// Required and must be unique within the system.
        /// </remarks>
        public string DisplayCode { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a security group.
        /// </summary>
        /// <remarks>
        /// Required.
        /// </remarks>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to control the sort order.
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a security group
        /// is active.
        /// </summary>
        public bool IsActive { get; set; }

        #endregion
    }
}
