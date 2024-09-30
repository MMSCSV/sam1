using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class DispensingDeviceRestockGroupResult
    {
        public Guid GCSMRestockGroupKey { get; set; }
        public Guid GCSMDispensingDeviceKey { get; set; }
        public Guid DispensingDeviceKey { get; set; }
        public string RestockGroupName { get; set; }
        public string DescriptionText { get; set; }
        public string GCSMDispensingDeviceName { get; set; }
        public bool ActiveFlag { get; set; }
        public string FacilityName { get; set; }
    }
}
