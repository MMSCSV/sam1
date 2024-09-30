using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    public class TemperatureMonitoredStorageSpace : Entity<Guid>
    {
        #region Constructors

        public TemperatureMonitoredStorageSpace()
        {
        }

        public TemperatureMonitoredStorageSpace(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator TemperatureMonitoredStorageSpace(Guid key)
        {
            return FromKey(key);
        }

        public static TemperatureMonitoredStorageSpace FromKey(Guid key)
        {
            return new TemperatureMonitoredStorageSpace(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a storage space.
        /// </summary>
        public Guid StorageSpaceKey { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a unit of temperature.
        /// </summary>
        public UnitOfTemperatureInternalCode UnitOfTemperatureInternalCode { get; set; }

        /// <summary>
        /// Gets or sets the lower temperature value.
        /// </summary>
        public short TemperatureRangeLowerAmount { get; set; }

        /// <summary>
        /// Gets or sets the upper temperature value.
        /// </summary>
        public short TemperatureRangeUpperAmount { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) between the SRM providing readings.
        /// </summary>
        public short ReportingIntervalAmount { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) for the warning delay.
        /// </summary>
        public short WarningDelayDurationAmount { get; set; }

        public short? TemperatureSetPointAmount { get; set; }

        public bool DisableDoorUnlockAlarmFlag { get; set; }
        #endregion
    }
}
