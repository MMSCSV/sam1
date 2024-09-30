using System;
using System.ComponentModel.DataAnnotations.Schema;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a pre-defined reason for resolving an out-of-temperature-range event.
    /// </summary>
    [Serializable]
    public class OutOfTemperatureRangeResolution : IEntity<Guid>
    {
        #region Constructors

        public OutOfTemperatureRangeResolution()
        {
            IsActive = true;
        }

        public OutOfTemperatureRangeResolution(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator OutOfTemperatureRangeResolution(Guid key)
        {
            return FromKey(key);
        }

        public static OutOfTemperatureRangeResolution FromKey(Guid key)
        {
            return new OutOfTemperatureRangeResolution(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of an out of temperature range resolution.
        /// </summary>
        [Column("OutOfTemperatureRangeResolutionKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        public Guid FacilityKey { get; set; }

        /// <summary>
        /// Gets or sets the Facility name
        /// </summary>
        public string FacilityName { get; set; }

        /// <summary>
        /// Gets or sets the text that describes an out-of-temperature-range resolution.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.OutOfTemperatureRangeResolutionDescriptionUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_DescriptionRequired")]
        [Column("DescriptionText")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether an out-of-temperature-range resolution is active.
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
