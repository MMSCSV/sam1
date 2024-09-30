using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a route by which a medication is given to patients.
    /// </summary>
    [Serializable]
    public class AdministrationRoute: Entity<Guid>
    {
        #region Constructors

        public AdministrationRoute()
        {
        }

        public AdministrationRoute(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator AdministrationRoute(Guid key)
        {
            return FromKey(key);
        }

        public static AdministrationRoute FromKey(Guid key)
        {
            return new AdministrationRoute(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of an external system.
        /// </summary>
        public Guid ExternalSystemKey { get; set; }

        /// <summary>
        /// Gets the name of an external system.
        /// </summary>
        public string ExternalSystemName { get; internal set; }

        /// <summary>
        /// Gets or sets the code that identifies an administration route.
        /// </summary>
        public string DisplayCode { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a administration route.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to control the sort order.
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// Gets the value that indicates whether an administration route is logically deleted.
        /// </summary>
        public bool IsDeleted { get; internal set; }

        #endregion
    }
}
