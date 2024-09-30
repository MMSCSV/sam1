using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class UserRoleEntity : IContractConvertible<Role>
    {
        private EntityRef<FacilityEntity> _facilityEntity;

        partial void OnCreated()
        {
            _facilityEntity = default(EntityRef<FacilityEntity>);
        }

        #region Public Properties

        [Association(Name = "FacilityEntity_UserRoleEntity", Storage = "_facilityEntity", ThisKey = "FacilityKey", OtherKey = "Key", IsForeignKey = true)]
        public FacilityEntity FacilityEntity
        {
            get
            {
                return _facilityEntity.Entity;
            }
            set
            {
                FacilityEntity previousValue = _facilityEntity.Entity;
                if (previousValue != value ||
                    _facilityEntity.HasLoadedOrAssignedValue == false)
                {
                    SendPropertyChanging();

                    _facilityEntity.Entity = value;

                    _FacilityKey = value != null ? value.Key : default(Guid);

                    SendPropertyChanged("FacilityEntity");
                }
            }
        }

        #endregion

        #region IContractConvertible<Role> Members

        public Role ToContract()
        {
            Facility facility = null;
            if (FacilityKey != null)
            {
                facility = (FacilityEntity != null)
                ? new Facility(FacilityEntity.Key) { Name = FacilityEntity.FacilityName }
                : FacilityKey.Value;
            }

            RolePermission[] rolePermissions = UserRolePermissionEntities
                    .Select(urp => urp.ToContract())
                    .ToArray();

            Guid[] overrideGroupKeys = UserRoleOverrideGroupEntities
                    .Select(urog => urog.OverrideGroupKey)
                    .ToArray();

            var userRolePermission = UserRolePermissionEntities
                    .SingleOrDefault(urp => urp.PermissionInternalCode == PermissionInternalCode.GrantVisitorAccess.ToInternalCode());


            Guid[] visitingUserRoleKeys = null;
            if (userRolePermission != null)
            {
                visitingUserRoleKeys = userRolePermission.UserRolePermissionRoleEntities
                    .Select(urpr => urpr.UserRoleKey)
                    .ToArray();
            }

            return new Role(Key)
            {
                AllowTemporaryUsers = AllowForTemporaryUserFlag,
                Facility = facility,
                Description = DescriptionText,
                InternalCode = InternalCode.FromNullableInternalCode<UserRoleInternalCode>(),
                IsActive = ActiveFlag,
                Name = RoleName,
                UserMemberCount = UserRoleMemberEntities.LongCount(),
                Permissions = rolePermissions,
                OverrideGroups = overrideGroupKeys,
                VisitingUserRoles = visitingUserRoleKeys,
                LastModified = LastModifiedBinaryValue.ToArray()
            };
        }

        #endregion
    }
}
