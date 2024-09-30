using System;

namespace Pyxis.Dispensing.Notification.PublishedNotices.Models
{
    internal class EncounterUnit
    {
        public Guid EncounterKey { get; set; }

        public Guid UnitKey { get; set; }

        public string OMNLNoticePrinterName { get; set; }

        public Guid FacilityKey { get; set; }

        public bool OMNLToPrintEquivalenciesFlag { get; set; }
    }
}
