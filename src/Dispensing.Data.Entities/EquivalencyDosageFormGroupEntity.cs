using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class EquivalencyDosageFormGroupEntity : IContractConvertible<EquivalencyDosageFormGroup>
    {
        #region IContractConvertible<EquivalencyDosageFormGroup> Members

        public EquivalencyDosageFormGroup ToContract()
        {
            return new EquivalencyDosageFormGroup(Key)
            {
                DisplayCode = DisplayCode,
                InternalCode = InternalCode.FromNullableInternalCode<EquivalencyDosageFormGroupInternalCode>(),
                Description = DescriptionText,
                SortOrder = SortValue,
                LastModified = LastModifiedBinaryValue.ToArray()
            };
        }

        #endregion
    }
}
