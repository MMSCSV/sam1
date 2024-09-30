using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class PermissionEntity : IContractConvertible<Permission>
    {
        #region IContractConvertible<Permission> Members

        public Permission ToContract()
        {
            PermissionType type = null;
            if (!string.IsNullOrEmpty(PermissionTypeInternalCode))
            {
                type = (PermissionTypeEntity != null)
                           ? PermissionTypeEntity.ToContract()
                           : PermissionTypeInternalCode.FromInternalCode<PermissionTypeInternalCode>();
            }

            return new Permission(InternalCode, DescriptionText)
            {
                PermissionType = type,
                PermissionTypeInternalCode = PermissionTypeInternalCode,
                PermissionName = PermissionName,
                SecurityGroupApplicable = SecurityGroupApplicableFlag, 
                SupportUserOnly = SupportUserOnlyFlag,
                Hide = HideFlag,
                Version = VersionText,
                ESSystem = ESSystemFlag,
                Pharmogistics = PharmogisticsFlag,
                GCSM = GCSMFlag
            };
        }

        #endregion
    }
}
