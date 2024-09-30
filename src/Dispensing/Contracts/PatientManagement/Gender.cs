using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents patients' administrative sex.
    /// </summary>
    /// <example>
    /// F - Female, M - Male, O - Other, U - Unknown, A - Ambiguous,
    /// N - Not Applicable
    /// </example>
    [Serializable]
    public class Gender : Entity<Guid>
    {
        #region Constructors

        public Gender()
        {

        }

        public Gender(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator Gender(Guid key)
        {
            return FromKey(key);
        }

        public static Gender FromKey(Guid key)
        {
            return new Gender(key);
        }

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Gets or sets the localizable code that identifies a gender.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.GenderDisplayCodeUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DisplayCodeOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DisplayCodeRequired")]
        public string DisplayCode { get; set; }

        /// <summary>
        /// Gets or sets the non-localizable code that identifies a gender.
        /// </summary>
        public string InternalCode { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a gender.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.GenderDescriptionUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionRequired")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a gender is active.
        /// </summary>
        /// <value><c>true</c> if this gender is active; otherwise, <c>false</c>.</value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to sort a gender.
        /// </summary>
        public int? SortOrder { get; set; }
        
        #endregion
    }
}
