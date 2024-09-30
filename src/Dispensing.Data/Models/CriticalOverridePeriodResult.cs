using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class CriticalOverridePeriodResult
    {
        public Guid CriticalOverridePeriodKey { get; set; }
        public Guid DispensingDeviceKey { get; set; }
        public string CriticalOverridePeriodName { get; set; }
        public short StartTimeOfDayValue { get; set; }
        public short EndTimeOfDayValue { get; set; }
        public bool MondayFlag { get; set; }
        public bool TuesdayFlag { get; set; }
        public bool WednesdayFlag { get; set; }
        public bool ThursdayFlag { get; set; }
        public bool FridayFlag { get; set; }
        public bool SaturdayFlag { get; set; }
        public bool SundayFlag { get; set; }
        public byte[] LastModifiedBinaryValue { get; set; }
        public DateTime? CreatedUTCDateTime { get; set; }
        public DateTime? CreatedLocalDateTime { get; set; }
        public Guid? CreatedActorKey { get; set; }
        public string CreatedActorName { get; set; }
    }
}
