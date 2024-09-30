using System;
using CareFusion.Dispensing.PharmacyOrderScheduling.Models;

namespace CareFusion.Dispensing.Contracts
{
    public class PharmacyOrderExplicitTime: Entity<Guid>
    {
        #region Constructors

        public PharmacyOrderExplicitTime()
        {
        }

        public PharmacyOrderExplicitTime(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderExplicitTime(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderExplicitTime FromKey(Guid key)
        {
            return new PharmacyOrderExplicitTime(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the integer that identifies a member within an ordered set.
        /// </summary>
        public int MemberNumber { get; set; }

        /// <summary>
        /// Gets or sets the time of day in minutes that an administration should occur.
        /// </summary>
        public short TimeOfDay { get; set; }

        public TimeSpan TimeOfDayTimeSpan
        {
            get { return TimeSpan.FromMinutes(TimeOfDay); }
        }

        #endregion

        #region Public Members

        public ExplicitTimeSpecification ToExplicitTimeSpecification()
        {
            return new ExplicitTimeSpecification
                {
                    MemberNumber = MemberNumber,
                    TimeOfDay = TimeOfDay
                };
        }

        #endregion
    }
}
