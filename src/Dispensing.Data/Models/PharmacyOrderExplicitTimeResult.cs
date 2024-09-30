using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class PharmacyOrderExplicitTimeResult
    {
        public Guid PharmacyOrderExplicitTimeKey { get; set; }

        public Guid PharmacyOrderTimingRecordKey { get; set; }

        public int MemberNumber { get; set; }

        public short ExplicitTimeOfDayValue { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
