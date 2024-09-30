using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents an event in which the temperature-controlled storage space is too high or too low.
    /// </summary>
    [Serializable]
    public class OutOfTemperatureRangeEventResolution : Entity<Guid>
    {
        #region Constructors

        public OutOfTemperatureRangeEventResolution()
        {
        }

        public OutOfTemperatureRangeEventResolution(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator OutOfTemperatureRangeEventResolution(Guid key)
        {
            return FromKey(key);
        }

        public static OutOfTemperatureRangeEventResolution FromKey(Guid key)
        {
            return new OutOfTemperatureRangeEventResolution(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the out-of-range event surrogate key.
        /// </summary>
        public Guid OutOfTemperatureRangeEventKey { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time when an out-of-range event is resolved.
        /// </summary>
        public DateTime? ResolvedUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time when an out-of-range event is resolved.
        /// </summary>
        public DateTime? ResolvedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an event is not resolvable.
        /// </summary>
        public bool NotResolvable { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of an out-of-temperature-range resolution.
        /// </summary>
        public Guid? OutOfTemperatureRangeResolutionKey { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a user account that resolved an out-of-range event.
        /// </summary>
        public Guid? UserAccountKey { get; set; }

        #endregion
    }
}
