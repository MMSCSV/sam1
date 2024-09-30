using System;
using System.ComponentModel.DataAnnotations.Schema;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a pre-defined reason for RxCheck
    /// </summary>
    public class RxCheckReason : IEntity<Guid>
    {
        #region Constructors

        public RxCheckReason()
        {
        }

        public RxCheckReason(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator RxCheckReason(Guid key)
        {
            return FromKey(key);
        }

        public static RxCheckReason FromKey(Guid key)
        {
            return new RxCheckReason(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of an RxCheck reason.
        /// </summary>
        [Column("RxCheckReasonKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a RxCheck reason.
        /// </summary>
        public RxCheckReasonInternalCode? RxCheckReasonInternalCode { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        public Guid FacilityKey { get; set; }

        /// <summary>
        /// Gets the code of the facility.
        /// </summary>
        public string FacilityCode { get; internal set; }

        /// <summary>
        /// Gets the name of the facility.
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a RxCheck reason.
        /// </summary>
        [Column("DescriptionText")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an RxCheck reason is active.
        /// </summary>
        [Column("ActiveFlag")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the binary value that is unique within a database and that is assigned a new
        /// value on insert and on update.
        /// </summary>
        [Column("LastModifiedBinaryValue")]
        public byte[] LastModified { get; set; }

        #endregion

        #region Public Members

        public bool IsTransient()
        {
            return Key == default(Guid);
        }

        #endregion
    }
}
