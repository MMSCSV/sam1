using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a kind of external patient identification.
    /// </summary>
    [Serializable]
    public class ExternalPatientIdentificationType : Entity<Guid>
    {
        #region Constructors

        public ExternalPatientIdentificationType()
        {
            UseOnOutbound = true;
        }

        public ExternalPatientIdentificationType(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator ExternalPatientIdentificationType(Guid key)
        {
            return FromKey(key);
        }

        public static ExternalPatientIdentificationType FromKey(Guid key)
        {
            return new ExternalPatientIdentificationType(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of an external system.
        /// </summary>
        public Guid ExternalSystemKey { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies an external patient identification type.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.ExternalPatientIdentificationTypeCodeUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_CodeOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_CodeRequired")]
        public string PatientIdTypeCode { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a patient identification type.
        /// </summary>
        public PatientIdentificationType PatientIdType { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an external patient identification type
        /// is used on outbound messages for a given external system and patient identification
        /// type.
        /// </summary>
        public bool UseOnOutbound { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to control the sort order.
        /// </summary>
        public int? SortOrder { get; set; }

        #endregion
    }
}
