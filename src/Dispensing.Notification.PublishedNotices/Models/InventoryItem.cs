using System;

namespace Pyxis.Dispensing.Notification.PublishedNotices.Models
{
    public class InventoryItem
    {
        public Guid DispensingDeviceKey { get; set; }

        public Guid ItemKey { get; set; }

        public Guid FacilityKey { get; set; }

        public DateTime? LastInventoryUtcDateTime { get; set; }
        
        public DateTime? LastInventoryLocalDateTime { get; set; }

        public string NoticePrinterName { get; set; }
    }
}
