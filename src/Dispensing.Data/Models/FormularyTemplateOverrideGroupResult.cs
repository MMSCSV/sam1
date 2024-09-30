using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class FormularyTemplateOverrideGroupResult
    {
        public Guid FormularyTemplateOverrideGroupKey { get; set; }
        public Guid FormularyTemplateKey { get; set; }
        public Guid OverrideGroupKey { get; set; }
        public string DisplayCode { get; set; }
        public string DescriptionText { get; set; }
        public bool ActiveFlag { get; set; }
    }
}
