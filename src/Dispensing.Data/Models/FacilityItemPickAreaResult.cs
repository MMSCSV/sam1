using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class FacilityItemPickAreaResult
    {
        public Guid FacilityItemPickAreaKey { get; set; }
        public Guid FacilityItemKey { get; set; }
        public Guid PickAreaKey { get; set; }
        public string PickAreaName { get; set; }
    }
}
