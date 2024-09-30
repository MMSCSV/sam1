using System;
using System.ComponentModel.DataAnnotations.Schema;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Models
{
    /// <summary>
    /// Represents the standard unit of measurement of a physical quantity.
    /// </summary>
    /// <example>
    /// mL - milliliters, mL/hr - milliliters/hour, kg - kilograms,
    /// mL/kg/hr - milliliters/kilogram/hour
    /// </example>
    [Serializable]
    public class UnitOfMeasure : IEntity<Guid>
    {
        #region Constructors

        public UnitOfMeasure()
        {

        }

        public UnitOfMeasure(Guid key)
        {
            Key = key;
        }
       
        #endregion

        #region Operator Overloads

        public static implicit operator UnitOfMeasure(Guid key)
        {
            return FromKey(key);
        }

        public static UnitOfMeasure FromKey(Guid key)
        {
            return new UnitOfMeasure(key);
        }

        public static explicit operator Guid?(UnitOfMeasure uom)
        {
            return uom != null ? uom.Key : default(Guid?);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a unit of measure.
        /// </summary>
        [Column("UOMKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the localizable code that identifies a unit of measure.
        /// </summary>
        public string DisplayCode { get; set; }

        /// <summary>
        /// Gets or sets the non-localizable code that identifies a unit of measure.
        /// </summary>
        public string InternalCode { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a unit of measure.
        /// </summary>
        [Column("DescriptionText")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates that a dosage form is shown on particular
        /// UI screens instead of a unit of measure code.
        /// </summary>
        [Column("UseDosageFormFlag")]
        public bool UseDosageForm { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to control the sort order.
        /// </summary>
        [Column("SortValue")]
        public int? SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a unit of measure is active.
        /// </summary>
        /// <value><c>true</c> if this unit of measure is active; otherwise, <c>false</c>.</value>
        [Column("ActiveFlag")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the base unit of measure.
        /// </summary>
        [Column("BaseUOMKey")]
        public Guid? BaseUnitOfMeasureKey { get; set; }

        /// <summary>
        /// Gets or sets the conversion.
        /// </summary>
        [Column("ConversionAmount")]
        public decimal? Conversion { get; set; }

        /// <summary>
        /// Gets or sets the binary value that is unique within a database and that is assigned a new
        /// value on insert and on update.
        /// </summary>
        [Column("LastModifiedBinaryValue")]
        public byte[] LastModified { get; set; }

        /// <summary>
        /// Gets or sets internal code that identifies unit of measure roles.
        /// </summary>
        public UOMRoleInternalCode[] Roles { get; set; }

        #endregion

        #region Public Members

        public bool IsTransient()
        {
            return Key == default(Guid);
        }

        #endregion

        #region Conversion Methods

        public static bool CanConvert(UnitOfMeasure from, UnitOfMeasure to)
        {
            // check nulls
            if (from == null || to == null)
                return false;

            if (
                // if same uom
                (from.Key == to.Key) ||
                // if from is base of to
                (to.BaseUnitOfMeasureKey.HasValue &&
                from.Key == to.BaseUnitOfMeasureKey.Value &&
                to.Conversion.HasValue &&
                to.Conversion.Value > 0) ||
                // if to is base of from
                (from.BaseUnitOfMeasureKey.HasValue &&
                from.BaseUnitOfMeasureKey.Value == to.Key &&
                from.Conversion.HasValue &&
                from.Conversion.Value > 0) ||
                // if from and to have same base
                (from.BaseUnitOfMeasureKey.HasValue &&
                to.BaseUnitOfMeasureKey.HasValue &&
                from.BaseUnitOfMeasureKey.Value == to.BaseUnitOfMeasureKey.Value &&
                from.Conversion.HasValue &&
                from.Conversion.Value > 0 &&
                to.Conversion.HasValue &&
                to.Conversion.Value > 0))
            {
                return true;
            }
            // cannot convert
            return false;
        }

        public static decimal Convert(decimal amount, UnitOfMeasure from, UnitOfMeasure to)
        {
            // check nulls
            if (from == null || to == null)
                throw new ArgumentException("Cannot convert amount given null unit of measure");
            // check convertability
            if (!CanConvert(from, to))
                throw new ArgumentException("Cannot convert amount from " + from.DisplayCode + " to " + to.DisplayCode + " units.");

            if (from.Key == to.Key)
            {
                // if same uom, no conversion
                return amount;
            }
            else if 
                (to.BaseUnitOfMeasureKey.HasValue &&
                from.Key == to.BaseUnitOfMeasureKey.Value &&
                to.Conversion.HasValue &&
                to.Conversion.Value > 0)
            {
                // if from is base of to
                return amount / to.Conversion.Value;
            }
            else if 
                (from.BaseUnitOfMeasureKey.HasValue &&
                from.BaseUnitOfMeasureKey.Value == to.Key &&
                from.Conversion.HasValue &&
                from.Conversion.Value > 0)
            {
                // if to is base of from
                return amount * from.Conversion.Value;
            }
            else if
                (from.BaseUnitOfMeasureKey.HasValue &&
                to.BaseUnitOfMeasureKey.HasValue &&
                from.BaseUnitOfMeasureKey.Value == to.BaseUnitOfMeasureKey.Value &&
                from.Conversion.HasValue &&
                from.Conversion.Value > 0 &&
                to.Conversion.HasValue &&
                to.Conversion.Value > 0)
            {
                // if from and to have same base
                return amount * from.Conversion.Value / to.Conversion.Value;
            }

            throw new ArgumentException("Cannot convert amount from " + from.DisplayCode + " to " + to.DisplayCode + " units.");
        }

        #endregion
    }
}
