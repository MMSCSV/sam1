using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class PharmacyOrderOrderingPersonSetResult
    {
        public Guid PharmacyOrderOrderingPersonSetKey { get; set; }

        public Guid PharmacyOrderKey { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
