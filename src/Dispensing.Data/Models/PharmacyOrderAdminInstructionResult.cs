using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class PharmacyOrderAdminInstructionResult
    {
        public Guid PharmacyOrderAdminInstructionKey { get; set; }

        public Guid PharmacyOrderAdminInstructionSetKey { get; set; }

        public int MemberNumber { get; set; }

        public string DescriptionText { get; set; }

        public bool TruncatedFlag { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
