using System;

namespace CareFusion.Dispensing.Contracts
{
    public class ClinicalDataNotice: Entity<Guid>
    {
        public Guid ClinicalDataSubjectKey { get; set; }

        public Guid? ClinicalDataResponseKey { get; set; }

        public DateTime NoticeUtcDateTime { get; set; }

        public DateTime NoticeLocalDateTime { get; set; }

        public string ResponseFreeformText { get; set; }
    }
}
