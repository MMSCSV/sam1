using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents the association of a facility item equivalency.
    /// </summary>
    [Serializable]
    public class FacilityEquivalency : Entity<Guid>
    {

        #region Constructors

        public FacilityEquivalency()
        {
        }

        public FacilityEquivalency(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator FacilityEquivalency(Guid key)
        {
            return FromKey(key);
        }

        public static FacilityEquivalency FromKey(Guid key)
        {
            return new FacilityEquivalency(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of an ItemEquivalencySet.
        /// </summary>
        public Guid ItemEquivalencySetKey { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a Facility.
        /// </summary>
        public Guid FacilityKey { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an equivalency is qpproved at the facility item level
        /// </summary>
        public bool Approved { get; set; }

        #endregion
    }
}
