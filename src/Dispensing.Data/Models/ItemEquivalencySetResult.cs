using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class ItemEquivalencySetResult
    {
        public Guid ItemEquivalencySetKey { get; set; }
        public Guid ItemKey { get; set; }
        public bool ApprovedFlag { get; set; }
        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
