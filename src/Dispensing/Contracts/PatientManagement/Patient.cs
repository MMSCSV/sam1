using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a person awaiting or undergoing medical care and
    /// treament.
    /// </summary>
    public class Patient : Entity<Guid>
    {
        #region Constructors

        public Patient()
        {
        }

        public Patient(Guid key)
        {
            Key = key;
        }

        public Patient(Guid key, Guid snapshotKey)
        {
            Key = key;
            SnapshotKey = snapshotKey;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator Patient(Guid key)
        {
            return FromKey(key);
        }

        public static Patient FromKey(Guid key)
        {
            return new Patient(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the patient's snapshot key.
        /// </summary>
        public Guid SnapshotKey { get; set; }

        /// <summary>
        /// Gets the value that indicates whether a patient is a surviving patient or has
        /// been merged with another patient such that the latter is the survivor.
        /// </summary>
        public bool Surviving { get; set; }

        /// <summary>
        /// Gets the patient's first name.
        /// </summary>
        /// <value>The patient's first name.</value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets the patient's middle name.
        /// </summary>
        /// <value>The patient's middle name.</value>
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets the patient's last name.
        /// </summary>
        /// <value>The patient's last name.</value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets the patient's preferred name.
        /// </summary>
        /// <value>The patient's preferred name.</value>
        public string PreferredName { get; set; }

        /// <summary>
        /// Gets the flag that indicates whether a last name is unknown because the record was created from a PIS order message that did not contain the name
        /// </summary>
        public bool UnknownLastName { get; set; }
        /// <summary>
        /// Gets or sets the patient's prefix.
        /// </summary>
        /// <value>The patient's prefix.</value>
        /// <example>Mr., Dr., Mrs.</example>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the patient's suffix.
        /// </summary>
        /// <value>The patient's suffix.</value>
        public string Suffix { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this patient was deceased.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this patient was deceased; otherwise, <c>false</c>.
        /// </value>
        public bool Deceased { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether authorization to see this patient's
        /// information is restricted.
        /// </summary>
        /// <value><c>true</c> if restricted; otherwise, <c>false</c>.</value>
        public bool RestrictedAccess { get; set; }

        /// <summary>
        /// Gets the flag that indicates whether a patient record has been created from a PIS order message, and not yet touched by a message from an ADT system
        /// </summary>
        public bool Placeholder { get; set; }

        /// <summary>
        /// Gets or sets a date time precision recorded for when a patient is born.
        /// </summary>
        /// <value>The birth date precision.</value>
        public DateTimePrecisionInternalCode? BirthDateTimePrecisionCode { get; set; }

        /// <summary>
        /// Gets the patient's local date and time or just date or just year
        /// and month or just year of when a patient is born.
        /// </summary>
        /// <value>The patient's date of birth.</value>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Gets or sets the patient's gender key.
        /// </summary>
        /// <value>The patient's gender.</value>
        public Guid? GenderKey { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a patient silo.
        /// </summary>
        public Guid? PatientSiloKey { get; set; }

        /// <summary>
        /// Gets or sets the identification set for this patient.
        /// </summary>
        /// <value>The identification set.</value>
        public PatientIdentificationSet IdentificationSet { get; set; }

        /// <summary>
        /// Gets or sets the allergy set for this patient.
        /// </summary>
        public AllergySet AllergySet { get; set; }

        /// <summary>
        /// Gets or sets the dispensing device key where this patient is created
        /// </summary>
        public Guid? CreatedAtDispensingDeviceKey { get; set; }

        #endregion
    }
}
