using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a record of a pick area for a given period of time.
    /// </summary>
    [Serializable]
    public class PickArea : Entity<Guid>
    {
        #region Constructors

        public PickArea()
        {

        }

        public PickArea(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PickArea(Guid key)
        {
            return FromKey(key);
        }

        public static PickArea FromKey(Guid key)
        {
            return new PickArea(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        public Guid FacilityKey { get; set; }

        /// <summary>
        /// Gets the code of a facility.
        /// </summary>
        public string FacilityCode { get; internal set; }

        /// <summary>
        /// Gets the name of a facility.
        /// </summary>
        public string FacilityName { get; internal set; }

        /// <summary>
        /// Gets or sets the name of the pick area.
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}
