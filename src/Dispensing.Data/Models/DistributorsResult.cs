using System;

namespace CareFusion.Dispensing.Data.Models
{
    public class DistributorsResult
    {
        public Guid DistributorKey { get; set; }

        public string DistributorID { get; set; }

        public string DistributorName { get; set; }
        public string StreetAddressText { get; set; }
        public string CityName { get; set; }
        public string SubdivisionName { get; set; }
        public string CountryName { get; set; }
        public string PostalCode { get; set; }
        public string ContactName { get; set; }
        public string ContactPhoneNumberText { get; set; }
        public bool DeleteFlag { get; set; }
        public byte[] LastModifiedBinaryValue { get; set; }
        public long TotalRows { get; set; }
    }
}
