using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class ListRejectedDeletedItemScanCodesResult
    {
        public Guid ItemScanCodeKey { get; set; }
        public string ScanCodeValue { get; set; }
        public DateTime StartUTCDateTime { get; set; }
        public bool FromExternalSystemFlag { get; set; }
        public Guid? LinkedByUserAccountKey { get; set; }
        public DateTime LinkedUTCDateTime { get; set; }
        public Guid? VerifiedByUserAccountKey { get; set; }
        public DateTime? VerifiedUTCDateTime { get; set; }
        public string ScanProductDeleteReasonInternalCode { get; set; }
        public string ScanProductDeleteReasonDescription { get; set; }
        public string OtherItemID { get; set; }
        public string MedDisplayName { get; set; }
        public string ItemID { get; set; }

    }
}
