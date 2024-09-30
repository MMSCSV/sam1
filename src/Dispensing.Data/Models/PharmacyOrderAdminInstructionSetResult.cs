using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class PharmacyOrderAdminInstructionSetResult
    {
        public Guid PharmacyOrderAdminInstructionSetKey { get; set; }

        public Guid PharmacyOrderKey { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
