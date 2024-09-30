using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents something that identifies a patient for a given patient identification
    /// type and optionally, a given assigning authority.
    /// </summary>
    [Serializable]
    public class PatientIdentification : Entity<Guid>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientIdentification"/> class.
        /// </summary>
        public PatientIdentification()
        {
        }

        public PatientIdentification(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PatientIdentification(Guid key)
        {
            return FromKey(key);
        }

        public static PatientIdentification FromKey(Guid key)
        {
            return new PatientIdentification(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the actual value for the patient ID.
        /// </summary>
        /// <value>The value of the patient id.</value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the string, normally containing a single digit, that is derived from 
        /// the contents of a patient ID and that is used for detecting errors in the ID value.
        /// </summary>
        public string CheckValue { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a patient ID type.
        /// </summary>
        /// <value>The patient id type.</value>
        public Guid IdentificationTypeKey { get; set; }

        /// <summary>
        /// Gets the display code of a patient ID type.
        /// </summary>
        /// <value>The patient id type.</value>
        public string IdentificationTypeDisplayCode { get; internal set; }

        #endregion
    }
}
