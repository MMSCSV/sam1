using System;
using System.ComponentModel.DataAnnotations.Schema;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a pre-defined reason for dispensing an item on override.
    /// </summary>
    [Serializable]
    public class OverrideReason: IEntity<Guid>
    {
        #region Constructors

        public OverrideReason()
        {
        }

        public OverrideReason(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator OverrideReason(Guid key)
        {
            return FromKey(key);
        }

        public static OverrideReason FromKey(Guid key)
        {
            return new OverrideReason(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of an Override reason.
        /// </summary>
        [Column("OverrideReasonKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies an override reason.
        /// </summary>
        public OverrideReasonInternalCode? InternalCode { get; internal set; }

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
        /// Gets or sets the text that describes an override reason.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.OverrideReasonDescriptionUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionRequired")]
        [Column("DescriptionText")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an override reason is active.
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
