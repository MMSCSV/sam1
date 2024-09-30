using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class PharmacyOrderRouteResult
    {
        public Guid PharmacyOrderRouteKey { get; set; }

        public Guid PharmacyOrderRouteSetKey { get; set; }

        public int MemberNumber { get; set; }

        public string AdminRouteCode { get; set; }

        public string AdminRouteDescriptionText { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
