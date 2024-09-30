using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a category that a clinical data subject may belong to.
    /// </summary>
    [Serializable]
    public class ClinicalDataCategory : Entity<Guid>
    {
        #region Constructors

        public ClinicalDataCategory()
        {

        }

        public ClinicalDataCategory(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator ClinicalDataCategory(Guid key)
        {
            return FromKey(key);
        }

        public static ClinicalDataCategory FromKey(Guid key)
        {
            return new ClinicalDataCategory(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the text that describes a clinical data category.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.ClinicalDataCategoryDescriptionUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionRequired")]
        public string Description { get; set; }

        #endregion
    }
}
