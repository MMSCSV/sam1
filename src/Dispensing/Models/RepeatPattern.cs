using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Resources;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Models
{
    /// <summary>
    /// Represents a unit of frequency for a repeat pattern period.
    /// </summary>
    [Serializable]
    public class RepeatPattern : IEntity<Guid>
    {
        #region Constructors

        public RepeatPattern()
        {
        }

        public RepeatPattern(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator RepeatPattern(Guid key)
        {
            return FromKey(key);
        }

        public static RepeatPattern FromKey(Guid key)
        {
            return new RepeatPattern(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a repeat pattern.
        /// </summary>
        [Column("RepeatPatternKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of an external system.
        /// </summary>
        public Guid ExternalSystemKey { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies a repeat pattern.
        /// </summary>
        [Column("RepeatPatternCode")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a repeat pattern.
        /// </summary>
        [Column("DescriptionText")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that is used to control the sort order.
        /// </summary>
        [Column("SortValue")]
        public int? SortOrder { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a standard repeat pattern.
        /// </summary>
        public string StandardRepeatPatternInternalCode { get; set; }

        /// <summary>
        /// Gets the standard repeat pattern.
        /// </summary>
        public StandardRepeatPatternInternalCode? StandardRepeatPattern
        {
            get { return StandardRepeatPatternInternalCode.FromNullableInternalCode<StandardRepeatPatternInternalCode>(); }
        }

        /// <summary>
        /// Gets or sets the internal code that identifies a standard timing record priority.
        /// </summary>
        public string StandardRepeatPatternDisplayCode { get; set; }

        /// <summary>
        /// Gets or sets the numeric part of a repeating frequency.
        /// </summary>
        [Column("RepeatPatternPeriodAmount")]
        public decimal? PeriodAmount { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether treatment is to be administered on a Monday.
        /// </summary>
        [Column("MondayFlag")]
        public bool Monday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether treatment is to be administered on a Tuesday.
        /// </summary>
        [Column("TuesdayFlag")]
        public bool Tuesday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether treatment is to be administered on a Wednesday.
        /// </summary>
        [Column("WednesdayFlag")]
        public bool Wednesday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether treatment is to be administered on a Thursday.
        /// </summary>
        [Column("ThursdayFlag")]
        public bool Thursday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether treatment is to be administered on a Friday.
        /// </summary>
        [Column("FridayFlag")]
        public bool Friday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether treatment is to be administered on a Saturday.
        /// </summary>
        [Column("SaturdayFlag")]
        public bool Saturday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether treatment is to be administered on a Sunday.
        /// </summary>
        [Column("SundayFlag")]
        public bool Sunday { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a repeat pattern is used as the ONCE record for manually created stat orders
        /// </summary>
        [Column("StatFlag")]
        public bool Stat { get; set; }

        /// <summary>
        /// Gets or sets the binary value that is unique within a database and that is assigned a new
        /// value on insert and on update.
        /// </summary>
        [Column("LastModifiedBinaryValue")]
        public byte[] LastModified { get; set; }

        /// <summary>
        /// Gets or sets the location repeat patterns associated with a repeat pattern.
        /// </summary>
        public LocationRepeatPattern[] ExplicitTimes { get; set; }

        #endregion

        #region Public Members

        public bool IsTransient()
        {
            return Key == default(Guid);
        }

        public bool HasSelectedDayOfWeek()
        {
            return Monday || Tuesday || Wednesday || Thursday || Friday || Saturday || Sunday;
        }

        public bool ValidatePeriodAmount(decimal minimum, out string message)
        {
            message = string.Empty;
            if (PeriodAmount == null)
            {
                message = string.Format(CultureInfo.CurrentCulture,
                                               ValidationStrings.RepeatPatternPeriodAmountRequired, minimum - 1);
            }

            if (PeriodAmount < minimum)
            {
                message = string.Format(CultureInfo.CurrentCulture,
                                               ValidationStrings.RepeatPatternPeriodAmountRequired, minimum - 1);
            }
            return string.IsNullOrEmpty(message);
        }

        public bool ValidateExplicitTimes(int minimum, int maximum, out string[] errors)
        {
            errors = new string[0];
            if (ExplicitTimes == null || !ExplicitTimes.Any())
                return true; // Passed validation

            List<string> validationErrors = new List<string>();
            var duplicates = ExplicitTimes.Select(lrp => lrp.FacilityKey)
                .GroupBy(f => f)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);
            if (duplicates.Any())
            {
                validationErrors.Add(ValidationStrings.RepeatPatternExplicitTimesDuplicateFacilities);
            }

            foreach (LocationRepeatPattern locationRepeatPattern in ExplicitTimes)
            {
                if (StandardRepeatPattern == Pyxis.Core.Data.InternalCodes.StandardRepeatPatternInternalCode.QSHIFT)
                {
                    // Explicit times are optional between the range, therefore we need to clean up
                    // the data.
                    if (locationRepeatPattern.LocationRepeatPatternTimes != null)
                    {
                        List<LocationRepeatPatternTime> realTimes = new List<LocationRepeatPatternTime>();
                        foreach (LocationRepeatPatternTime lrp in locationRepeatPattern.LocationRepeatPatternTimes.Reverse())
                        {
                            if (lrp == null &&
                                realTimes.Count == 0)
                                continue;

                            realTimes.Insert(0, lrp);
                        }

                        locationRepeatPattern.LocationRepeatPatternTimes = realTimes.ToArray();
                    }
                }

                LocationRepeatPatternTime[] times = locationRepeatPattern.LocationRepeatPatternTimes;
                if (times == null || times.Length == 0)  //bug fix: 105046
                {
                    continue;
                }

                if (times == null ||
                    times.Length < minimum ||
                    times.Length > maximum ||
                    times.Any(lrp => lrp == null))
                {
                    string message;
                    if (maximum == int.MaxValue)
                        message = string.Format(CultureInfo.CurrentCulture,
                            ValidationStrings.RepeatPatternExplicitTimesSetsRequired, minimum);
                    else
                        message = string.Format(CultureInfo.CurrentCulture,
                            ValidationStrings.RepeatPatternExplicitTimesSetsRangeRequired, minimum, maximum);

                    validationErrors.Add(message);
                }
                else
                {
                    IEnumerable<short> explicitTimes = times.Select(lrp => lrp.ExplicitTimeOfDay);

                    var duplicateExplicitTimes = explicitTimes
                        .GroupBy(et => et)
                        .Where(g => g.Count() > 1)
                        .Select(g => g.Key);
                    if (duplicateExplicitTimes.Any())
                    {
                        validationErrors.Add(ValidationStrings.RepeatPatternDuplicateExplicitTimes);
                    }

                    // Make sure the explicit times are in order.
                    if (!explicitTimes.IsOrdered())
                    {
                        validationErrors.Add(ValidationStrings.RepeatPatternExplicitTimesNotOrdered);
                    }

                    // Make sure the explicit times are within bounds.
                    if (explicitTimes.Any(et => et < ValidationConstants.RepeatPatternExplicitTimeOfDayLowerBound ||
                                                et > ValidationConstants.RepeatPatternExplicitTimeOfDayUpperBound))
                    {
                        string message = string.Format(CultureInfo.CurrentCulture,
                            ValidationStrings.RepeatPatternExplicitTimesOutOfBounds,
                            MinutesToTime(ValidationConstants.RepeatPatternExplicitTimeOfDayLowerBound),
                            MinutesToTime(ValidationConstants.RepeatPatternExplicitTimeOfDayUpperBound));
                        validationErrors.Add(message);
                    }
                }
            }

            errors = validationErrors.ToArray();
            return (errors.Length == 0); // If no errors then pass validation.
        }

        #endregion

        private string MinutesToTime(int minites)
        {
            DateTime dt = DateTime.MinValue.AddMinutes(minites);
            return dt.ToString("HH:mm");
        }
    }
}
