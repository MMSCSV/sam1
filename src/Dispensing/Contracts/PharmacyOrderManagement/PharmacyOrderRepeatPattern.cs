using System;
using CareFusion.Dispensing.Models;
using CareFusion.Dispensing.PharmacyOrderScheduling.Models;

namespace CareFusion.Dispensing.Contracts
{
    public class PharmacyOrderRepeatPattern: Entity<Guid>
    {
        #region Constructors

        public PharmacyOrderRepeatPattern()
        {
        }

        public PharmacyOrderRepeatPattern(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderRepeatPattern(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderRepeatPattern FromKey(Guid key)
        {
            return new PharmacyOrderRepeatPattern(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the integer that identifies a member within an ordered set.
        /// </summary>
        public int MemberNumber { get; set; }

        /// <summary>
        /// Gets or sets the repeat pattern.
        /// </summary>
        public RepeatPattern RepeatPattern { get; set; }

        #endregion

        #region Public Members

        public RepeatPatternSpecification ToRepeatPatternSpecification()
        {
            if (RepeatPattern == null)
                return null;

            return new RepeatPatternSpecification
                {
                    MemberNumber = MemberNumber,
                    RepeatPatternKey = RepeatPattern.Key,
                    Code = RepeatPattern.Code,
                    Description = RepeatPattern.Description,
                    StandardRepeatPatternInternalCode = RepeatPattern.StandardRepeatPatternInternalCode,
                    PeriodAmount = RepeatPattern.PeriodAmount,
                    Monday = RepeatPattern.Monday,
                    Tuesday = RepeatPattern.Tuesday,
                    Wednesday = RepeatPattern.Wednesday,
                    Thursday = RepeatPattern.Thursday,
                    Friday = RepeatPattern.Friday,
                    Saturday = RepeatPattern.Saturday,
                    Sunday = RepeatPattern.Sunday
                };
        }

        #endregion
    }
}
