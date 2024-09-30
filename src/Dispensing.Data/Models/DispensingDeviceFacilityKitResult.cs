using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class DispensingDeviceFacilityKitResult
    {
        public Guid FacilityKitDispensingDeviceKey { get; set; }
        public Guid DispensingDeviceKey { get; set; }
        public Guid FacilityKitKey { get; set; }
        public bool StatFlag { get; set; }
    }
}
