using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class DispensingDeviceClinicalDataSubjectResult
    {
        public Guid ClinicalDataSubjectDispensingDeviceKey { get; set; }
        public Guid DispensingDeviceKey { get; set; }
        public Guid ClinicalDataSubjectKey { get; set; }
    }
}
