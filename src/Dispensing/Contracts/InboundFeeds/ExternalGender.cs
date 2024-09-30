using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents patients' administrative sex as per an external system.
    /// </summary>
    [Serializable]
    public class ExternalGender : Entity<Guid>
    {
        #region Constructors

        public ExternalGender()
        {

        }

        public ExternalGender(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator ExternalGender(Guid key)
        {
            return FromKey(key);
        }

        public static ExternalGender FromKey(Guid key)
        {
            return new ExternalGender(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of an external system.
        /// </summary>
        public Guid ExternalSystemKey { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies an external gender.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.ExternalGenderCodeUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_CodeOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_CodeRequired")]
        public string GenderCode { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a gender.
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an external gender
        /// is used on outbound messages for a given external system and gender.
        /// </summary>
        public bool UseOnOutbound { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to control the sort order.
        /// </summary>
        public int? SortOrder { get; set; }

        #endregion
    }
}
