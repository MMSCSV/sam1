using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a kind of patient identification.
    /// </summary>
    /// <example>
    /// MRN - Medical Record Number, AN - Account Number
    /// </example>
    [Serializable]
    public class PatientIdentificationType : Entity<Guid>
    {
        #region Constructors

        public PatientIdentificationType()
        {

        }

        public PatientIdentificationType(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PatientIdentificationType(Guid key)
        {
            return FromKey(key);
        }

        public static PatientIdentificationType FromKey(Guid key)
        {
            return new PatientIdentificationType(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the localizable code that identifies a patient identification type.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.PatientIdentificationTypeDisplayCodeUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DisplayCodeOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DisplayCodeRequired")]
        public string DisplayCode { get; set; }

        /// <summary>
        /// Gets or sets the non-localizable code that identifies a patient identification type.
        /// </summary>
        public string InternalCode { get; set; }

        /// <summary>
        /// Gets or sets text that describes a patient identification type.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.PatientIdentificationTypeDescriptionUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionOutOfBounds")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a patient identification type is active.
        /// </summary>
        /// <value><c>true</c> if this patient identification type is active; otherwise, <c>false</c>.</value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to sort a patient identification type.
        /// </summary>
        public int? SortOrder { get; set; }

        #endregion
    }
}
