using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class PermissionTypeEntity : IContractConvertible<PermissionType>
    {
        #region IContractConvertible<PermissionType> Members

        public PermissionType ToContract()
        {
            return new PermissionType(InternalCode, DescriptionText);
        }

        #endregion
    }
}
