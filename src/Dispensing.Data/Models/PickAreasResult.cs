using System;
using System.Data.Linq;

namespace CareFusion.Dispensing.Data.Models
{
    public class PickAreasResult
    {
        public Guid PickAreaKey { get; set; }

        public Guid FacilityKey { get; set; }

        public string FacilityCode { get; set; }

        public string FacilityName { get; set; }

        public string PickAreaName { get; set; }

        public bool DeleteFlag { get; set; }

        public Binary LastModifiedBinaryValue { get; set; }
    }
}