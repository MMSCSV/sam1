using System;
using CareFusion.Dispensing.Models;

namespace CareFusion.Dispensing.Contracts
{
    public class PharmacyOrderRelativeTime: Entity<Guid>
    {
        #region Constructors

        public PharmacyOrderRelativeTime()
        {
        }

        public PharmacyOrderRelativeTime(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderRelativeTime(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderRelativeTime FromKey(Guid key)
        {
            return new PharmacyOrderRelativeTime(key);
        }

        #endregion

        #region Public Properties

        public decimal TaskIntervalAmount { get; set; }

        public UnitOfMeasure TaskIntervalUnitOfMeasure { get; set; }

        #endregion
    }
}
