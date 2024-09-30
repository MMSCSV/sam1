using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a record of an area for a given period of time.
    /// </summary>
    [Serializable]
    public class Area : Entity<Guid>
    {
        #region Constructors

        public Area()
        {
            AllUserRoles = true;
        }

        public Area(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator Area(Guid key)
        {
            return FromKey(key);
        }

        public static Area FromKey(Guid key)
        {
            return new Area(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        public Guid FacilityKey { get; set; }

        /// <summary>
        /// Gets or sets the code of a facility.
        /// </summary>
        public string FacilityCode { get; internal set; }

        /// <summary>
        /// Gets the name of a facility.
        /// </summary>
        public string FacilityName { get; internal set; }

        /// <summary>
        /// Gets or sets the name of an area.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of an area.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether all user roles for the facility are effectively
        /// associated to an area.
        /// </summary>
        public bool AllUserRoles { get; set; }

        /// <summary>
        /// Gets the value that indicates whether an area is logically deleted.
        /// </summary>
        public bool IsDeleted { get; internal set; }

        /// <summary>
        /// Gets or sets the associated role keys
        /// </summary>
        public Guid[] AssociatedRoles { get; set; }

        /// <summary>
        /// Gets or sets the clinical data subjects associated with an area.
        /// </summary>
        public Guid[] ClinicalDataSubjects { get; set; }

        #endregion
    }
}
