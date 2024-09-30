using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class FacilityItemClinicalDataSubjectResult
    {
        public Guid ClinicalDataSubjectFacilityItemFunctionKey { get; set; }
        public Guid FacilityItemKey { get; set; }
        public string PatientCareFunctionInternalCode { get; set; }
        public Guid ClinicalDataSubjectKey { get; set; }
    }
}
