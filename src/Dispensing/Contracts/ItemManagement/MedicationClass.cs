using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a category that indicates the degree of restriction regarding the usage of the medications
    /// as determined by legislation.
    /// </summary>
    [Serializable]
    public class MedicationClass : Entity<Guid>
    {
        #region Constructors

        public MedicationClass()
        {

        }

        public MedicationClass(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator MedicationClass(Guid key)
        {
            return FromKey(key);
        }

        public static MedicationClass FromKey(Guid key)
        {
            return new MedicationClass(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of an external system.
        /// </summary>
        /// <remarks>
        /// This property is not updatable.
        /// </remarks>
        public Guid ExternalSystemKey { get; set; }

        /// <summary>
        /// Gets the name of an external system.
        /// </summary>
        public string ExternalSystemName { get; internal set; }

        /// <summary>
        /// Gets or sets the code that identifies a medication class.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.MedicationClassCodeUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_CodeOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_CodeRequired")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a medication class.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.MedicationClassDescriptionUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionOutOfBounds")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a class is for controlled medications.
        /// </summary>
        public bool IsControlled { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a formulary template.
        /// </summary>
        public Guid? FormularyTemplateKey { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to control the sort order.
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// Gets the value that indicates whether a medication class is logically deleted.
        /// </summary>
        public bool IsDeleted { get; internal set; }

        #endregion
    }
}

