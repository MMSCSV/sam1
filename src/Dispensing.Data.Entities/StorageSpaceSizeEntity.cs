using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class StorageSpaceSizeEntity : IContractConvertible<StorageSpaceSize>
    {
        #region IContractConvertible<StorageSpaceSize> Members

        public StorageSpaceSize ToContract()
        {
            return new StorageSpaceSize(InternalCode, DescriptionText)
                {
                    DisplayCode = DisplayCode,
                    SortValue = SortValue
                };
        }

        #endregion
    }
}
