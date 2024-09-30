using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a physical way that a medication can exist.
    /// </summary>
    [Serializable]
    public class DosageForm : Entity<Guid>
    {
        #region Constructors

        public DosageForm()
        {

        }

        public DosageForm(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator DosageForm(Guid key)
        {
            return FromKey(key);
        }

        public static DosageForm FromKey(Guid key)
        {
            return new DosageForm(key);
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
        /// Gets or sets the code of the dosage form.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.DosageFormCodeUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_CodeOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_CodeRequired")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the text that describes the dosage form of a medication.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.DosageFormDescriptionUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionOutOfBounds")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of an equivalency dosage form group.
        /// </summary>
        public Guid? EquivalencyDosageFormGroupKey { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to control the sort order.
        /// </summary>
        public int? SortOrder { get; set; }

        #endregion
    }
}
