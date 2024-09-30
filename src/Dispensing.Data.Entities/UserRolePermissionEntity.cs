using System;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class UserRolePermissionEntity : IContractConvertible<RolePermission>
    {
        #region IContractConvertible<RolePermission> Members

        public RolePermission ToContract()
        {
            Permission permission = (PermissionEntity != null)
                ? PermissionEntity.ToContract()
                : PermissionInternalCode.FromInternalCode<PermissionInternalCode>();

            Guid[] securityGroupKeys = UserRolePermissionSecurityGroupEntities
                    .Select(urpsg => urpsg.SecurityGroupKey)
                    .ToArray();

            return new RolePermission(Key)
                {
                    Permission = permission,
                    SecurityGroups = securityGroupKeys
                };
        }

        #endregion
    }
}
