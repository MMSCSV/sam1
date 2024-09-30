using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class StorageSpaceFormEntity : IContractConvertible<StorageSpaceForm>
    {
        #region IContractConvertible<StorageSpaceForm> Members

        public StorageSpaceForm ToContract()
        {
            return new StorageSpaceForm(Key)
            {
               InternalCode = StorageSpaceFormInternalCode.FromNullableInternalCode<StorageSpaceFormInternalCode>(),
		       StorageSpaceType = StorageSpaceTypeInternalCode.FromInternalCode<StorageSpaceTypeInternalCode>(),
		       Description = DescriptionText,
		       SortOrder = SortValue,
		       ActiveFlag = ActiveFlag,
               ShortName = ShortName,
               LastModified = LastModifiedBinaryValue.ToArray()
            };
        }

        #endregion
    }
}
