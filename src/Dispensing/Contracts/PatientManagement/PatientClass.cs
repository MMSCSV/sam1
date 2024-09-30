using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents the cateogry of patient on any encounter.
    /// </summary>
    /// <example>
    /// E - Emergency
    /// I - Inpatient
    /// O - Outpatient
    /// P - Preadmit
    /// R - Recurring patient
    /// B - Obstetrics
    /// C - Commercial account
    /// N - Not applicable
    /// U - Unknown
    /// </example>
    [Serializable]
    public class PatientClass : Entity<Guid>
    {
        #region Constructors

        public PatientClass()
        {

        }

        public PatientClass(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PatientClass(Guid key)
        {
            return FromKey(key);
        }

        public static PatientClass FromKey(Guid key)
        {
            return new PatientClass(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of an external system.
        /// </summary>
        public Guid ExternalSystemKey { get; set; }

        /// <summary>
        /// Gets or sets the non-localizable code that identifies a patient class.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to sort a patient class.
        /// </summary>
        public int? SortOrder { get; set; }

        #endregion
    }
}
