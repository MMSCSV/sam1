using System;
using System.Collections.Generic;
using System.Linq;
using Pyxis.Core.Data.InternalCodes;
using PharmacyOrderScheduling = CareFusion.Dispensing.PharmacyOrderScheduling.Models;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a combination of a quantity, frequency, priority and timing.
    /// </summary>
    public class PharmacyOrderTimingRecord: Entity<Guid>
    {
        #region Constructors

        public PharmacyOrderTimingRecord()
        {
        }

        public PharmacyOrderTimingRecord(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderTimingRecord(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderTimingRecord FromKey(Guid key)
        {
            return new PharmacyOrderTimingRecord(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the integer that identifies a member within an ordered set.
        /// </summary>
        public int MemberNumber { get; set; }

        /// <summary>
        /// Gets or sets the numeric part of an overall duration for which a pharmacy-order-task
        /// act is required.
        /// </summary>
        public decimal? ServiceDurationAmount { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a unit of duration for
        /// an overall duration.
        /// </summary>
        public UnitOfDurationInternalCode? ServiceUnitOfDuration { get; set; }

        /// <summary>
        /// Gets or sets the earliest UTC date and time at which a pharmacy order task should be started
        /// as specified by a requester, or start UTC date and time at which a pharmacy order task should
        /// be started as specified by a filter.
        /// </summary>
        public DateTime? EffectiveUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the earliest local date and time at which a pharmacy order task should be started
        /// as specified by a requester, or start local date and time at which a pharmacy order task should
        /// be started as specified by a filter.
        /// </summary>
        public DateTime? EffectiveLocalDateTime { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether only a date (and no time) is recorded for when
        /// a timing record becomes effective.
        /// </summary>
        public bool IsEffectiveDateOnly { get; set; }

        /// <summary>
        /// Gets or sets the latest UTC date and time at which pharmacy order tasks should be performed
        /// as specified by a requester, or end UTC date and time at which pharmacy order tasks should
        /// be performed as specified by a filter.
        /// </summary>
        public DateTime? ExpirationUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the latest local date and time at which pharmacy order tasks should be performed
        /// as specified by a requester, or end local date and time at which pharmacy order tasks should
        /// be performed as specified by a filter.
        /// </summary>
        public DateTime? ExpirationLocalDateTime { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether only a date (and not time) is recorded for when
        /// a timing record expires.
        /// </summary>
        public bool IsExpirationDateOnly { get; set; }

        /// <summary>
        /// Gets or sets the text that describes conditions under which a task is to be performed.
        /// </summary>
        public string ConditionText { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a timing record conjunction.
        /// </summary>
        public TimingRecordConjunctionInternalCode? Conjuction { get; set; }

        /// <summary>
        /// Gets or sets the total number of occurrences of a task act that should result
        /// from the timing record.
        /// </summary>
        public int? TotalOccurrenceQuantity { get; set; }

        public IEnumerable<PharmacyOrderRepeatPattern> RepeatPatterns { get; set; }

        public IEnumerable<PharmacyOrderExplicitTime> ExplicitTimes { get; set; }

        public IEnumerable<PharmacyOrderPriority> Priorities { get; set; }

        #endregion

        #region Derived Properties
        
        public bool IsStat
        {
            get
            {
                var timingRecordSpecification = ToTimingRecordSpecification();
                return timingRecordSpecification.IsStat;
            }
        }

        public bool IsPrn
        {
            get
            {
                var timingRecordSpecification = ToTimingRecordSpecification();
                return timingRecordSpecification.IsPrn;
            }
        }

        #endregion

        #region Public Members

        public PharmacyOrderScheduling.Models.TimingRecordSpecification ToTimingRecordSpecification()
        {
            var timingRecordSpecification = new PharmacyOrderScheduling.Models.TimingRecordSpecification
                {
                    MemberNumber = MemberNumber,
                    EffectiveUtcDateTime = EffectiveUtcDateTime,
                    ExpirationUtcDateTime = ExpirationUtcDateTime,
                    ServiceDurationAmount = ServiceDurationAmount,
                    ServiceUnitOfDurationInternalCode = ServiceUnitOfDuration.ToInternalCode(),
                    ConjuctionInternalCode = Conjuction.ToInternalCode(),
                    TotalOccurrenceQuantity = TotalOccurrenceQuantity
                };

            if (RepeatPatterns != null)
            {
                timingRecordSpecification.RepeatPatterns = RepeatPatterns
                    .Where(rp => rp.RepeatPattern != null)
                    .OrderBy(rp => rp.MemberNumber)
                    .Select(rp => rp.ToRepeatPatternSpecification())
                    .ToArray();
            }

            if (ExplicitTimes != null)
            {
                timingRecordSpecification.ExplicitTimes = ExplicitTimes
                    .OrderBy(et => et.MemberNumber)
                    .Select(et => et.ToExplicitTimeSpecification())
                    .ToArray();
            }

            if (Priorities != null)
            {
                timingRecordSpecification.Priorities = Priorities
                    .Where(p => p.TimingRecordPriority != null)
                    .OrderBy(p => p.MemberNumber)
                    .Select(p => p.ToPrioritySpecification())
                    .ToArray();
            }

            return timingRecordSpecification;
        }

        #endregion
    }
}
