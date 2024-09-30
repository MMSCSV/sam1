using System;
using System.Linq;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class UserRoleMemberEntity : IContractConvertible<UserRole>
    {
        #region IContractConvertible<UserRole> Members

        public UserRole ToContract()
        {
            Guid[] areaKeys = UserRoleMemberAreaEntities
                    .Select(urma => urma.AreaKey)
                    .ToArray();

            return new UserRole(Key)
                {
                    RoleKey = UserRoleKey,
                    MedicationTemporaryAccess = MedTemporaryAccessFlag,
                    Areas = areaKeys,
                    LastModified = LastModifiedBinaryValue.ToArray()
                };
        }

        #endregion
    }
}
