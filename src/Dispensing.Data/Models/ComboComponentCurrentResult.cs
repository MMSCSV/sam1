using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class ComboComponentCurrentResult
    {
        public Guid ComboComponentKey { get; set; }
        public DateTime StartUTCDateTime { get; set; }
        public DateTime StartLocalDateTime { get; set; }
        public DateTime? EndUTCDateTime { get; set; }
        public DateTime? EndLocalDateTime { get; set; }
        public Guid ComboFacilityItemKey { get; set; }
        public Guid ComponentFacilityItemKey { get; set; }
        public int? ComponentQuantity { get; set; }
        public bool ChargeComponentFlag { get; set; }
        public bool MultiplierFlag { get; set; }
        public bool DeleteFlag { get; set; }
        public Guid? AssociationActorKey { get; set; }
        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
