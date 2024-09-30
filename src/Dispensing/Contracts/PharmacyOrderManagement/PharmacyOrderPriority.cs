using System;
using CareFusion.Dispensing.Models;
using CareFusion.Dispensing.PharmacyOrderScheduling.Models;

namespace CareFusion.Dispensing.Contracts
{
    public class PharmacyOrderPriority: Entity<Guid>
    {
        #region Constructors

        public PharmacyOrderPriority()
        {
        }

        public PharmacyOrderPriority(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderPriority(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderPriority FromKey(Guid key)
        {
            return new PharmacyOrderPriority(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the integer that identifies a member within an ordered set.
        /// </summary>
        public int MemberNumber { get; set; }

        /// <summary>
        /// Gets or sets the timing record priority.
        /// </summary>
        public TimingRecordPriority TimingRecordPriority { get; set; }

        #endregion

        #region Public Members

        public PrioritySpecification ToPrioritySpecification()
        {
            if (TimingRecordPriority == null)
                return null;

            return new PrioritySpecification
                {
                    MemberNumber = MemberNumber,
                    TimingRecordPriorityKey = TimingRecordPriority.Key,
                    Code = TimingRecordPriority.Code,
                    Description = TimingRecordPriority.Description,
                    StandardTimingRecordPriorityInternalCode = TimingRecordPriority.StandardTimingRecordPriorityInternalCode
                };
        }

        #endregion
    }
}
