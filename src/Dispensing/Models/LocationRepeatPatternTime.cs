using System;
using System.ComponentModel.DataAnnotations.Schema;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Models
{
    /// <summary>
    /// Represents an explicit time of day that a task is to be performed for a
    /// given location repeat pattern.
    /// </summary>
    [Serializable]
    public class LocationRepeatPatternTime : IEntity<Guid>
    {
        #region Constructors

        public LocationRepeatPatternTime()
        {
        }

        public LocationRepeatPatternTime(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator LocationRepeatPatternTime(Guid key)
        {
            return FromKey(key);
        }

        public static LocationRepeatPatternTime FromKey(Guid key)
        {
            return new LocationRepeatPatternTime(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a location repeat pattern time.
        /// </summary>
        [Column("LocationRepeatPatternTimeKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a location repeat pattern.
        /// </summary>
        public Guid LocationRepeatPatternKey { get; set; }

        /// <summary>
        /// Gets or sets the time of day in minutes that an administration should occur.
        /// </summary>
        [Column("ExplicitTimeOfDayValue")]
        public short ExplicitTimeOfDay { get; set; }

        /// <summary>
        /// Gets or sets the binary value that is unique within a database and that is assigned a new
        /// value on insert and on update.
        /// </summary>
        [Column("LastModifiedBinaryValue")]
        public byte[] LastModified { get; set; }

        #endregion

        #region Public Members

        public bool IsTransient()
        {
            return Key == default(Guid);
        }

        #endregion
    }
}
