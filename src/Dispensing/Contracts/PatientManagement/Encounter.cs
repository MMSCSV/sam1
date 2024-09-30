using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a visit by a person to a healthcare provider organization
    /// in order to receive medical care.
    /// </summary>
    /// <remarks>
    /// For an admitted patient, the encounter starts when the patient is admitted
    /// and ends when the patient is discharged.
    /// 
    /// An encounter may be a pre-admission record of a visit that never materializes
    /// as an actual visit.
    /// </remarks>
    public class Encounter : Entity<Guid>
    {
        #region Contructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Encounter"/> class.
        /// </summary>
        public Encounter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Encounter"/> class.
        /// </summary>
        /// <param name="key">The encounter key.</param>
        public Encounter(Guid key)
        {
            Key = key;
        }

        public Encounter(Guid key, Guid snapshotKey)
        {
            Key = key;
            SnapshotKey = snapshotKey;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator Encounter(Guid key)
        {
            return FromKey(key);
        }

        public static Encounter FromKey(Guid key)
        {
            return new Encounter(key);
        }

        #endregion

        #region Contract Properties

        public DateTime? LastItemTransactionUtcDateTime { get; set; }

        public DateTime? LastItemTransactionDateTime { get; set; }

        public DateTime? CreatedUtcDateTime { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the encounter's snapshot key.
        /// </summary>
        public Guid SnapshotKey { get; set; }

        /// <summary>
        /// Gets the value that indicates whether an encounter is a surviving encounter or has been
        /// merged with another encounter such that the latter is the survivor.
        /// </summary>
        public bool Surviving { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies an encounter admission status.
        /// </summary>
        public EncounterAdmissionStatusInternalCode AdmissionStatus { get; set; }

        /// <summary>
        /// Gets or sets the patient key.
        /// </summary>
        /// <value>The patient key.</value>
        public Guid PatientKey { get; set; }

        /// <summary>
        /// Gets or sets the ID of the encounter itself.
        /// </summary>
        /// <value>The ID.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the string, notmally containing a single digit, that is derived
        /// from the contents of an encounter ID and that is used for detecting errors in
        /// the ID value.
        /// </summary>
        public string IdCheckValue { get; set; }

        /// <summary>
        /// Gets or sets the alternate id of the encounter itself.
        /// </summary>
        /// <value>The alternate id.</value>
        public string AlternateId { get; set; }

        /// <summary>
        /// Gets or sets the string, notmally containing a single digit, that is derived
        /// from the contents of an alternate encounter ID and that is used for detecting 
        /// errors in the ID value.
        /// </summary>
        public string AlternateIdCheckValue { get; set; }

        /// <summary>
        /// Gets or sets the ID that identifies a pre-admission.
        /// </summary>
        /// <value>The pre-admission ID.</value>
        public string PreAdmitId { get; set; }

        /// <summary>
        /// Gets or sets the string, notmally containing a single digit, that is derived
        /// from the contents of an pre-admit ID and that is used for detecting errors in
        /// the ID value.
        /// </summary>
        public string PreAdmitIdCheckValue { get; set; }

        /// <summary>
        /// Gets or sets the ID of a patient account that applies to an encounter.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the string, notmally containing a single digit, that is derived
        /// from the contents of an account ID and that is used for detecting errors in
        /// the ID value.
        /// </summary>
        public string AccountIdCheckValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only a date (and no time) is recorded
        /// for when a patient is admitted.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is expected admit date only; otherwise, <c>false</c>.
        /// </value>
        public bool IsAdmitDateOnly { get; set; }

        /// <summary>
        /// Gets the UTC date and time or just date that a patient is admitted
        /// or an outpatient/emergency patient is registered.
        /// </summary>
        /// <value>The admit UTC date.</value>
        public DateTime? AdmitUtcDate { get; set; }

        /// <summary>
        /// Gets the local date and time or just date that a patient is admitted
        /// or an outpatient/emergency patient is registered.
        /// </summary>
        /// <value>The admit local date.</value>
        /// <remarks>
        /// DateTime.Kind must be set to DateTimeKind.Unspecified.
        /// </remarks>
        public DateTime? AdmitDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only a date (and no time) is recorded
        /// for when a patient is expected to be admitted.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is expected admit date only; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpectedAdmitDateOnly { get; set; }

        /// <summary>
        /// Gets the UTC date and time or just date that a patient is expected
        /// to be admitted.
        /// </summary>
        /// <value>The expected admit UTC date.</value>
        public DateTime? ExpectedAdmitUtcDate { get; set; }

        /// <summary>
        /// Gets the local date and time or just date that a patient is expected
        /// to be admitted.
        /// </summary>
        /// <value>The expected admit local date.</value>
        /// <remarks>
        /// DateTime.Kind must be set to DateTimeKind.Unspecified.
        /// </remarks>
        public DateTime? ExpectedAdmitDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only a date (and no time) is recorded
        /// for when a patient is discharged.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is expected admit date only; otherwise, <c>false</c>.
        /// </value>
        public bool IsDischargeDateOnly { get; set; }

        
        /// <summary>
        /// Gets the UTC date and time or just date that a patient is discharged.
        /// </summary>
        /// <value>The discharge UTC date.</value>
        public DateTime? DischargeUtcDate { get; set; }

        /// <summary>
        /// Gets the local date and time or just date that a patient is discharged.
        /// </summary>
        /// <value>The discharge local date.</value>
        /// <remarks>
        /// DateTime.Kind must be set to DateTimeKind.Unspecified.
        /// </remarks>
        public DateTime? DischargeDate { get; set; }

        /// <summary>
        /// Gets the flag indicates whether an alternative auto-discharge duration applies
        /// </summary>
        public bool AlternateAutoDischarge { get; set; }

        /// <summary>
        /// Gets or sets the encounter patient class key
        /// </summary>
        public Guid? PatientClassKey { get; set; }

        /// <summary>
        /// Gets or sets the hospital service key
        /// </summary>
        public Guid? HospitalServiceKey { get; set; }
        
        /// <summary>
        /// Gets or sets a value that indicates whether an encounter consists of a series of recurring visits.
        /// </summary>
        public bool IsRecurring { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies an encounter type.
        /// </summary>
        public EncounterTypeInternalCode EncounterType { get; set; }

        /// <summary>
        /// Gets or sets the encounters height amount
        /// </summary>F
        public decimal? HeightAmount { get; set; }

        /// <summary>
        /// Gets or sets the encounters height unit of measure
        /// </summary>
        public Guid? HeightUnitOfMeasureKey { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies the observation method for the encounters height
        /// </summary>
        public ObservationMethodInternalCode? HeightObservationMethodInternalCode { get; set; }

        /// <summary>
        /// Gets or sets the encounters weight amount
        /// </summary>
        public decimal? WeightAmount { get; set; }

        /// <summary>
        /// Gets or sets the UTC date the weight was recorded for the encounters
        /// </summary>
        public DateTime? WeightUtcDate { get; set; }

        /// <summary>
        /// Gets or sets UTC date the weight was recorded for the encounters
        /// </summary>
        public DateTime? WeightDate { get; set; }

        /// <summary>
        /// Gets or sets the encounters weight unit of measure
        /// </summary>
        public Guid? WeightUnitOfMeasureKey { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies the observation method for the encounters weight
        /// </summary>
        public ObservationMethodInternalCode? WeightObservationMethodInternalCode { get; set; }

        public DateTime? LeaveOfAbsenceEffectiveUtcDate { get; set; }

        public DateTime? LeaveOfAbsenceEffectiveDate { get; set; }

        public DateTime? LeaveOfAbsenceReturnUtcDate { get; set; }

        public DateTime? LeaveOfAbsenceReturnDate { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a dispensing device where encounter was created.
        /// </summary>
        public Guid? CreatedAtDispensingDeviceKey { get; internal set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>The comment.</value>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the patient location.
        /// </summary>
        /// <value>The patient location.</value>
        public PatientLocation Location { get; set; }

        /// <summary>
        /// Gets or sets the attending physician role set associated with this encounter.
        /// </summary>
        /// <value>The physician role sets.</value>
        public PhysicianRoleSet AttendingPhysicianRoleSet { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time or just date that a patient's encounter is cancelled
        /// </summary>
        public DateTime? CancelledUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time or just date that a patient's encounter is cancelled
        /// </summary>
        public DateTime? CancelledDateTime { get; set; }

        #endregion
    }
}
