using System;

namespace CareFusion.Dispensing.Data.Models
{
    public class VirtualStockLocationsResult
    {
        public Guid VirtualStockLocationKey { get; set; }

        public Guid FacilityKey { get; set; }

        public string FacilityCode { get; set; }

        public string FacilityName { get; set; }

        public string VirtualStockLocationName { get; set; }

        public bool DeleteFlag { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
