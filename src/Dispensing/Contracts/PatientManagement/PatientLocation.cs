using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a patient's assigned, pending, and temporary
    /// locations within the hospital.
    /// </summary>
    public class PatientLocation : Entity<Guid>
    {
        #region Constructors

        public PatientLocation()
        {
        }

        public PatientLocation(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PatientLocation(Guid key)
        {
            return FromKey(key);
        }

        public static PatientLocation FromKey(Guid key)
        {
            return new PatientLocation(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the facility that a patient is assigned to.
        /// </summary>
        /// <value>The assigned facility.</value>
        public Guid AssignedFacilityKey { get; set; }

        /// <summary>
        /// Gets or sets the unit a patient is assigned to.
        /// </summary>
        /// <value>The assigned unit.</value>
        public Guid? AssignedUnitKey { get; set; }

        /// <summary>
        /// Gets or sets the unit room a patient is assigned to.
        /// </summary>
        /// <value>The assigned unit room.</value>
        public Guid? AssignedUnitRoomKey { get; set; }

        /// <summary>
        /// Gets or sets the ID of a bed that a patient is assigned to. 
        /// </summary>
        /// <value>The assigned bed id.</value>
        public string AssignedBedId { get; set; }

        /// <summary>
        /// Gets or sets the unit a patient is temporarily located
        /// at but not assigned to.
        /// </summary>
        public Guid? TemporaryUnitKey { get; set; }

        /// <summary>
        /// Gets or sets the unit room a patient is temporarily located
        /// at but not assigned to.
        /// </summary>
        public Guid? TemporaryUnitRoomKey { get; set; }

        /// <summary>
        /// Gets or sets the ID of a bed that a patient is temporarily located
        /// at but not assigned to.
        /// </summary>
        public string TemporaryBedId { get; set; }

        #endregion
    }
}
