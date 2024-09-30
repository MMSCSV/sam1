using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class PharmacyOrderSupplierDispensingInstructionResult
    {
        public Guid PharmacyOrderSupplierDispensingInstructionKey { get; set; }

        public Guid PharmacyOrderSupplierDispensingInstructionSetKey { get; set; }

        public int MemberNumber { get; set; }

        public string DescriptionText { get; set; }

        public bool TruncatedFlag { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
