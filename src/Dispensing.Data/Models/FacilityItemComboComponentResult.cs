using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class FacilityItemComboComponentResult
    {
        public Guid ComboComponentKey { get; set; }
        public Guid ComboFacilityItemKey { get; set; }
        public Guid ComponentFacilityItemKey { get; set; }
        public string ComponentMedDisplayName { get; set; }
        public short? ComponentQuantity { get; set; }
        public bool ChargeComponentFlag { get; set; }
        public bool MultiplierFlag { get; set; }
        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
