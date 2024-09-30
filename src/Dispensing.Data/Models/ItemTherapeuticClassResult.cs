using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class ItemTherapeuticClassResult
    {
        public Guid TherapeuticClassMemberKey { get; set; }
        public Guid MedItemKey { get; set; }
        public Guid TherapeuticClassKey { get; set; }
        public string TherapeuticClassCode { get; set; }
        public string TherapeuticClassDescriptionText { get; set; }
    }
}
