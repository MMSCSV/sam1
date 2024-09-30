using System;
using System.Data.Linq;

namespace CareFusion.Dispensing.Data.Models
{
    public class MedClassesResult
    {
        public Guid MedClassKey { get; set; }

        public Guid ExternalSystemKey { get; set; }

        public string ExternalSystemName { get; set; }

        public string MedClassCode { get; set; }

        public string DescriptionText { get; set; }

        public bool ControlledFlag { get; set; }

        public Guid? FormularyTemplateKey { get; set; }

        public int? SortValue { get; set; }

        public bool DeleteFlag { get; set; }

        public Binary LastModifiedBinaryValue { get; set; }
    }
}