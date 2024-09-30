using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a general allergy category.
    /// </summary>
    [Serializable]
    public class AllergyType : Entity<Guid>
    {
        #region Constructors

        public AllergyType()
        {
        }

        public AllergyType(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator AllergyType(Guid key)
        {
            return FromKey(key);
        }

        public static AllergyType FromKey(Guid key)
        {
            return new AllergyType(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of an external system.
        /// </summary>
        public Guid ExternalSystemKey { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies an allergy type.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.AllergyTypeCodeUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_CodeOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_CodeRequired")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the text that describes an allergy type.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.AllergyTypeDescriptionUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionOutOfBounds")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to control the sort order.
        /// </summary>
        public int? SortOrder { get; set; }

        #endregion
    }
}
