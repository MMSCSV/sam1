using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a general severity of an allergy.
    /// </summary>
    public class AllergySeverity : Entity<Guid>
    {
        #region Constructors

        public AllergySeverity()
        {
        }

        public AllergySeverity(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator AllergySeverity(Guid key)
        {
            return FromKey(key);
        }

        public static AllergySeverity FromKey(Guid key)
        {
            return new AllergySeverity(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of an external system.
        /// </summary>
        public Guid ExternalSystemKey { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies an allergy type.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the text that describes an allergy type.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to control the sort order.
        /// </summary>
        public int? SortOrder { get; set; }

        #endregion
    }
}
