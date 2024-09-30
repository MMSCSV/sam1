using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a category that indicates the kind of hazard medications pose
    /// as waste.
    /// </summary>
    [Serializable]
    public class HazardousWasteClass : Entity<Guid>
    {
        #region Constructors

        public HazardousWasteClass()
        {

        }

        public HazardousWasteClass(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator HazardousWasteClass(Guid key)
        {
            return FromKey(key);
        }

        public static HazardousWasteClass FromKey(Guid key)
        {
            return new HazardousWasteClass(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        public Guid FacilityKey { get; set; }

        /// <summary>
        /// Gets or sets the localizable code that identifies a hazardous waste class.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.HazardousWasteClassDisplayCodeUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DisplayCodeOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DisplayCodeRequired")]
        public string DisplayCode { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a hazardous waste class.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.HazardousWasteClassDescriptionUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionOutOfBounds")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the text that describes how medications with a given hazardous waste
        /// class is to be disposed.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.HazardousWasteClassDisposalInstructionsUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "HazardousWasteClassDisposalInstructionsOutOfBounds")]
        public string DisposalInstructions { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to control the sort order.
        /// </summary>
        public int? SortOrder { get; set; }

        #endregion
    }
}
