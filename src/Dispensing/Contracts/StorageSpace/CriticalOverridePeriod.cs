using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents the time period on particular days of the week during which a
    /// critical override may occur.
    /// </summary>
    [Serializable]
    public class CriticalOverridePeriod : Entity<Guid>, ICloneable
    {
        #region Constructors

        public CriticalOverridePeriod()
        {
        }

        public CriticalOverridePeriod(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator CriticalOverridePeriod(Guid key)
        {
            return FromKey(key);
        }

        public static CriticalOverridePeriod FromKey(Guid key)
        {
            return new CriticalOverridePeriod(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of a critical override period.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.CriticalOverridePeriodNameUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_NameOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_NameRequired")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the time of day in minutes of when a critical override
        /// period starts.
        /// </summary>
        [DispensingRangeValidator(ValidationConstants.CriticalOverridePeriodStartTimeOfDayLowerBound,
            ValidationConstants.CriticalOverridePeriodStartTimeOfDayUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "CriticalOverridePeriodStartTimeOfDayOutOfBounds")]
        public short StartTimeOfDay { get; set; }

        /// <summary>
        /// Gets or sets the time of day in minutes of when a critical override
        /// period ends.
        /// </summary>
        [DispensingRangeValidator(ValidationConstants.CriticalOverridePeriodEndTimeOfDayLowerBound,
            ValidationConstants.CriticalOverridePeriodEndTimeOfDayUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "CriticalOverridePeriodEndTimeOfDayOutOfBounds")]
        public short EndTimeOfDay { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the period is applicable 
        /// on each Monday.
        /// </summary>
        public bool Monday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the period is applicable 
        /// on each Tuesday.
        /// </summary>
        public bool Tuesday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the period is applicable 
        /// on each Wednesday.
        /// </summary>
        public bool Wednesday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the period is applicable 
        /// on each Thursday.
        /// </summary>
        public bool Thursday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the period is applicable 
        /// on each Friday.
        /// </summary>
        public bool Friday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the period is applicable 
        /// on each Saturday.
        /// </summary>
        public bool Saturday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the period is applicable 
        /// on each Sunday.
        /// </summary>
        public bool Sunday { get; set; }

        /// <summary>
        /// Gets or sets the UTC date when the period was created
        /// </summary>
        public DateTime? CreatedUTCDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date when the period was created
        /// </summary>
        public DateTime? CreatedLocalDateTime { get; set; }
        /// <summary>
        /// 
        ///  Gets or sets the Key of actor for which the critical override period is created.
        /// </summary>
        public Guid? CreatedActorKey { get; set; }

        /// <summary>
        ///  Gets or sets the name of actor for which the critical override period is created.
        /// </summary>
        public string CreatedActorName { get; set; }

        /// <summary>
        /// Gets or sets the name of dispensing device for which the critical override period is associated.
        /// </summary>
        public string DispensingDeviceName { get; set; }

        /// <summary>
        ///  Gets or sets the Key of dispensing device for which the critical override period is associated.
        /// </summary>
        public Guid DispensingDeviceKey { get; set; }

        /// <summary>
        /// Gets or sets the IsDelete flag for critical override period
        /// </summary>
        public bool IsToBeDeleted { get; set; }

        /// <summary>
        /// Gets or sets dispensing device count for which the critical override period is associated.
        /// </summary>
        public int AssociatedDeviceCount { get; set; }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Performs a deep-copy of the current instance.
        /// </summary>
        /// <returns></returns>
        public CriticalOverridePeriod Clone()
        {
            CriticalOverridePeriod clone = (CriticalOverridePeriod)ShallowClone(true);

            // nothing to deep-copy

            return clone;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion
    }
}
