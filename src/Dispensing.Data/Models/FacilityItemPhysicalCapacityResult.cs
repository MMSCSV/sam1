using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class FacilityItemPhysicalCapacityResult
    {
        public Guid FacilityItemPhysicalCapacityKey { get; set; }
        public Guid FacilityItemKey { get; set; }
        public string StorageSpaceSizeInternalCode { get; set; }
        public string StorageSpaceSizeDisplayCode { get; set; }
        public string StorageSpaceSizeDescriptionText { get; set; }
        public int? MaximumQuantity { get; set; }
        public int? PhysicalMaximumQuantity { get; set; }
        public int? ParQuantity { get; set; }
        public int? RefillPointQuantity { get; set; }
        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
