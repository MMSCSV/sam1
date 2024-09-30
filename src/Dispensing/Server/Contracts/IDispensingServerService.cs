using System;
using System.Collections.Generic;
using System.ServiceModel;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Server.Contracts
{
    /// <summary>
    /// Provides a collection server services for devices.
    /// </summary>
    [ServiceContract(Namespace = "http://schemas.carefusion.com/2011/05/dispensing/services")]
    public interface IDispensingServerService
    {
        /// <summary>
        /// Returns a list of active authorized facilities for the specifed user context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>A IEnumerable(T) object, where the generic parameter T is <see cref="FacilityData"/>.</returns>
        [OperationContract]
        IEnumerable<FacilityData> GetFacilities(Context context);

        /// <summary>
        /// Obsolete in 1.7.0
        /// </summary>
        /// <param name="dispensingDeviceKey"></param>
        /// <returns></returns>
        [OperationContract]
        DispensingDeviceData GetDispensingDevice(Guid dispensingDeviceKey);

        /// <summary>
        /// Replacement of GetDispensingDevice(Guid dispensingDeviceKey) in 1.7.0
        /// </summary>
        /// <param name="dispensingDeviceKey"></param>
        /// <returns></returns>
        [OperationContract]
        DispensingDeviceData GetDispensingDeviceByDeviceKey(Guid dispensingDeviceKey);

        /// <summary>
        /// Obsolete in 1.7.0
        /// Returns a list of dispensing devices for the specified facility surrogate key.
        /// </summary>
        /// <param name="facilityKey">The facility surrogate key.</param>
        /// <returns>A IEnumerable(T) object, where the generic parameter T is <see cref="DispensingDeviceData"/>.</returns>
        [OperationContract(Name = "GetDispensingDevicesByFacility")]
        IEnumerable<DispensingDeviceData> GetDispensingDevices(Guid facilityKey);

        /// <summary>
        /// Replacement of GetDispensingDevices(Guid facilityKey) in 1.7.0
        /// Returns a list of dispensing devices for the specified facility surrogate key.
        /// </summary>
        /// <param name="facilityKey">The facility surrogate key.</param>
        /// <returns>A IEnumerable(T) object, where the generic parameter T is <see cref="DispensingDeviceData"/>.</returns>
        [OperationContract]
        IEnumerable<DispensingDeviceData> GetDispensingDevicesByFacilityKey(Guid facilityKey);

        /// <summary>
        /// Obsolete in 1.7.0
        /// Returns a list of dispensing devices for the specified facility surrogate key.
        /// </summary>
        /// <param name="facilityKey">The facility surrogate key.</param>
        /// <param name="dispensingDeviceType">The dispensing device type.</param>
        /// <returns>A IEnumerable(T) object, where the generic parameter T is <see cref="DispensingDeviceData"/>.</returns>
        [OperationContract(Name = "GetDispensingDevicesByFacilityAndType")]
        IEnumerable<DispensingDeviceData> GetDispensingDevices(Guid facilityKey, DispensingDeviceTypeInternalCode dispensingDeviceType);

        /// <summary>
        /// Replacement of GetDispensingDevices(Guid facilityKey, DispensingDeviceTypeInternalCode dispensingDeviceType)
        /// Returns a list of dispensing devices for the specified facility surrogate key.
        /// </summary>
        /// <param name="facilityKey">The facility surrogate key.</param>
        /// <param name="dispensingDeviceType">The dispensing device type.</param>
        /// <returns>A IEnumerable(T) object, where the generic parameter T is <see cref="DispensingDeviceData"/>.</returns>
        [OperationContract]
        IEnumerable<DispensingDeviceData> GetDispensingDevicesByFacilityKeyAndDeviceType(Guid facilityKey, DispensingDeviceTypeInternalCode dispensingDeviceType);

        /// <summary>
        /// Returns a <see cref="AuthenticationEventData"/> representing an authentication event.
        /// by the specified user.
        /// </summary>
        /// <param name="userAccountKey">The user account surrogate key.</param>
        /// <returns>A <see cref="AuthenticationEventData"/> object.</returns>
        [OperationContract]
        AuthenticationEventData GetRecentUserAccountSuccessfulAttempt(Guid userAccountKey);

        /// <summary>
        /// Returns a <see cref="RecentUserAccountAuthenticationAttemptData"/> representing last authentication attempts
        /// by the specified user.
        /// </summary>
        /// <param name="userAccountKey">The user account surrogate key.</param>
        /// <param name="ignoreAuthenticationEventKey"></param>
        /// <returns>A <see cref="RecentUserAccountAuthenticationAttemptData"/> object.</returns>
        [OperationContract]
        RecentUserAccountAuthenticationAttemptData GetRecentUserAccountAuthenticationAttempts(Guid userAccountKey, Guid ignoreAuthenticationEventKey);

        /// <summary>
        /// Obsolete in 1.7.0
        /// Assigns the specified dispensing device to a physical device.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="dispensingDeviceKey">The dispensing device surrogate key.</param>
        /// <param name="computerName">The computer name of the physical device.</param>
        /// <param name="serverAddress">The server address.</param>
        [OperationContract]
        void AssignDispensingDevice(Context context, Guid dispensingDeviceKey, string computerName, string serverAddress);

        /// <summary>
        /// Replacement of AssignDispensingDevice(Context context, Guid dispensingDeviceKey, string computerName, string serverAddress) in 1.7.0
        /// Assigns the specified dispensing device to a physical device.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="dispensingDeviceData">The dispensing device data record</param>
        [OperationContract]
        void UpdateDispensingDevice(Context context, DispensingDeviceData dispensingDeviceData);

        /// <summary>
        /// Validates device for auto full download.
        /// </summary>
        /// <param name="dispensingDeviceData">The dispensing device data record</param>
        [OperationContract]
        DispensingDeviceData ValidateDevice(DispensingDeviceData dispensingDeviceData);

        /// <summary>
        /// Gets the sync server url from system config snapshot.
        [OperationContract]
        string GetDataSyncServerUrl();

        /// <summary>
        /// Returns the list of areas, associated devices, and inventory quantity for given items
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="itemKeys">The items for which to return inventory info</param>
        /// <param name="currentDeviceKey">The device from which to start (to match facility, settings, etc)</param>
        /// <param name="includeProfileAndNonProfile">Flag to indicate whether to include both profile and non-profile devices</param>
        /// <returns>List of devices and inventory for corresponding item</returns>
        [OperationContract]
        IEnumerable<DispensingDeviceItemInventoryData> ListDispensingDeviceItemInventory(Context context, IEnumerable<Guid> itemKeys, Guid currentDeviceKey, bool includeProfileAndNonProfile);
        /// <summary>
        /// If .NET implementation doesnot give the expected result, then we will be using the CRYPT32.DLL for validating the revoked state
        /// </summary>
        /// <param name="context"></param>
        /// <param name="encodedCertificate"></param>
        /// <returns></returns>
        [OperationContract]
        string IsCertificateRevoked_WinAPI(Context context,string encodedCertificate);

        [OperationContract]
        string IsCertificateRevoked(Context context, string encodedCertificate);

        /// <summary>
        /// register domain user account 
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="facilityKey">Facility Key</param>
        /// <param name="localUserAccount">The local user account</param>
        /// <param name="domainUserAccount">The domain user account</param>
        /// <returns>registered domain user account</returns>
        [OperationContract]
        UserMergeStatus RegisterDomainUserAccount(Context context, Guid localUserKey, Guid domainUserKey);

        /// <summary>
        /// Creates a Pick Item Delivery records with ready for check status.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userAccountKey">Local user account.</param>
        /// <param name="pickItemDeliveryData">Pick data to create the Item Delivery records.</param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<PickItemDeliveryReferenceData> CreateDispensingDevicePickItemDeliveryRecord(Context context, Guid userAccountKey, IEnumerable<PickItemDeliveryData> pickItemDeliveryData);


        /// <summary>
        /// Barcode association requires Server interaction
        /// </summary>
        /// <param name="context"></param>
        /// <param name="itemKey">Item key.</param>
        /// <param name="barcodeScanData">New bar code scanned value.</param>
        /// <returns></returns>
        [OperationContract]
        BarcodeLinkResult LinkItemScancode(Context context, Guid itemKey, string barcodeScanData);

        /// <summary>
        /// Update dispensing device IdentityFlow to Custom from ResourceOwner
        /// </summary>
        /// <param name="context"></param>
        /// <param name="currentDeviceKey"></param>
        /// <returns></returns>
        [OperationContract]
        bool UpdateDispensingDeviceIdentityFlow(Context context, Guid currentDeviceKey);
    }
}
