using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a collection of dispensing devices that are close together and that effectively
    /// behave as one unit.
    /// </summary>
    [Serializable]
    public class VirtualStockLocation : Entity<Guid>
    {
        #region Constructors

        public VirtualStockLocation()
        {
        }

        public VirtualStockLocation(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator VirtualStockLocation(Guid key)
        {
            return FromKey(key);
        }

        public static VirtualStockLocation FromKey(Guid key)
        {
            return new VirtualStockLocation(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        public Guid FacilityKey { get; set; }

        /// <summary>
        /// Gets the code of the facility.
        /// </summary>
        public string FacilityCode { get; internal set; }

        /// <summary>
        /// Gets the name of the facility.
        /// </summary>
        public string FacilityName { get; internal set; }

        /// <summary>
        /// Gets or sets the name of a virtual stock location.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the value that indicates whether a virtual stock location is logically deleted.
        /// </summary>
        public bool IsDeleted { get; internal set; }

        #endregion
    }
}
