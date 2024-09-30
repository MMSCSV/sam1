using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class ItemEquivalencyResult
    {
        public Guid ItemEquivalencyKey { get; set; }
        public Guid ItemEquivalencySetKey { get; set; }
        public Guid EquivalentItemKey { get; set; }
        public string EquivalentItemID { get; set; }
        public string EquivalentItemMedDisplayName { get; set; }
        public string EquivalentItemPureGenericName { get; set; }
        public string EquivalentItemBrandName { get; set; }
        public string EquivalentItemDosageFormCode { get; set; }
        public short EquivalentItemQuantity { get; set; }
        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
