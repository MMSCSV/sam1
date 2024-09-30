using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class PharmacyOrderResult
    {
        public Guid PharmacyOrderSnapshotKey { get; set; }

        public Guid PharmacyOrderKey { get; set; }

        public DateTime? EffectiveUtcDateTime { get; set; }

        public DateTime? EffectiveLocalDateTime { get; set; }

        public bool EffectiveDateOnlyFlag { get; set; }

        public DateTime? ExpirationUtcDateTime { get; set; }

        public DateTime? ExpirationLocalDateTime { get; set; }

        public bool ExpirationDateOnlyFlag { get; set; }

        public int? TotalOccurrenceQuantity { get; set; }

        public int NetRemoveOccurrenceQuantity { get; set; }

        public bool CompletedFlag { get; set; }

        public Guid ExternalSystemKey { get; set; }

        public Guid EncounterKey { get; set;  }

        public string PharmacyOrderId { get; set; }

        public bool DiscontinuedFlag { get; set; }

        public bool CancelledFlag { get; set; }

        public DateTime? HoldEffectiveUtcDateTime { get; set; }

        public DateTime? HoldEffectiveLocalDateTime { get; set; }

        public bool HoldEffectiveDateOnlyFlag { get; set; }

        public DateTime? ReleaseHoldEffectiveUtcDateTime { get; set; }

        public DateTime? ReleaseHoldEffectiveLocalDateTime { get; set; }

        public bool ReleaseHoldEffectiveDateOnlyFlag { get; set; }

        public DateTime? CancelledUtcDateTime { get; set; }

        public DateTime? CancelledLocalDateTime { get; set; }

        public Guid? GiveItemKey { get; set; }

        public string GiveId { get; set; }

        public string GiveDescriptionText { get; set; }

        public decimal? GiveAmount { get; set; }

        public decimal? MaximumGiveAmount { get; set; }

        public Guid? GiveUOMKey { get; set; }

        public string GiveUOMDisplayCode { get; set; }

        public Guid? GiveUOMBaseUOMKey { get; set; }

        public decimal? GiveUOMConversionAmount { get; set; }

        public Guid? GiveExternalUOMKey { get; set; }

        public decimal? DispenseQuantity { get; set; }

        public string PharmacyOrderTypeInternalCode { get; set; }

        public bool InboundWarningFlag { get; set; }

        public bool SchedulePersistableFlag { get; set; }

        public bool DispenseNotUsingDispensingDeviceFlag { get; set; }

        public DateTime? CreatedUtcDateTime { get; set; }

        public DateTime? CreatedLocalDateTime { get; set; }

        public bool ActiveNowFlag { get; set; }

        public bool OnHoldNowFlag { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
