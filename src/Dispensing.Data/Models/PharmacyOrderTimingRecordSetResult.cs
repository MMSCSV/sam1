using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class PharmacyOrderTimingRecordSetResult
    {
        public Guid PharmacyOrderTimingRecordSetKey { get; set; }

        public Guid PharmacyOrderKey { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
