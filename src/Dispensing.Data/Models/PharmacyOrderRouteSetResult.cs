using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class PharmacyOrderRouteSetResult
    {
        public Guid PharmacyOrderRouteSetKey { get; set; }

        public Guid PharmacyOrderKey { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
