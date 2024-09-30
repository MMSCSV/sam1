using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class DispensingDeviceAreaResult
    {
        public Guid AreaDispensingDeviceKey { get; set; }
        public Guid DispensingDeviceKey { get; set; }
        public Guid AreaKey { get; set; }
        public string AreaName { get; set; }
    }
}
