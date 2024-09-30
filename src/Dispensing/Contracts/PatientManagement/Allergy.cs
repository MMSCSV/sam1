using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents an allergy that a given patient has to an allergen.
    /// </summary>
    [Serializable]
    public class Allergy : Entity<Guid>
    {
        #region Constructors

        public Allergy()
        {
        }

        public Allergy(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator Allergy(Guid key)
        {
            return FromKey(key);
        }

        public static Allergy FromKey(Guid key)
        {
            return new Allergy(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the integer that identifies a member within an ordered set.
        /// </summary>
        public int MemberNumber { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of an allergy type.
        /// </summary>
        public Guid? AllergyTypeKey { get; set; }

        /// <summary>
        /// Gets or sets the ID that uniquely identifies a single allergy for a patient.
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies an allergen.
        /// </summary>
        public string AllergenCode { get; set; }

        /// <summary>
        /// Gets or sets the text that describes an allergen.
        /// </summary>
        public string AllergenDescription { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an allergen description is truncated.
        /// </summary>
        public bool AllergenDescriptionTruncated { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of an allergy severity.
        /// </summary>
        public Guid? AllergySeverityKey { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies an allergy sensitivity.
        /// </summary>
        public string SensitivityCode { get; set; }

        /// <summary>
        /// Gets or sets the text that describes an allergy sensitivity.
        /// </summary>
        public string SensitivityDescription { get; set; }

        /// <summary>
        /// Gets or sets the allergy reactions associated with an allergy.
        /// </summary>
        public AllergyReaction[] Reactions { get; set; }

        #endregion
    }
}
