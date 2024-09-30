using Pyxis.Core.Data.Schema.Core.Models;
using System;

namespace CareFusion.Dispensing.Data.Models
{
    public class RolePermissionResult : UserRolePermission
    {
        public string PermissionTypeInternalCode { get; set; }

        public string PermissionTypeInternalCodeDescription { get; set; }

        public string PermissionName { get; set; }

        public string DescriptionText { get; set; }

        public bool SupportUserOnlyFlag { get; set; }

        public bool SecurityGroupApplicableFlag { get; set; }

        public bool HideFlag { get; set; }

        public bool DeleteFlag { get; set; }

        public string VersionText { get; set; }

        public bool ESSystemFlag { get; set; }

        public bool PharmogisticsFlag { get; set; }

        public bool GCSMFlag { get; set; }

        public DateTime LastModifiedUTCDateTime { get; set; }
    }
}
