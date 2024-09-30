using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class StorageSpaceTypeEntity : IContractConvertible<StorageSpaceType>
    {
        #region IContractConvertible<StorageSpaceType> Members

        public StorageSpaceType ToContract()
        {
            return new StorageSpaceType(InternalCode, DescriptionText)
            {
                SortValue = SortValue,
                DirectlyContainsInventory = DirectlyContainsInventoryFlag,
                ProductGenerationInternalCode = ProductGenerationInternalCode,
                ShortName = ShortName
            };
        }

        #endregion
    }
}
