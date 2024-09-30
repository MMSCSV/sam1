using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class StorageSpaceFailureReasonEntity : IContractConvertible<StorageSpaceFailureReason>
    {
        #region IContractConvertible<StorageSpaceFailureReason> Members

        public StorageSpaceFailureReason ToContract()
        {
            return new StorageSpaceFailureReason(InternalCode, DescriptionText)
                       {
                            SortValue = SortValue
                       };
        }

        #endregion
    }
}
