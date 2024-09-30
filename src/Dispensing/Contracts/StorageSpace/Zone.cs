using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a record of a zone for a given period of time.
    /// </summary>
    [Serializable]
    public class Zone : Entity<Guid>
    {
        #region Constructors

        public Zone()
        {
        }

        public Zone(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator Zone(Guid key)
        {
            return FromKey(key);
        }

        public static Zone FromKey(Guid key)
        {
            return new Zone(key);
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
        /// Gets or sets the name of a zone.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets ort sets the number that identifies the zone.
        /// </summary>
        public short? Number { get; set; }

        #endregion
    }
}
