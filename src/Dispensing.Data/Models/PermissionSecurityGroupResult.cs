using System;

namespace CareFusion.Dispensing.Data.Models
{
    public class PermissionSecurityGroupResult
    {
        public Guid UserRolePermissionKey { get; set; }

        public Guid SecurityGroupKey { get; set; }
    }
}
