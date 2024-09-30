using System;

namespace CareFusion.Dispensing.Data.Models
{
    internal class PharmacyOrderDeliveryStateResult
    {
        public Guid PharmacyOrderKey { get; set; }

        public string PharmacyOrderId { get; set; }

        public Guid ItemDeliveryKey { get; set; }

        public string ScanCodeValue { get; set; }

        public long TrackingNumber { get; set; }

        public string ItemDeliveryTrackingStatusInternalCode { get; set; }

        public DateTime StartLocalDateTime { get; set; }

        public DateTime StartUtcDateTime { get; set; }

        public Guid? LastModifiedActorKey { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string ContactInformation { get; set; }

        public string DeliveryLocationName { get; set; }

        public Guid DeliveryLocationKey { get; set; }
    }
}
