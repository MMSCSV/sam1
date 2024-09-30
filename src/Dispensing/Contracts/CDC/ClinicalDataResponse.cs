using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a reply or reaction that may be given to a clinical data notice.
    /// </summary>
    [Serializable]
    public class ClinicalDataResponse : Entity<Guid>
    {
         #region Constructors

        public ClinicalDataResponse()
        {

        }

        public ClinicalDataResponse(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator ClinicalDataResponse(Guid key)
        {
            return FromKey(key);
        }

        public static ClinicalDataResponse FromKey(Guid key)
        {
            return new ClinicalDataResponse(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the internal code that identifies a clinical data assent.
        /// </summary>
        public ClinicalDataAssentInternalCode? Assent { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a valid standard response to a clinical data
        /// subject.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Gets or sets the text that describes what a caregiver is to do if they choose a particular
        /// response.
        /// </summary>
        public string Instruction { get; set; }

        #endregion
    }
}
