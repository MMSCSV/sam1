using System;
using CareFusion.Dispensing.Models;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a pharmacy order.
    /// </summary>
    [Serializable]
    public class PharmacyOrderComponent : Entity<Guid>
    {
        #region Constructors

        public PharmacyOrderComponent()
        {
        }

        public PharmacyOrderComponent(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderComponent(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderComponent FromKey(Guid key)
        {
            return new PharmacyOrderComponent(key);
        }

        #endregion

        #region Public Properties

        public int MemberNumber { get; set; }

        public PharmacyOrderComponentTypeInternalCode ComponentType { get; set; }

        public Guid? MedItemKey { get; set; }

        public string ComponentId { get; set; }

        public string Description { get; set; }

        public decimal? Amount { get; set; }

        public UnitOfMeasure AmountUnitOfMeasure { get; set; }

        public Guid? AmountExternalUnitOfMeasureKey { get; set; }

        public int NetRemoveOccurrenceQuantity { get; set; }

        public DateTime? LinkedUtcDateTime { get; set; }

        public DateTime? LinkedDateTime { get; set; }

        #endregion
    }
}
