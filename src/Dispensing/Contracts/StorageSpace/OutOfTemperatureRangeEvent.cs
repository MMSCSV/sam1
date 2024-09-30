using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents an event in which the temperature-controlled storage space is too high or too low.
    /// </summary>
    [Serializable]
    public class OutOfTemperatureRangeEvent : Entity<Guid>
    {
        #region Constructors

        public OutOfTemperatureRangeEvent()
        {
        }

        public OutOfTemperatureRangeEvent(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator OutOfTemperatureRangeEvent(Guid key)
        {
            return FromKey(key);
        }

        public static OutOfTemperatureRangeEvent FromKey(Guid key)
        {
            return new OutOfTemperatureRangeEvent(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a storage space.
        /// </summary>
        public Guid StorageSpaceKey { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time that an out-of-temperature-range event started.
        /// </summary>
        public DateTime EventStartUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time that an out-of-temperature-range event started.
        /// </summary>
        public DateTime EventStartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time that an out-of-temperature-range event ended.
        /// </summary>
        public DateTime? EventEndUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time that an out-of-temperature-range event ended.
        /// </summary>
        public DateTime? EventEndDateTime { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a temperature monitored storage space that applies
        /// at the start of an out-of-range event.
        /// </summary>
        public Guid StartTemperatureMonitoredStorageSpaceKey { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a temperature monitored storage space that applies
        /// at the end of an out-of-range event.
        /// </summary>
        public Guid? EndTemperatureMonitoredStorageSpaceKey { get; set; }

        #endregion
    }
}
