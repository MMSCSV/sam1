using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class ExternalGenderResults
    {
        public Guid ExternalGenderKey { get; set; }

        public Guid ExternalSystemKey { get; set; }

        public string GenderCode { get; set; }

        public Guid? GenderKey { get; set; }

        public bool UseOnOutboundFlag { get; set; }

        public int? SortValue { get; set; }

        public DateTime? CreatedUtcDateTime { get; private set; } = null;

        public DateTime? CreatedLocalDateTime { get; private set; } = null;

        public Guid? LastModifiedActorKey { get; private set; } = null;

        public byte[] LastModifiedBinaryValue { get; set; }

        public string GenderDisplayCode { get; set; }

        public string GenderInternalCode { get; set; }

        public string GenderDescriptionText { get; set; }

        public int? GenderSortValue { get; set; }

        public bool? GenderActiveFlag { get; set; }

        public byte[] GenderLastModifiedBinaryValue { get; set; }
    }
}
