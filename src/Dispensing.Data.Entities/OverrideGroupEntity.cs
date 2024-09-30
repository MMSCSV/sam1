using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class OverrideGroupEntity : IContractConvertible<OverrideGroup>
    {
        #region IContractConvertible<OverrideGroup> Members

        public OverrideGroup ToContract()
        {
            return new OverrideGroup(Key)
                {
                    DisplayCode = DisplayCode,
                    Description = DescriptionText,
                    IsActive = ActiveFlag,
                    SortOrder = SortValue,
                    LastModified = LastModifiedBinaryValue.ToArray()
                };
        }

        #endregion
    }
}
