using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a category that indicated the kind of therapy that a medication
    /// can be used for.
    /// </summary>
    [Serializable]
    public class TherapeuticClass : Entity<Guid>
    {
        #region Constructors

        public TherapeuticClass()
        {

        }

        public TherapeuticClass(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator TherapeuticClass(Guid key)
        {
            return FromKey(key);
        }

        public static TherapeuticClass FromKey(Guid key)
        {
            return new TherapeuticClass(key);
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
        /// Gets or sets the code that identifies a therapeutic class.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.TherapeuticClassCodeUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_CodeOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_CodeRequired")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a therapeutic class.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.TherapeuticClassDescriptionUpperBound,
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
