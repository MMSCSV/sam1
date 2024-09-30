using System;
using System.Collections.Generic;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data
{
    public interface IStorageRepository : IRepository
    {
        #region Dispensing Device Members

        /// <summary>
        /// Retrieves a collection of <see cref="DispensingDevice"/> by key.
        /// </summary>
        /// <param name="dispensingDeviceKeys">The collection of item keys or NULL for all.</param>
        /// <param name="deleteFlag"></param>
        /// <param name="computerName"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="DispensingDevice"/>.</returns>
        IEnumerable<DispensingDevice> GetDispensingDevices(IEnumerable<Guid> dispensingDeviceKeys = null, bool? deleteFlag = null, string computerName = null);

        /// <summary>
        /// Return a dispensing device object using the computer name of the station.
        /// </summary>
        /// <param name="computerName"></param>
        /// <returns></returns>
        DispensingDevice GetDispensingDevice(string computerName);

        /// <summary>
        /// Return a DispensingDevice record using its primary key.
        /// </summary>
        /// <param name="dispensingDeviceKey"></param>
        /// <returns></returns>
        DispensingDevice GetDispensingDevice(Guid dispensingDeviceKey);

        /// <summary>
        /// Insert a new record into the DispensingDevice and DispensingDeviceSnapshot table.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dispensingDevice"></param>
        /// <returns></returns>
        Guid InsertDispensingDevice(Context context, DispensingDevice dispensingDevice);

        /// <summary>
        /// Update a record in the dispensing device snapshot table.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dispensingDevice"></param>
        void UpdateDispensingDevice(Context context, DispensingDevice dispensingDevice);

        /// <summary>
        /// Flag a dispensing device record as deleted.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dispensingDeviceKey"></param>
        void DeleteDispensingDevice(Context context, Guid dispensingDeviceKey);

        /// <summary>
        /// Logically deletes all critical override periods of devices within the specified facility.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="facilityKey"></param>
        void DeleteDispensingDeviceCriticalOverridePeriods(Context context, Guid facilityKey);

        /// <summary>
        /// Get the serial id of device
        /// </summary>
        /// <param name="dispensingDeviceKey"></param>
        string GetDispensingDeviceSerialId(Guid dispensingDeviceKey);

        #endregion

        #region VirtualStockLocation Members

        /// <summary>
        /// Retrieves a collection of <see cref="VirtualStockLocation"/> by key.
        /// </summary>
        /// <param name="virutalStockLocationKeys">The collection of virtual location keys or NULL for all.</param>
        /// <param name="deleted"></param>
        /// <param name="facilityKey"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="VirtualStockLocation"/>.</returns>
        IEnumerable<VirtualStockLocation> GetVirtualStockLocations(IEnumerable<Guid> virutalStockLocationKeys = null, bool? deleted = null, Guid? facilityKey = null);

        /// <summary>
        /// Gets a <see cref="VirtualStockLocation"/> by its surrogate key.
        /// </summary>
        /// <param name="virtualStockLocationKey">The surrogate key of the <see cref="VirtualStockLocation"/>.</param>
        /// <returns>The <see cref="VirtualStockLocation"/> if found, otherwise null.</returns>
        VirtualStockLocation GetVirtualStockLocation(Guid virtualStockLocationKey);

        /// <summary>
        /// Inserts an <see cref="VirtualStockLocation"/> into the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="virtualStockLocation">The virtual stock location.</param>
        /// <returns>A zone key, which uniquely identifies the zone in the database.</returns>
        Guid InsertVirtualStockLocation(Context context, VirtualStockLocation virtualStockLocation);

        /// <summary>
        /// Updates an existing <see cref="VirtualStockLocation"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="virtualStockLocation">The virtual stock location.</param>
        void UpdateVirtualStockLocation(Context context, VirtualStockLocation virtualStockLocation);

        /// <summary>
        /// Logically deletes an existing <see cref="VirtualStockLocation"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="virtualStockLocationKey">The virtual stock location key.</param>
        void DeleteVirtualStockLocation(Context context, Guid virtualStockLocationKey);

        #endregion

        #region Zone Members

        /// <summary>
        /// Retrieves a collection of <see cref="Zone"/> by key.
        /// </summary>
        /// <param name="zoneKeys">The collection of zone keys or NULL for all.</param>
        /// <param name="deleted"></param>
        /// <param name="facilityKey"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="Zone"/>.</returns>
        IEnumerable<Zone> GetZones(IEnumerable<Guid> zoneKeys = null, bool? deleted = null, Guid? facilityKey = null);

        /// <summary>
        /// Gets a <see cref="Zone"/> by its surrogate key.
        /// </summary>
        /// <param name="zoneKey">The surrogate key of the <see cref="Zone"/>.</param>
        /// <returns>The <see cref="Zone"/> if found, otherwise null.</returns>
        Zone GetZone(Guid zoneKey);

        /// <summary>
        /// Inserts an <see cref="Zone"/> into the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="zone">The zone.</param>
        /// <returns>A zone key, which uniquely identifies the zone in the database.</returns>
        Guid InsertZone(Context context, Zone zone);

        /// <summary>
        /// Updates an existing <see cref="Zone"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="zone">The zone.</param>
        void UpdateZone(Context context, Zone zone);

        /// <summary>
        /// Logically deletes an existing <see cref="Zone"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="zoneKey">The zone key.</param>
        void DeleteZone(Context context, Guid zoneKey);

        #endregion

        #region SystemBusDevice Members

        /// <summary>
        /// Return a SystemBudDevice record from its key.
        /// </summary>
        /// <param name="systemBusDeviceKey"></param>
        /// <returns></returns>
        SystemBusDevice GetSystemBusDevice(Guid systemBusDeviceKey);

        /// <summary>
        /// Return the SystemBusDevice object using the given serial number.
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <returns></returns>
        IEnumerable<SystemBusDevice> GetSystemBusDevices(long deviceNumber);
       
        /// <summary>
        /// Insert a new record into the SystemBusDevice and SystemBusDeviceSnaphot tables.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="systemBusDevice"></param>
        /// <returns></returns>
        Guid InsertSystemBusDevice(Context context, SystemBusDevice systemBusDevice);

        /// <summary>
        /// Update the given system bus device record.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="systemBusDevice"></param>
        void UpdateSystemBusDevice(Context context, SystemBusDevice systemBusDevice);

        /// <summary>
        /// Flag the given system bus device record as deleted.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="systemBusDeviceKey"></param>
        void DeleteSystemBusDevice(Context context, Guid systemBusDeviceKey);

        #endregion

        #region DispensingDeviceSystemBusDevice Members

        /// <summary>
        /// Inserts a new dispensing device and system bus device association.
        /// </summary>
        void InsertDispensingDeviceSystemBusDevice(Context context, Guid dispensingDeviceKey, Guid systemBusDeviceKey);

        /// <summary>
        /// Deletes a dispensing device to system bus device association.
        /// </summary>
        void DeleteDispensingDeviceSystemBusDevice(Context context, Guid dispensingDeviceKey, Guid systemBusDeviceKey);

        #endregion

        #region DispensingDeviceStorageSpace Members

        /// <summary>
        /// Inserts a new dispensing device and storage space association.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dispensingDeviceKey"></param>
        /// <param name="storageSpaceKey"></param>
        /// <returns></returns>
        void InsertDispensingDeviceStorageSpace(Context context, Guid dispensingDeviceKey, Guid storageSpaceKey);

        /// <summary>
        /// Deletes a dispensing device to storage space association.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dispensingDeviceKey"></param>
        /// <param name="storageSpaceKey"></param>
        void DeleteDispensingDeviceStorageSpace(Context context, Guid dispensingDeviceKey, Guid storageSpaceKey);

        #endregion

        #region StorageSpace Members

        /// <summary>
        /// Return a StorageSpace record using its primary key.
        /// </summary>
        /// <param name="storageSpaceKey"></param>
        /// <returns></returns>
        StorageSpace GetStorageSpace(Guid storageSpaceKey);

        /// <summary>
        /// Return a StorageSpace record for the given system bus device key.
        /// </summary>
        /// <param name="systemBusDeviceKey"></param>
        /// <returns></returns>
        StorageSpace GetStorageSpaceBySystemBusDeviceKey(Guid systemBusDeviceKey);

        /// <summary>
        /// Gets a list of <see cref="StorageSpace"/> based on the device number and storage space type.
        /// </summary>
        /// <param name="deviceNumber"></param>
        /// <param name="storageSpaceType"></param>
        /// <returns></returns>
        IEnumerable<StorageSpace> GetStorageSpaces(long deviceNumber, StorageSpaceTypeInternalCode storageSpaceType);

        /// <summary>
        /// Gets a list of immediate children storage spaces for a given storage space key
        /// </summary>
        /// <param name="storageSpaceKey"></param>
        /// <returns></returns>
        IEnumerable<StorageSpace> GetChildrenStorageSpaces(Guid storageSpaceKey);

        /// <summary>
        ///  Insert a new records into the StorageSpace and StorageSpaceState tables.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="storageSpace"></param>
        /// <returns></returns>
        Guid InsertStorageSpace(Context context, StorageSpace storageSpace);

        /// <summary>
        /// Update the given storage space record.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="storageSpace"></param>
        void UpdateStorageSpace(Context context, StorageSpace storageSpace);

        /// <summary>
        /// Updates the derived values of the given storage space and all descendant storage spaces. This includes
        /// the short name, abbreviated name, sort value, and level.
        /// </summary>
        void UpdateStorageSpaceDerivedValues(Context context, Guid storageSpaceKey);

        /// <summary>
        /// Updates the mini trays and mini pockets pending and current mini drawer tray mode
        /// </summary>
        void UpdateStorageSpaceMiniDrawerTrayMode(Context context, Guid trayStorageSpaceKey,
            MiniDrawerTrayModeInternalCode? pendingMiniDrawerTrayMode,
            MiniDrawerTrayModeInternalCode? miniDrawerTrayMode);

        /// <summary>
        /// Updates the anchor information for the storage space.
        /// </summary>
        void UpdateStorageSpaceAnchor(Context context, Guid storageSpaceKey, Guid? anchorStorageSpaceKey, bool unavailableForInventory);

        /// <summary>
        /// Flag a storage space as deleted.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="storageSpaceKey"></param>
        void DeleteStorageSpace(Context context, Guid storageSpaceKey);

        #endregion

        #region StorageSpaceState Members

        /// <summary>
        /// Return the given StorageSpaceState object using the surrogate key.
        /// </summary>
        /// <param name="storageSpaceStateKey"></param>
        /// <returns></returns>
        StorageSpaceState GetStorageSpaceState(Guid storageSpaceStateKey);

        /// <summary>
        /// Return the given StorageSpaceState object using the storage space key.
        /// </summary>
        /// <param name="storageSpaceKey"></param>
        /// <returns></returns>
        StorageSpaceState GetStorageSpaceStateByStorageSpace(Guid storageSpaceKey);

        /// <summary>
        /// Inserts the given storage space state record.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="storageSpaceState"></param>
        Guid InsertStorageSpaceState(Context context, StorageSpaceState storageSpaceState);

        /// <summary>
        /// Update the given storage space state record.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="storageSpaceState"></param>
        void UpdateStorageSpaceState(Context context, StorageSpaceState storageSpaceState);

        #endregion

        #region StorageSpaceItem Members

        /// <summary>
        /// Return the StorageSpaceItem and StorageSpaceInventory objects by their key.
        /// </summary>
        /// <param name="storageSpaceItemKey"></param>
        /// <returns></returns>
        StorageSpaceItem GetStorageSpaceItem(Guid storageSpaceItemKey);

        /// <summary>
        /// Load or pend an item to a storage space.
        /// </summary>
        Guid InsertStorageSpaceItem(Context context, StorageSpaceItem storageSpaceItem);

        /// <summary>
        /// Inserts a storage space item that is immediately dissasociated (i.e. for documentation purposes only)
        /// </summary>
        Guid InsertDissassociatedStorageSpaceItem(Context context, StorageSpaceItem storageSpaceItem);

        /// <summary>
        /// Update a storage space item.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="storageSpaceItem"></param>
        /// <returns></returns>
        void UpdateStorageSpaceItem(Context context, StorageSpaceItem storageSpaceItem);

        /// <summary>
        /// Updates a storage space items last dispensing activity
        /// </summary>
        void UpdateStorageSpaceItemDispenseActivity(Context context, Guid storageSpaceItemKey, DateTime activityLocalDateTime, DateTime activityUtcDateTime);

        /// <summary>
        /// Updates a storage space items last dispensing activity
        /// </summary>
        void UpdateStorageSpaceItemDispenseActivity(Guid? deviceKey, Guid storageSpaceItemKey, DateTime activityLocalDateTime, DateTime activityUtcDateTime);

        /// <summary>
        /// Updates a storage space items last inventory activity
        /// </summary>
        void UpdateStorageSpaceItemInventoryActivity(Context context, Guid storageSpaceItemKey, DateTime activityLocalDateTime, DateTime activityUtcDateTime);

        /// <summary>
        /// Updates a storage space items last load/refill activity
        /// </summary>
        void UpdateStorageSpaceItemLoadRefillActivity(Context context, Guid storageSpaceItemKey, bool isLoadTransaction, DateTime activityLocalDateTime, DateTime activityUtcDateTime, bool loadRefillSuccessfulScan = false,
            DateTime? lastLoadrefillLocalDateTime = null, DateTime? lastLoadrefillUtcDateTime = null, DateTime? lastLoadLocalDateTime = null, DateTime? lastLoadUtcDateTime = null);

        /// <summary>
        /// Updates the DispenseFractional flag for all StorageSpaceItems at the given device for the given item
        /// </summary>
        void UpdateStorageSpaceItemsFractionalFlag(Context context, Guid dispensingDeviceKey, Guid itemKey, bool dispenseFractional);

        /// <summary>
        /// Updates a storage space items last load/refill RxCheck activity
        /// </summary>
        void UpdateStorageSpaceItemRxCheckLoadRefillActivity(Context context, Guid storageSpaceItemKey, DateTime rxCheckLocalDateTime, DateTime rxCheckUtcDateTime);

        /// <summary>
        /// Delete the storage space item by keys.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="storageSpaceKey"></param>
        /// <param name="itemKey"></param>
        /// <param name="dispensingDeviceEjectWithInventoryFlag"></param>
        void DeleteStorageSpaceItem(Context context, Guid storageSpaceKey, Guid itemKey, bool dispensingDeviceEjectWithInventoryFlag = false);

        #endregion

        #region CriticalOverrideMode Members

        /// <summary>
        /// Inserts a new association between a dispensing device and a critical override reason.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dispensingDeviceKey"> </param>
        /// <param name="criticalOverrideReason"> </param>
        void InsertCriticalOverrideMode(Context context, Guid dispensingDeviceKey, CriticalOverrideModeReasonInternalCode criticalOverrideReason);

        /// <summary>
        /// Deletes an existing association between a dispensing device and a critical override mode reason.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dispensingDeviceKey"> </param>
        /// <param name="criticalOverrideReason"> </param>
        void DeleteCriticalOverrideMode(Context context, Guid dispensingDeviceKey, CriticalOverrideModeReasonInternalCode criticalOverrideReason);

        #endregion
    }
}
