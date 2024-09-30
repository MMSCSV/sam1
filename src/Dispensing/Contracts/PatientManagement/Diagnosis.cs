using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents the determination of the nature and cause of a disease
    /// or injury for a given encounter for a given period of time.
    /// </summary>
    [Serializable]
    public class Diagnosis : Entity<Guid>
    {
        #region Constructors

        public Diagnosis()
        {
        }

        public Diagnosis(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator Diagnosis(Guid key)
        {
            return FromKey(key);
        }

        public static Diagnosis FromKey(Guid key)
        {
            return new Diagnosis(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the externally provided code that identifies a diagnosis.
        /// </summary>
        /// <value>The external code.</value>
        public string DiagnosisCode { get; set; }

        /// <summary>
        /// Gets or sets the ID of a coding system for diagnosis external codes.
        /// </summary>
        /// <value>The coding system id.</value>
        public string CodingSystemId { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        #endregion
    }
}
