using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a category of dosage form such that each dosage form in the category is clinically equivalent.
    /// </summary>
    [Serializable]
    public class EquivalencyDosageFormGroup : Entity<Guid>
    {
        #region Constructors

        public EquivalencyDosageFormGroup()
        {

        }

        public EquivalencyDosageFormGroup(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator EquivalencyDosageFormGroup(Guid key)
        {
            return FromKey(key);
        }

        public static EquivalencyDosageFormGroup FromKey(Guid key)
        {
            return new EquivalencyDosageFormGroup(key);
        }

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Gets or sets the localizable code that identifies an equivalency dosage form group.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.EquivalencyDosageFormGroupDisplayCodeUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DisplayCodeOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DisplayCodeRequired")]
        public string DisplayCode { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies an equivalency dosage form group.
        /// </summary>
        public EquivalencyDosageFormGroupInternalCode? InternalCode { get; set; }

        /// <summary>
        /// Gets or sets the text that describes an equivalency dosage form group.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.EquivalencyDosageFormGroupDescriptionUpperBound,
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
