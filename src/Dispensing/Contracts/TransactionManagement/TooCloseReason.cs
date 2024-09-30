using System;
using System.ComponentModel.DataAnnotations.Schema;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a pre-defined reason for dispensing an item too close to a prior dispense.
    /// </summary>
    [Serializable]
    public class TooCloseReason: IEntity<Guid>
    {
        #region Constructors

        public TooCloseReason()
        {
        }

        public TooCloseReason(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator TooCloseReason(Guid key)
        {
            return FromKey(key);
        }

        public static TooCloseReason FromKey(Guid key)
        {
            return new TooCloseReason(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a too close reason.
        /// </summary>
        [Column("TooCloseReasonKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies an too-close reason.
        /// </summary>
        public TooCloseReasonInternalCode? InternalCode { get; internal set; }

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        [NotNullValidator]
        public Guid? FacilityKey { get; set; }

        /// <summary>
        /// Gets or sets the Facility name
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// Gets or sets the text that describes an too-close reason.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.TooCloseReasonDescriptionUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionRequired")]
        [Column("DescriptionText")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a too-close reason is active.
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
