using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class PharmacyOrderSupplierDispensingInstructionSetResult
    {
        public Guid PharmacyOrderSupplierDispensingInstructionSetKey { get; set; }

        public Guid PharmacyOrderKey { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
