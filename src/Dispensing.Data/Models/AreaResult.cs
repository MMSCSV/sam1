using System;

namespace CareFusion.Dispensing.Data.Models
{
    public class AreaResult
    {
        public Guid AreaKey { get; set; }

        public Guid FacilityKey { get; set; }

        public string FacilityCode { get; set; }

        public string FacilityName { get; set; }

        public string AreaName { get; set; }

        public string DescriptionText { get; set; }

        public bool AllUserRolesFlag { get; set; }

        public bool DeleteFlag { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
