using System;

namespace CareFusion.Dispensing.Data.Models
{
    public class ZonesResult
    {
        public Guid ZoneKey { get; set; }

        public Guid FacilityKey { get; set; }

        public string FacilityCode { get; set; }

        public string FacilityName { get; set; }

        public string ZoneName { get; set; }

        public Nullable<short> ZoneNumber { get; set; }

        public bool DeleteFlag { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
