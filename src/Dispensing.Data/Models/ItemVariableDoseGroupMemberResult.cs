using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Data.Models
{
    public class ItemVariableDoseGroupMemberResult
    {
        public Guid VariableDoseGroupMemberKey { get; set; }
        public Guid VariableDoseGroupKey { get; set; }
        public Guid MedItemKey { get; set; }
        public string ItemID { get; set; }
        public string MedDisplayName { get; set; }
        public string PureGenericName { get; set; }
        public string BrandName { get; set; }
        public Guid? DosageFormKey { get; set; }
        public string DosageFormCode { get; set; }
        public Guid? EquivalencyDosageFormGroupKey { get; set; }
        public string EquivalencyDosageFormGroupDisplayCode { get; set; }
        public int RankValue { get; set; }
        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
