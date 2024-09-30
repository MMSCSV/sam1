using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class PharmacyOrderPriorityResult
    {
        public Guid PharmacyOrderPriorityKey { get; set; }

        public Guid PharmacyOrderTimingRecordKey { get; set; }

        public int MemberNumber { get; set; }

        public Guid TimingRecordPriorityKey { get; set; }

        public Guid TimingRecordPriorityExternalSystemKey { get; set; }

        public string TimingRecordPriorityCode { get; set; }

        public string TimingRecordDescriptionText { get; set; }

        public string StandardTimingRecordPriorityInternalCode { get; set; }

        public string StandardTimingRecordPriorityDisplayCode { get; set; }

        public string StandardTimingRecordPriorityDescriptionText { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
