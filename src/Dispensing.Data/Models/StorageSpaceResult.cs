using System;

namespace CareFusion.Dispensing.Data.Models
{
    public class StorageSpaceResult
    {
        public Guid StorageSpaceSnapshotKey { get; set; }

        public Guid StorageSpaceKey { get; set; }

        public Guid FacilityKey { get; set; }

        public string StorageSpaceTypeInternalCode { get; set; }

        public Guid? StorageSpaceFormKey { get; set; }

        public string StorageSpaceFormDescription { get; set; }

        public Guid? ParentStorageSpaceKey { get; set; }

        public string StorageSpaceName { get; set; }

        public string SerialID { get; set; }

        public string RegistrySerialID { get; set; }

        public string StackedSerialID { get; set; }

        public string ProductName { get; set; }

        public string PositionID { get; set; }

        public Guid? AnchorStorageSpaceKey { get; set; }

        public Guid? SystemBusDeviceKey { get; set; }

        public Guid? SecondSystemBusDeviceKey { get; set; }

        public string MiniDrawerTrayModeInternalCode { get; set; }

        public string PendingMiniDrawerTrayModeInternalCode { get; set; }

        public Guid? StorageSpaceStateKey { get; set; }

        public bool ClosedFlag { get; set; }

        public bool LockedFlag { get; set; }

        public bool FailedFlag { get; set; }

        public bool DefrostFlag { get; set; }

        public bool OnBatteryFlag { get; set; }

        public bool FailureRequiresMaintenanceFlag { get; set; }

        public string StorageSpaceFailureReasonInternalCode { get; set; }

        public string StorageSpaceFailureReasonDescription { get; set; }

        public string StorageSpaceShortName { get; set; }

        public string StorageSpaceAbbreviatedName { get; set; }

        public byte[] StorageSpaceLastModified { get; set; }

        public string StorageSpaceFormInternalCode { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }

        public string SortValue { get; set; }

        public byte? LevelNumber { get; set; }

        public Guid? DispensingDeviceKey { get; set; }

        public string ParentMiniDrawerTrayModeInternalCode { get; set; }

        public string ParentPendingMiniDrawerTrayModeInternalCode { get; set; }

        public bool MoreThanOneItemFlag { get; set; }

        public bool UnavailableForInventoryFlag { get; set; }

        public bool FailureInHierarchyFlag { get; set; }

        public bool DefrostInHierarchyFlag { get; set; }

        public bool RestrictedAccessFlag { get; set; }

        public string InteriorCabinetLightIntensityModeInternalCode { get; set; }
    }
}
