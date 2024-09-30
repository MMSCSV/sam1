using System;
using System.ComponentModel.DataAnnotations.Schema;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Models
{
    /// <summary>
    /// Represents the association of a location and a repeat pattern for a given
    /// period of time such that the repeat pattern has institution-specified
    /// times for that period of time.
    /// </summary>
    [Serializable]
    public class LocationRepeatPattern : IEntity<Guid>
    {
        #region Constructors

        public LocationRepeatPattern()
        {
        }

        public LocationRepeatPattern(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator LocationRepeatPattern(Guid key)
        {
            return FromKey(key);
        }

        public static LocationRepeatPattern FromKey(Guid key)
        {
            return new LocationRepeatPattern(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a location repeat pattern time.
        /// </summary>
        [Column("LocationRepeatPatternKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        public Guid FacilityKey { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a repeat pattern.
        /// </summary>
        public Guid RepeatPatternKey { get; set; }

        /// <summary>
        /// Gets or sets the binary value that is unique within a database and that is assigned a new
        /// value on insert and on update.
        /// </summary>
        [Column("LastModifiedBinaryValue")]
        public byte[] LastModified { get; set; }

        /// <summary>
        /// Gets or sets the explicit times associated with a location repeat pattern.
        /// </summary>
        public LocationRepeatPatternTime[] LocationRepeatPatternTimes { get; set; }

        #endregion

        #region Public Members

        public bool IsTransient()
        {
            return Key == default(Guid);
        }

        #endregion
    }
}
