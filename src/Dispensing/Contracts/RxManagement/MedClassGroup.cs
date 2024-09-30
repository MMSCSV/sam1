using System;
using System.Collections.Generic;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents the MedClass Groups
    /// </summary>
    [Serializable]
    public class MedClassGroup : Entity<Guid>
    {
        #region Constructors

        public MedClassGroup()
        {

        }

        public MedClassGroup(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator MedClassGroup(Guid key)
        {
            return FromKey(key);
        }

        public static MedClassGroup FromKey(Guid key)
        {
            return new MedClassGroup(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Facility Key
        /// </summary>
        public Guid FacilityKey { get; set; }

        /// <summary>
        /// Gets the name of Facility
        /// </summary>
        public string FacilityName { get; internal set; }

        /// <summary>
        /// Gets or sets the code that identifies a Med Class Group.
        /// </summary>
        public string MedClassGroupCode { get; set; }

        /// <summary>
        /// Gets or sets the text that describes the Med Class Group
        /// </summary>
        public string DescriptionText { get; set; }

        /// <summary>
        /// Gets or sets the Medication Classes associated with the Med Class Group
        /// </summary>
        public IEnumerable<MedicationClass> MedicationClasses { get; set; }

        #endregion
    }
}

