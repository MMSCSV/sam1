using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CareFusion.Dispensing.Contracts
{
    public class PharmacyOrderScheduleStatus
    {
        public string ErrorDescription { get; set; }

        public PharmacyOrderScheduleState State { get; set; }
    }

    public enum PharmacyOrderScheduleState
    {
        Valid,

        InvalidSchedule,

        InvalidTasks
    }

    public class PharmacyOrderSchedule
    {
        public Guid OrderKey { get; set; }

        public PharmacyOrderScheduleStatus Status { get; set; }

        public bool IsScheduled
        {
            get
            {
                return (Tasks != null && Tasks.Count() > 0);
            }
        }

        public bool IsPrn { get; set; }

        public bool IsContinuous { get; set; }

        public bool IsOnCall { get; set; }

        public bool IsStat { get; set; }

        public bool IsOneTime { get; set; }

        public TimeSpan? FrequencyInterval { get; set; }

        public IEnumerable<PharmacyOrderTask> Tasks { get; set; }

        public TimeZoneInfo TimeZone { get; set; }

        public DateTime StartUtcDateTime { get; set; }

        public DateTime StartDateTime
        {
            get
            {
                return TimeZoneInfo.ConvertTimeFromUtc(StartUtcDateTime, TimeZone);
            }
        }

        public DateTime? EndUtcDateTime { get; set; }

        public DateTime? EndDateTime
        {
            get
            {
                return EndUtcDateTime.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(EndUtcDateTime.Value, TimeZone) : default(DateTime?);
            }
        }

        public int? TotalOccurrences { get; set; }

        public string FrequencyDisplayString { get; set; }
    }
}
