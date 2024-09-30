using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents external unit of measurement of a physical quantity.
    /// </summary>
    [Serializable]
    public class ExternalUnitOfMeasure : Entity<Guid>
    {
        #region Constructors

        public ExternalUnitOfMeasure()
        {

        }

        public ExternalUnitOfMeasure(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator ExternalUnitOfMeasure(Guid key)
        {
            return FromKey(key);
        }

        public static ExternalUnitOfMeasure FromKey(Guid key)
        {
            return new ExternalUnitOfMeasure(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of an external system.
        /// </summary>
        public Guid ExternalSystemKey { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a unit of measure role.
        /// </summary>
        public UOMRoleInternalCode UnitOfMeasureRole { get; set; }

        /// <summary>
        /// Gets the description of a unit of measure role.
        /// </summary>
        public string UnitOfMeasureRoleDescription { get; internal set; }

        /// <summary>
        /// Gets or sets the code that identifies an external unit of measure.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.ExternalUnitOfMeasureCodeUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_CodeOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_CodeRequired")]
        public string UnitOfMeasureCode { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a unit of measure.
        /// </summary>
        public Guid? StandardUnitOfMeasureKey { get; set; }

        /// <summary>
        /// Gets the display code of a unit of measure.
        /// </summary>
        public string StandardUnitOfMeasureDisplayCode { get; internal set; }

        /// <summary>
        /// Gets the use dosage form of a unit of measure.
        /// </summary>
        public bool StandardUnitOfMeasureUseDosageForm { get; internal set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an external gender
        /// is used on outbound messages for a given external system and gender.
        /// </summary>
        public bool UseOnOutbound { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to control the sort order.
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// Gets the value that indicates a logical delete for an external unit of measure.
        /// </summary>
        public bool IsDeleted { get; internal set; }

        #endregion
    }
}
