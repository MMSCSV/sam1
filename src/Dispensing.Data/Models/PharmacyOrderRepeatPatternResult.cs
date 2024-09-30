using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class PharmacyOrderRepeatPatternResult
    {
        public Guid PharmacyOrderRepeatPatternKey { get; set; }

        public Guid PharmacyOrderTimingRecordKey { get; set; }

        public int MemberNumber { get; set; }

        public Guid RepeatPatternKey { get; set; }

        public Guid RepeatPatternExternalSystemKey { get; set; }

        public string RepeatPatternCode { get; set; }

        public string RepeatPatternDescriptionText { get; set; }

        public string StandardRepeatPatternInternalCode { get; set; }

        public string StandardRepeatPatternDisplayCode { get; set; }

        public string StandardRepeatPatternDescriptionText { get; set; }

        public decimal? RepeatPatternPeriodAmount { get; set; }

        public bool RepeatPatternMondayFlag { get; set; }

        public bool RepeatPatternTuesdayFlag { get; set; }

        public bool RepeatPatternWednesdayFlag { get; set; }

        public bool RepeatPatternThursdayFlag { get; set; }

        public bool RepeatPatternFridayFlag { get; set; }

        public bool RepeatPatternSaturdayFlag { get; set; }

        public bool RepeatPatternSundayFlag { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
