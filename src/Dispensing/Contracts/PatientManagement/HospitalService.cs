using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a treatment or type of surgery that a patient is scheduled to receive.
    /// </summary>
    [Serializable]
    public class HospitalService : Entity<Guid>
    {
        #region Constructors

        public HospitalService()
        {

        }

        public HospitalService(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator HospitalService(Guid key)
        {
            return FromKey(key);
        }

        public static HospitalService FromKey(Guid key)
        {
            return new HospitalService(key);
        }

        #endregion
        
        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of an external system.
        /// </summary>
        public Guid ExternalSystemKey { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies a hospital service.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a hospital service.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to sort a hospital service.
        /// </summary>
        public int? SortOrder { get; set; }
        
        #endregion
    }
}
