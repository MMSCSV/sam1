using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class PharmacyOrderComponentResult
    {
        public Guid PharmacyOrderComponentKey { get; set; }

        public Guid PharmacyOrderComponentSetKey { get; set; }

        public int MemberNumber { get; set; }

        public string PharmacyOrderComponentTypeInternalCode { get; set; }

        public Guid? MedItemKey { get; set; }

        public string ComponentId { get; set; }

        public string ComponentDescriptionText { get; set; }

        public decimal? ComponentAmount { get; set; }

        public Guid? ComponentUOMKey { get; set; }

        public string ComponentUOMDisplayCode { get; set; }

        public string ComponentUOMDescriptionText { get; set; }

        public bool ComponentUOMUseDosageFormFlag { get; set; }

        public bool ComponentUOMActiveFlag { get; set; }

        public Guid? ComponentUOMBaseUOMKey { get; set; }

        public decimal? ComponentUOMConversionAmount { get; set; }

        public Guid? ComponentExternalUOMKey { get; set; }

        public int NetRemoveOccurrenceQuantity { get; set; }

        public DateTime? LinkedUtcDateTime { get; set; }

        public DateTime? LinkedLocalDateTime { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
