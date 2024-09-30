using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class PharmacyOrderOrderingPersonResult
    {
        public Guid PharmacyOrderOrderingPersonKey { get; set; }

        public Guid PharmacyOrderOrderingPersonSetKey { get; set; }

        public int MemberNumber { get; set; }

        public string PersonId { get; set; }

        public string PrefixText { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string SuffixText { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
