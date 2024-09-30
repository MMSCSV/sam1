using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class FacilityItemBlockedDispensingDeviceResult
    {
        public Guid FacilityItemPickAreaKey { get; set; }
        public Guid FacilityItemKey { get; set; }
        public Guid DispensingDeviceKey { get; set; }
    }
}
