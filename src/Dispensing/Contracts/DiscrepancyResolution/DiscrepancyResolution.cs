using System;
using System.ComponentModel.DataAnnotations.Schema;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a pre-defined reason for resolving a discrepancy
    /// </summary>
    public class DiscrepancyResolution : IEntity<Guid>
    {
        #region Constructors

        public DiscrepancyResolution()
        {
        }

        public DiscrepancyResolution(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator DiscrepancyResolution(Guid key)
        {
            return FromKey(key);
        }

        public static DiscrepancyResolution FromKey(Guid key)
        {
            return new DiscrepancyResolution(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a Discrepancy Resolution.
        /// </summary>
        [Column("DiscrepancyResolutionKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a discrepancy resolution.
        /// </summary>
        public DiscrepancyResolutionInternalCode? DiscrepancyResolutionInternalCode { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies type of discrepancy Resolution
        /// </summary>
        public string DiscrepancyResolutionTypeInternalCode { get; set; }

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
        /// Gets or sets the text that describes a discrepancy resolution.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.DiscrepancyResolutionUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionRequired")]
        [Column("DescriptionText")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a discrepancy resolution is active.
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
