using System;
using System.Data.Linq;

namespace CareFusion.Dispensing.Data.Models
{
    public class ItemScanCodesResult
    {
        public Guid ItemScanCodeKey { get; set; }

        public Guid ItemKey { get; set; }

        public string ScanCodeValue { get; set; }

        public Guid? LinkedByUserAccountKey { get; set; }

        public string LinkedByUserAccountFullName { get; set; }

        public string LinkedByUserAccountUserID { get; set; }

        public DateTime LinkedLocalDateTime { get; set; }

        public DateTime LinkedUTCDateTime { get; set; }

        public Guid? VerifiedByUserAccountKey { get; set; }

        public string VerifiedByUserAccountFullName { get; set; }

        public string VerifiedByUserAccountUserID { get; set; }

        public DateTime? VerifiedLocalDateTime { get; set; }

        public DateTime? VerifiedUTCDateTime { get; set; }

        public bool FromExternalSystemFlag { get; set; }

        public string ScanProductDeleteReasonInternalCode { get; set; }

        public string OtherItemID { get; set; }

        public string CreatedByExternalSystemName { get; set; }

        public string DeletedByExternalSystemName { get; set; }

        public bool DeleteFlag { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
