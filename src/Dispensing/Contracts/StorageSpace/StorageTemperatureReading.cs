using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    public class StorageTemperatureReading : Entity<Guid>
    {
        #region Constructors

        public StorageTemperatureReading()
        {
        }

        public StorageTemperatureReading(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator StorageTemperatureReading(Guid key)
        {
            return FromKey(key);
        }

        public static StorageTemperatureReading FromKey(Guid key)
        {
            return new StorageTemperatureReading(key);
        }

        #endregion

        #region Public Properties

        public Guid StorageSpaceKey { get; set; }

        public UnitOfTemperatureInternalCode ProvidedUnitOfTemperatureReadingInternalCode { get; set; }

        public double FahrenheitAmount { get; set; }

        public double CelsiusAmount { get; set; }

        public Guid? OutOfTemperatureRangeEventKey { get; set; }

        public DateTime ReadingUTCDateTime { get; set; }

        public DateTime ReadingLocalDateTime { get; set; }

        #endregion
    }
}
