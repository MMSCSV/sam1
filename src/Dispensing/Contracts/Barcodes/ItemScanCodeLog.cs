using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    public class ItemScanCodeLog
    {
        public Guid ItemScanCodeKey { get; set; }
        public String ScanCode { get; set; }
        public DateTime StartUtcDateTime { get; set; }
        public bool FromExternalSystemFlag { get; set; }
        public Guid? LinkedByUserAccountKey { get; set; }
        public DateTime LinkedUtcDateTime { get; set; }
        public Guid? VerifiedByUserAccountKey { get; set; }
        public DateTime? VerifiedUtcDateTime { get; set; }
        public ScanProductDeleteReasonInternalCode? ScanProductDeleteReasonInternalCode { get; set; }
        public string OtherItemId { get; set; }
        public string ItemDisplayName { get; set; }
        public string ItemId { get; set; }
        public string ScanCodeDeleteReasonDescription { get; set; }
    }
}
