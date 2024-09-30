using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    public class StorageSpace : Entity<Guid>
    {
        #region Constructors

        public StorageSpace()
        {
        }

        public StorageSpace(Guid key)
        {
            Key = key;
        }

        public StorageSpace(Guid key, Guid snapshotKey)
        {
            Key = key;
            SnapshotKey = snapshotKey;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator StorageSpace(Guid key)
        {
            return FromKey(key);
        }

        public static StorageSpace FromKey(Guid key)
        {
            return new StorageSpace(key);
        }

        #endregion

        #region Public Properties

        public Guid SnapshotKey { get; set; }

        public Guid? ParentStorageSpaceKey { get; set; }

        public StorageSpaceTypeInternalCode StorageSpaceTypeInternalCode { get; set; }

        public Guid? StorageSpaceFormKey { get; set; }

        public string StorageSpaceName { get; set; }

        public string ProductName { get; set; }

        public string SerialID { get; set; }

        public string StackedSerialID { get; set; }

        public string RegistrySerialID { get; set; }

        public string PositionID { get; set; }

        public Guid? AnchorStrorageSpaceKey { get; set; }

        public Guid? FacilityKey { get; set; }

        public Guid? SystemBusDeviceKey { get; set; }

        public Guid? SecondSystemBusDeviceKey { get; set; }

        public MiniDrawerTrayModeInternalCode? MiniDrawerTrayMode { get; set; }

        public MiniDrawerTrayModeInternalCode? PendingMiniDrawerTrayMode { get; set; }

        public StorageSpaceFormInternalCode? StorageSpaceFormInternalCode{ get; set; }

        public string StorageSpaceFormDescriptionText { get; set; }

        public StorageSpaceState StorageSpaceState { get; set; }

        public string ShortName { get; set; }

        public string AbbreviatedName { get; set; }

        public string DisplayName { get; set; }

        public string SortValue { get; set; }

        public Guid? DispensingDeviceKey { get; set; }

        public byte? Level { get; set; }

        public MiniDrawerTrayModeInternalCode? ParentMiniDrawerTrayModeInternalCode { get; internal set; }

        public MiniDrawerTrayModeInternalCode? ParentPendingMiniDrawerTrayModeInternalCode { get; internal set; }

        public bool MoreThanOneItem { get; set; }

        public bool UnavailableForInventory { get; set; }

        public bool Mobile { get; set; }

        public long? ShipToID { get; set; }

        public InteriorCabinetLightIntensityModeInternalCode? InteriorCabinetLightIntensityModeInternalCode { get; set; }

        public bool FailureInHierarchy { get; set; }

        public bool DefrostInHierarchy { get; set; }
        public bool RestrictedAccess { get; set; }

  #endregion

  #region Dervied Properties

  public bool IsCubiePocket()
        {
            return StorageSpaceTypeInternalCode.IsCubiePocket();
        }

        public bool HasPhysicalMax()
        {
            return StorageSpaceTypeInternalCode.HasPhysicalMax();
        }

        public bool IsDrawer()
        {
            return StorageSpaceTypeInternalCode.IsDrawer();
        }

        public bool IsDoor()
        {
            return StorageSpaceTypeInternalCode.IsDoor();
        }

        /// <summary>
        /// Determines if the storage space is accessible via hardware.
        /// </summary>
        /// <returns></returns>
        public bool IsHardwareAccessible()
        {
            return StorageSpaceTypeInternalCode.IsHardwareAccessible();
        }

        /// <summary>
        /// Determines if the storage space is directly accessible via hardware
        /// (i.e. it is something that the HAL/Bus has knowledge about and forms part of the access path)
        /// </summary>
        /// <example>
        /// Cubie drawers/pockets, Matrix drawers, Mini drawer/tray/pockets, Doors, SRM, Refrigerator Cabinet
        /// </example>
        public bool IsHardwareAccessibleDirectly()
        {
            return StorageSpaceTypeInternalCode.IsHardwareAccessibleDirectly();
        }

        /// <summary>
        /// Determines if the storage space is indirectly accessible via hardware
        /// (i.e. it is something that the HAL/Bus doesnt have knowledge about but is contained in an accessible piece of hardware)
        /// </summary>
        /// <example>
        /// Matrix pockets, SRM pockets, Tall Aux pockets
        /// </example>
        /// <returns></returns>
        public bool IsHardwareAccessibleIndirectly()
        {
            return StorageSpaceTypeInternalCode.IsHardwareAccessibleIndirectly();
        }

        /// <summary>
        /// Determines if the storage space's state can be updated.
        /// Only certain storage spaces have state records.
        /// </summary>
        /// <returns></returns>
        public bool CanUpdateState()
        {
            return StorageSpaceTypeInternalCode.CanUpdateState();
        }

        /// <summary>
        /// Determines if the storage space's parent state needs to be updated.
        /// </summary>
        /// <returns></returns>
        public bool NeedToUpdateParentState()
        {
            return StorageSpaceTypeInternalCode.NeedToUpdateParentState();
        }

        /// <summary>
        /// Determines if the storage space can be failed.
        /// Only certain storage spaces have state records that can be marked as failed.
        /// </summary>
        /// <returns></returns>
        public bool CanFail()
        {
            return StorageSpaceTypeInternalCode.CanFail();
        }

        /// <summary>
        /// Determines if the storage space's parent needs to be failed.
        /// </summary>
        /// <returns></returns>
        public bool NeedToFailParent()
        {
            return StorageSpaceTypeInternalCode.NeedToFailParent();
        }

        /// <summary>
        /// Determines if a failure condition causes failures to
        /// cascade to a storage space's parent.
        /// </summary>
        /// <param name="failureReason"></param>
        /// <returns></returns>
        public bool CascadeFailureToParent(StorageSpaceFailureReasonInternalCode failureReason)
        {
            return StorageSpaceTypeInternalCode.CascadeFailureToParent(failureReason);
        }

        #endregion
    }
}
