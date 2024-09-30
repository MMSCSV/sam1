using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class ExternalPatientIdentificationTypeResults
    {
        public Guid ExternalPatientIdTypeKey { get; set; }

        public Guid ExternalSystemKey { get; set; }

        public string PatientIdTypeCode { get; set; }

        public Guid? PatientIdTypeKey { get; set; }

        public bool UseOnOutboundFlag { get; set; }

        public int? SortValue { get; set; }

        public DateTime? CreatedUtcDateTime { get; private set; }

        public DateTime? CreatedLocalDateTime { get; private set; }

        public Guid? LastModifiedActorKey { get; private set; }

        public byte[] LastModifiedBinaryValue { get; set; }

        public string PatientIDTypeDisplayCode { get; set; }

        public string PatientIDTypeInternalCode { get; set; }

        public string PatientIDTypeDescriptionText { get; set; }

        public int? PatientIDTypeSortValue { get; set; }

        public bool? PatientIDTypeActiveFlag { get; set; }

        public byte[] PatientIDTypeLastModifiedBinaryValue { get; set; }
    }
}
