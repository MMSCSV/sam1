using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class AdminRouteResult
    {
        public Guid AdminRouteKey { get; set; }

        public Guid ExternalSystemKey { get; set; }

        public string ExternalSystemName { get; set; }

        public string AdminRouteCode { get; set; }

        public string DescriptionText { get; set; }

        public int? SortValue { get; set; }

        public bool DeleteFlag { get; set; }

        public DateTime? CreatedLocalDateTime { get; set; }

        public DateTime? CreatedUtcDateTime { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
