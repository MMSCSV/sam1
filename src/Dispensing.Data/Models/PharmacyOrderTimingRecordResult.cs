using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class PharmacyOrderTimingRecordResult
    {
        public Guid PharmacyOrderTimingRecordKey { get; set; }

        public Guid PharmacyOrderTimingRecordSetKey { get; set; }

        public int MemberNumber { get; set; }

        public decimal? ServiceDurationAmount { get; set; }

        public string ServiceUnitOfDurationInternalCode { get; set; }

        public DateTime? EffectiveUtcDateTime { get; set; }

        public DateTime? EffectiveLocalDateTime { get; set; }

        public bool EffectiveDateOnlyFlag { get; set; }

        public DateTime? ExpirationUtcDateTime { get; set; }

        public DateTime? ExpirationLocalDateTime { get; set; }

        public bool ExpirationDateOnlyFlag { get; set; }

        public string ConditionText { get; set; }

        public string TimingRecordConjunctionInternalCode { get; set; }

        public int? TotalOccurrenceQuantity { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
