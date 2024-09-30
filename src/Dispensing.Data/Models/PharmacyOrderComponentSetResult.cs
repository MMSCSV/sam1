using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class PharmacyOrderComponentSetResult
    {
        public Guid PharmacyOrderComponentSetKey { get; set; }

        public Guid PharmacyOrderKey { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
