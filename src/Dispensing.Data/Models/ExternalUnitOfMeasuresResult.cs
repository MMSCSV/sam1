using System;
using System.Data.Linq;

namespace CareFusion.Dispensing.Data.Models
{
    public class ExternalUnitOfMeasuresResult
    {
        public Guid ExternalUOMKey { get; set; }

        public Guid ExternalSystemKey { get; set; }

        public string UOMRoleInternalCode { get; set; }

        public string UOMRoleDescriptionText { get; set; }

        public string UOMCode { get; set; }

        public Guid? UOMKey { get; set; }

        public string UOMDisplayCode { get; set; }

        public bool? UOMUseDosageFormFlag { get; set; }

        public bool UseOnOutboundFlag { get; set; }

        public int? SortValue { get; set; }

        public bool DeleteFlag { get; set; }

        public DateTime? CreatedLocalDateTime { get; set; }

        public DateTime? CreatedUTCDateTime { get; set; }

        public Binary LastModifiedBinaryValue { get; set; }

    }
}