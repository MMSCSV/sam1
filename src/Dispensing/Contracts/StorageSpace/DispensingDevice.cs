using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a device that is used for storing and dispensing items
    /// and that has a user interface for controlling the device.
    /// </summary>
    [Serializable]
    public class DispensingDevice : Entity<Guid>
    {
        #region Constructors

        public DispensingDevice()
        {
            UpgradeTimeOfDay = 180;
            InventoryDrawerMapTimeOutDuration = 300;
            AnesthesiaTimeOutDuration = 60;
            AnesthesiaSwitchUserDuration = null;
            ReverificationTimeOutDuration = 0;
            PreadmissionLeadDuration = 72;
            AdmissionProlongedInactivityDuration = 60;
            DischargeDelayDuration = 2;
            PreadmissionProlongedInactivityDuration = 72;
            TransferDelayDuration = 2;
            PatientCaseTransactionHoldDuration = 24;
            SyncUploadMaximumRetryQuantity = 75;
            SyncUploadRetryInterval = 60;
            OneTimePasswordTimeoutDurationAmount = ValidationConstants.DispensingDeviceOtpTimeoutDurationDefaultValue; // Valid time for the code in minutes
            ControlledSubstanceLicenseKey = null;
            GCSMDestructionBinTimeOutDurationAmount = ValidationConstants.DispensingDeviceOpenDrawerTimeoutDefaultValue;
            LeaveOfAbsenceDelayDurationAmount = ValidationConstants.LeaveOfAbsenceDelayDurationAmountDefaultValue;
            ExternalSystemDevicePortNumber = ValidationConstants.ExternalSystemDevicePortNumber;
            ExternalSystemDeviceCommandTimeoutDurationAmount = ValidationConstants.ExternalSystemDeviceCommandTimeoutDurationAmount;
            BarcodeReceiverDevicePortNumber = ValidationConstants.BarcodeReceiverDevicePortNumber;
            ReceiveBarcodeInventoryDecrementExternalFlag = ValidationConstants.ReceiveBarcodeInventoryDecrementExternalFlag;

        }

        public DispensingDevice(Guid key)
        {
            Key = key;
        }

        public DispensingDevice(Guid key, Guid snapshotKey)
        {
            Key = key;
            SnapshotKey = snapshotKey;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator DispensingDevice(Guid key)
        {
            return FromKey(key);
        }

        public static DispensingDevice FromKey(Guid key)
        {
            return new DispensingDevice(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a dispensing device snapshot.
        /// </summary>
        public Guid SnapshotKey { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        public Guid FacilityKey { get; set; }

        /// <summary>
        /// Gets the code of a facility.
        /// </summary>
        public string FacilityCode { get; internal set; }

        /// <summary>
        /// Gets the name of a facility.
        /// </summary>
        public string FacilityName { get; internal set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a dispensing device type.
        /// </summary>
        public DispensingDeviceTypeInternalCode DispensingDeviceType { get; set; }

        /// <summary>
        /// Gets or sets the name that identifies a dispensing device.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.DispensingDeviceNameUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_NameOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "Global_NameRequired")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the all-in-one computer name.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.DispensingDeviceComputerNameUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "DispensingDeviceComputerNameOutOfBounds")]
        public string ComputerName { get; set; }

        /// <summary>
        /// Gets or sets the IP address or DNS address name of a server.
        /// </summary>
        public string ServerAddress { get; set; }
        
        /// <summary>
        /// Gets or sets the surrogate key of a server that is the sync server for a dispensing device.
        /// </summary>
        public Guid? SyncServerKey { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies an authentication method.
        /// </summary>
        public AuthenticationMethodInternalCode AuthenticationMethod { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies an authentication method.
        /// </summary>
        public AuthenticationMethodInternalCode? BioIdExempt { get; set; }

        /// <summary>
        /// </summary>
        public AuthenticationMethodInternalCode? CardExempt { get; set;}

        /// <summary>
        /// Gets or sets the internal code that identifies a user scan mode.
        /// </summary>
        public UserScanModeInternalCode? UserScanMode { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a zone.
        /// </summary>
        public Guid? ZoneKey { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a dispensing device is out of service.
        /// </summary>
        public bool IsOutOfService { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a dispensing device uses pharmacy orders
        /// for the remove workflow.
        /// </summary>
        public bool IsProfileMode { get; set; }

        /// <summary>
        /// Gets or sets the duration (in seconds) until the menu times out.
        /// </summary>
        [DispensingRangeValidator(ValidationConstants.DispensingDeviceMenuTimeoutMinValue,
            ValidationConstants.DispensingDeviceMenuTimeoutMaxValue,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "DispensingDeviceMenuTimoutDurationOutOfBounds")]
        public short MenuTimeOutDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in seconds) until an open drawer times out.
        /// </summary>
        [DispensingRangeValidator(ValidationConstants.DispensingDeviceOpenDrawerTimeoutMinValue,
            ValidationConstants.DispensingDeviceOpenDrawerTimeoutMaxValue,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "DispensingDeviceOpenDrawerTimeoutDurationOutOfBounds")]
        public short OpenDrawerTimeOutDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in seconds) until an empty return bin times out.
        /// </summary>
        [DispensingRangeValidator(ValidationConstants.DispensingDeviceEmptyReturnBinTimeoutMinValue,
            ValidationConstants.DispensingDeviceEmptyReturnBinTimeoutMaxValue,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "DispensingDeviceEmptyReturnBinTimoutDurationOutOfBounds")]
        public short EmptyReturnBinTimeOutDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in seconds) until a dispensing device times out during inventory
        /// configuration involving a drawer's virtual map.
        /// </summary>
        public short InventoryDrawerMapTimeOutDuration { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether users at an anesthesia dispensing device may switch
        /// with accessible drawers left open.
        /// </summary>
        public bool AnesthesiaSwitchUser { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) until an anesthesia dispensing device times out during clinical
        /// workflows.
        /// </summary>
        public short AnesthesiaTimeOutDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) for patient case transaction hold.
        /// </summary>
        public short PatientCaseTransactionHoldDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) until an anesthesia dispensing device times out during 
        /// switch user.
        /// </summary>
        public short? AnesthesiaSwitchUserDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in seconds) until a dispensing device requires the signed-in user to
        /// re-authenticate when dispensing an item that requires reverification.
        /// </summary>
        public short ReverificationTimeOutDuration { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the access inventory feature
        /// is turned off.
        /// </summary>
        public bool IsAccessInventoryFeatureOff { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a dispensing device is temporarily
        /// in non-profile mode.
        /// </summary>
        public bool IsTemporarilyNonProfileMode { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) up to which an item may be removed before a pharmacy
        /// order task time.
        /// </summary>
        public short? RemoveBeforeOrderStartDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) up to which an item may be removed after a pharmacy
        /// order task time.
        /// </summary>
        public short? RemoveAfterOrderExpiredDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) before an order task is displayed in the Due Now window.
        /// </summary>
        public short BeforeMedicationDueNowDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) after an order task is displayed in the Due Now window.
        /// </summary>
        public short AfterMedicationDueNowDuration { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an override reason is required.
        /// </summary>
        public bool IsOverrideReasonRequired { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a too-close warning is required.
        /// </summary>
        public bool IsTooCloseWarningRequired { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether patient specific functionality is enabled.
        /// </summary>
        public bool IsPatientSpecificFunctionalityEnabled { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is witnessed on remove/take.
        /// </summary>
        public bool WitnessOnDispense { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is witnessed on return.
        /// </summary>
        public bool WitnessOnReturn { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is witnessed on waste/discard.
        /// </summary>
        public bool WitnessOnDispose { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is witnessed on load/refill.
        /// </summary>
        public bool WitnessOnLoadRefill { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is witnessed on unload.
        /// </summary>
        public bool WitnessOnUnload { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is witnessed on override.
        /// </summary>
        public bool WitnessOnOverride { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is witnessed on outdate.
        /// </summary>
        public bool WitnessOnOutdate { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is witnessed on inventory.
        /// </summary>
        public bool WitnessOnInventory { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether emptying a return bin is witnessed if the
        /// bin contains an item.
        /// </summary>
        public bool WitnessOnEmptyReturnBin { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is witnessed on destock.
        /// </summary>
        public bool WitnessOnDestock { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is witnessed on RxCheck.
        /// </summary>
        public bool WitnessOnRxCheck { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether items are scanned on load/refill.
        /// </summary>
        public bool ScanOnLoadRefill { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether items are scanned on remove.
        /// </summary>
        public bool ScanOnRemove { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether items are scanned on return.
        /// </summary>
        public bool ScanOnReturn { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a lot ID is required on remove.
        /// </summary>
        public bool RequireLotIdOnRemove { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a lot ID is required on return.
        /// </summary>
        public bool RequireLotIdOnReturn { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a serial ID is required on remove.
        /// </summary>
        public bool RequireSerialIdOnRemove { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a serial ID is required on return.
        /// </summary>
        public bool RequireSerialIdOnReturn { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an expiration date is required on remove.
        /// </summary>
        public bool RequireExpirationDateOnRemove { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an expiration date is required on return.
        /// </summary>
        public bool RequireExpirationDateOnReturn { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether critical override is enabled.
        /// </summary>
        public bool CriticalOverride { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) after which some downtime or delay is there
        /// an automatic switch to critical override.
        /// </summary>
        [DispensingRangeValidator(ValidationConstants.DispensingDeviceAutoCriticalOverrideDurationLowerBound, RangeBoundaryType.Inclusive,
            short.MaxValue, RangeBoundaryType.Ignore,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "DispensingDeviceAutoCriticalOverrideDurationOutOfBounds")]
        public short? AutoCriticalOverrideDuration { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a blind count occurs at a dispensing device.
        /// </summary>
        public bool IsBlindCount { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether outdate tracking is enabled.
        /// </summary>
        public bool OutdateTracking { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether items may be returned to stock.
        /// </summary>
        public bool ReturnToStock { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates time of day in minutes of when an upgrade starts.
        /// </summary>
        [DispensingRangeValidator(ValidationConstants.DispensingDeviceUpgradeTimeOfDayLowerBound,
            ValidationConstants.DispensingDeviceUpgradeTimeOfDayUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "DispensingDeviceUpgradeTimeOfDayOutOfBounds")]
        public short UpgradeTimeOfDay { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the software for a dispensing device is upgraded
        /// manually.
        /// </summary>
        public bool ManualUpgrade { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the Global Patient Search is displayed by default
        /// during dispense.
        /// </summary>
        public bool DefaultGlobalPatientSearch { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the system automatically prints a transaction slip after
        /// a successful medication outdate transaction.
        /// </summary>
        public bool PrintMedicationOutdateTransaction { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the system automatically prints a transaction slip after
        /// a successful medication unload transaction.
        /// </summary>
        public bool PrintMedicationUnloadTransaction { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the system automatically prints a transaction slip after
        /// a successful medication empty return bin transaction.
        /// </summary>
        public bool PrintMedicationEmptyReturnBinTransaction { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the system automatically prints a transaction slip after
        /// a successful medication return transaction.
        /// </summary>
        public bool PrintMedicationReturnTransaction { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the system automatically prints a transaction slip after
        /// a successful medication remove transaction.
        /// </summary>
        public bool PrintMedicationRemoveTransaction { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the system automatically prints after every successful
        /// medication waste transaction.
        /// </summary>
        public bool PrintMedicationDispose { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the system prints after every successful medication
        /// destock transaction.
        /// </summary>
        public bool PrintMedicationDestock { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the system prints after every successful medication
        /// RxCheck transaction with an adjusted quantity > 0.
        /// </summary>
        public bool PrintMedicationRxCheck { get; set; }

        ///<summary>
        /// Gets or sets the value that indicates whether the system prints after every successful medication
        /// load/refill transaction.
        /// </summary> 
        public bool PrintMedicationLoadRefill { get; set; }

        ///<summary>
        /// Gets or sets the value of PrintPatientLabelRemove
        /// </summary> 
        public bool PrintPatientLabelRemove { get; set; }

        /// <summary>
        /// Gets or sets the name of a printer for critical-low notices.
        /// </summary>
        public string CriticalLowNoticePrinterName { get; set; }

        /// <summary>
        /// Gets or sets the name of a printer for critical-low notices.
        /// </summary>
        public string StockOutNoticePrinterName { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether preadmissions are shown.
        /// </summary>
        public bool ShowPreadmission { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether recurring admissions are shown.
        /// </summary>
        public bool ShowRecurringAdmission { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) that is subtracted from a preadmit date time and that
        /// is used to determine whether an encounter is shown.
        /// </summary>
        public short? PreadmissionLeadDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) that is added to an expected admit date time and that
        /// is used to determine whether an encounter is shown.
        /// </summary>
        public short? PreadmissionProlongedInactivityDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in days) of inactivity after which an encounter is considered to be
        /// in a state of prolonged inactivity.
        /// </summary>
        public short? AdmissionProlongedInactivityDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) that is added to a discharge date time and that is used
        /// to determine whether dispensing activities may be performed against an encounter.
        /// </summary>
        public short? DischargeDelayDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) that is used to determine whether an encounter is
        /// associated with a dispensing device following unit or facility transfer.
        /// </summary>
        public short? TransferDelayDuration { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of an area that is used for replenishment purposes 
        /// (and integration with JIT).
        /// </summary>
        public Guid? ReplenishmentAreaKey { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a virtual stock location.
        /// </summary>
        public Guid? VirtualStockLocationKey { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a user needs to check that a load or refill
        /// by another user follows pharmacy policy.
        /// </summary>
        public bool RxCheck { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a dispensing device generates medication label.
        /// </summary>
        public bool AutoMedLabel { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether syn continues to upload to the server what data
        /// it can follow on error for a given dispensing device.
        /// </summary>
        public bool SyncUploadAllowSkip { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the sync continues to download data from the server
        /// to a dispensing device while sync upload failure state exists for that dispensing device.
        /// </summary>
        public bool SyncAllowDownloadOnUploadFailure { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of times the sync retries to upload some kind of data from a dispensing
        /// device to the server following errors when trying to store that kind of data at the server.
        /// </summary>
        public short SyncUploadMaximumRetryQuantity { get; set; }

        /// <summary>
        /// Gets or sets the amount of time (in seconds) until the first retry attempt occurs following a sync upload error,
        /// and the additional time added to the interval for each subsequent retry, so that the nth retry
        /// atttempt occurs n times the interval after the prior retry attempt.
        /// </summary>
        /// <example>If the interval is 10 seconds then the 7th retry occurs 70 seconds after teh 6th attempt.</example>
        public short SyncUploadRetryInterval { get; set; }

        public WasteModeInternalCode? WasteMode { get; set; }

        public short FutureTaskWarningDurationAmount { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether a CUBIE's inventory is witnessed when the CUBIE is ejected with inventory
        /// </summary>
        public bool WitnessOnCubieEject { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates when the last time the device communicated with the server
        /// </summary>
        public DateTime? LastServerCommunicationUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates when the last time the device communicated with the server
        /// </summary>
        public DateTime? LastServerCommunicationDateTime { get; set; }

        /// <summary>
        /// Gets the the UTC date and time of when a dispensing device last failed to upload data to the server. 
        /// </summary>
        public DateTime? SyncUploadFailureUtcDateTime { get; set; }

        /// <summary>
        /// Gets the the local date and time of when a dispensing device last failed to upload data to the server. 
        /// </summary>
        public DateTime? SyncUploadFailureDateTime { get; set; }

        /// <summary>
        /// Gets the the UTC date and time of when a dispensing device last failed to download data from the server. 
        /// </summary>
        public DateTime? SyncDownloadFailureUtcDateTime { get; set; }

        /// <summary>
        /// Gets the local date and time of when a dispensing device is in retry mode
        /// and cross the configured count for showing a notification at the device
        /// </summary>
        public DateTime? SyncInRetryModeDateTime { get; set; }

        /// <summary>
        /// Gets the UTC date and time of when a dispensing device is in retry mode
        /// and cross the configured count for showing a notification at the device
        /// </summary>
        public DateTime? SyncInRetryModeUtcDateTime { get; set; }

        /// <summary>
        /// Gets the the local date and time of when a dispensing device last failed to download data from the server. 
        /// </summary>
        public DateTime? SyncDownloadFailureDateTime { get; set; }

        public string ServerDatabaseVersion { get; internal set; }

        public string ClientDatabaseVersion { get; internal set; }

        public string SystemRelease { get; internal set; }

        /// <summary>
        /// Gets or sets the critical period overrides associated with a device.
        /// </summary>
        [ObjectCollectionValidator(typeof(CriticalOverridePeriod))]
        public CriticalOverridePeriod[] CriticalOverridePeriods { get; set; }

        /// <summary>
        /// Gets or sets the areas associated with a device.
        /// </summary>
        public Area[] Areas { get; set; }

        /// <summary>
        /// Gets or sets the override groups associated with a device.
        /// </summary>
        public OverrideGroup[] OverrideGroups { get; set; }

        /// <summary>
        /// Gets or sets the clinical data subjects associated with a device.
        /// </summary>
        public Guid[] ClinicalDataSubjects { get; set; }

        /// <summary>
        /// Gets or sets the facility kits associated with a device.
        /// </summary>
        public Guid[] FacilityKits { get; set; }

        /// <summary>
        /// Gets or sets the stat facility kits associated with a device.
        /// </summary>
        public Guid[] StatFacilityKits { get; set; }

        /// <summary>
        /// Gets or sets the Restock Groups associated with a device.
        /// </summary>
        public RestockGroup[] RestockGroups { get; set; }

        /// <summary>
        /// Gets the value that holds the last rowversion (last modified binary value) that has been downloaded
        /// </summary>
        public byte[] LastDownloadTickValue { get; set; }

        /// <summary>
        /// Gets the value that holds the last rowversion (last modified binary value) that has been uploaded
        /// </summary>
        public byte[] LastUploadTickValue { get; set; }

        ///<summary>
        /// Gets the value of the maximum change tracking version used in last upload to server
        /// </summary>
        public long? LastUploadChangeTrackingValue { get; set; }

        ///<summary>
        /// Gets the value of the maximum change tracking version used in last download to dispensing device
        /// </summary> 
        public long? LastDownloadChangeTrackingValue { get; set; }

        /// <summary>
        /// Gets or sets UTC date and time of when a dispensing device processed its last successful data upload from the server
        /// </summary>
        public DateTime? LastSuccessfulUploadUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time of when a dispensing device processed its last successful data upload from the server
        /// </summary>
        public DateTime? LastSuccessfulUploadDateTime { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time of when a dispensing device processed its last successful data download from the server
        /// </summary>
        public DateTime? LastSuccessfulDownloadUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time of when a dispensing device processed its last successful data download from the serve
        /// </summary>
        public DateTime? LastSuccessfulDownloadDateTime { get; set; }

        /// <summary>
        /// Gets or sets the dispensing device computer IP address value
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether dispense quantities on pharmacy order pertain to a dispensing device
        /// </summary>
        public bool PharmacyOrderDispenseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the value for the client secret
        /// </summary>
        public string IdentityServerClientSecretValue { get; set; }

        /// <summary>
        /// Gets or sets the value for the registered station name in IDS
        /// </summary>
        public string IdentityServerClientID { get; set; }

        /// <summary>
        /// Gets or sets the value for the secret key to generate a One-Time password
        /// </summary>
        public string OneTimePasswordSecretKeyValue { get; set; }

        /// <summary>
        /// Gets or sets the value for timeout a One-Time password in Minutes
        /// </summary>
        public short OneTimePasswordTimeoutDurationAmount { get; set; }

        /// <summary>
        /// Gets or sets the nonlocalizable code that identifies a sync download status
        /// </summary>
        public SyncDownloadStatusInternalCode? SyncDownloadStatus { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether equivalencies are used at a dispensing device and could be approved at either the PIS or facility levl
        /// </summary>
        public bool UseEquivalencies { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates grab and scan setting to decrement inventory upon each scan of a medication
        /// </summary>
        public bool IsGrabScan { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether Cubie Replacement By Med ID is supported at dispensing device 
        /// </summary>
        public bool CubieReplacementByMedID { get; set; }

        public bool ReceiveBarcodeInventoryDecrementExternalFlag { get; set; }

        public string ExternalSystemDeviceHostValue { get; set; }

        public int? ExternalSystemDevicePortNumber { get; set; }

        public string ExternalSystemDeviceAdminUserPasswordValue { get; set; }

        public short? ExternalSystemDeviceCommandTimeoutDurationAmount { get; set; }
        public short? OpenBinTimeoutDurationAmount { get; set; }

        public int? BarcodeReceiverDevicePortNumber { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a ControlledSubstanceLicense.
        /// </summary>
        public Guid? ControlledSubstanceLicenseKey { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether patients' preferred names are shown.
        /// </summary>
        public bool DisplayPatientPreferredNameFlag { get; set; }

        /// <summary>
        /// Gets or sets the value for timeout Accessing Destruction Bin in Minutes
        /// </summary>
        public short GCSMDestructionBinTimeOutDurationAmount { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether to Scan a Med during Compounding 
        /// </summary>
        public bool GCSMScanOnCompoundingFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether to Scan a Med during Issue
        /// </summary>
        public bool GCSMScanOnIssueFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether to Scan a Med during fulfilling a prescription
        /// </summary>
        public bool GCSMScanOnPrescriptionFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether to Scan a Med during Receive
        /// </summary>
        public bool GCSMScanOnReceiveFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether to Scan a Med during Restocking
        /// </summary>
        public bool GCSMScanOnRestockADMFlag { get; set; }

        public bool GCSMScanOnReturnFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether to Scan a Med during a Sell
        /// </summary>
        public bool GCSMScanOnSellFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether to Scan a Med during Stock Transfer
        /// </summary>
        public bool GCSMScanOnStockTransferFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether an item is witnessed on Compounding.
        /// </summary>
        public bool GCSMWitnessOnCompoundingFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether an item is witnessed on Emptying the Destruction bin.
        /// </summary>
        public bool GCSMWitnessOnEmptyDestructionBinFlag { get; set; }
        public bool GCSMWitnessOnInventoryFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether an item is witnessed on Issue. 
        /// </summary>
        public bool GCSMWitnessOnIssueFlag { get; set; }

        public bool GCSMWitnessOnOutdateFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether an item is witnessed on fulfilling a prescription
        /// </summary>
        public bool GCSMWitnessOnPrescriptionFlag { get; set; }

        public bool GCSMWitnessOnRecallFlag { get; set; }


        /// <summary>
        /// Gets or sets the flag that indicateswhether an item is witnessed on Receive.
        /// </summary>
        public bool GCSMWitnessOnReceiveFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether an item is witnessed on Restocking. 
        /// </summary>
        public bool GCSMWitnessOnRestockADMFlag { get; set; }

        public bool GCSMWitnessOnReturnFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether an item is witnessed on Compounding.
        /// </summary>
        public bool GCSMWitnessOnReverseCompoundingFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether an item is witnessed on Sell .
        /// </summary>
        public bool GCSMWitnessOnSellFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether an item is witnessed on Transfer.
        /// </summary>
        public bool GCSMWitnessOnStockTransferFlag { get; set; }

        public bool GCSMWitnessOnUnloadFlag { get; set; }

        public bool GCSMWitnessOnWasteFlag { get; set; }

        public bool GCSMWitnessOnceInventoryFlag { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is witnessed on RxCheck.
        /// </summary>
        public bool GCSMWitnessOnRecoverFlag { get; set; }
        public bool GCSMWitnessOnAccessToDestructionBinFlag { get; set; }
        public bool GCSMWitnessOnAddToDestructionBinFlag { get; set; }

        public bool GCSMBlindCountFlag { get; set; }

        public string GCSMSheetPrinterName { get; set; }

        public string GCSMLabelPrinterName { get; set; }

        public bool GCSMPrintReceiptOnAccessDestructionBinFlag { get; set; }

        public bool GCSMPrintReceiptOnDiscrepancyResolutionFlag { get; set; }

        public bool GCSMPrintReceiptOnEmptyDestructionBinFlag { get; set; }

        public bool GCSMPrintReceiptOnFillPrescriptionFlag { get; set; }

        public bool GCSMPrintReceiptOnManageStockFlag { get; set; }

        public bool GCSMPrintReceiptOnNonStandardCompoundRecordFlag { get; set; }

        public bool GCSMPrintReceiptOnOutdateFlag { get; set; }

        public bool GCSMPrintReceiptOnPendingStandardCompoundRecordFlag { get; set; }

        public bool GCSMPrintReceiptOnRecallFlag { get; set; }

        public bool GCSMPrintReceiptOnReceiveFlag { get; set; }

        public bool GCSMPrintReceiptOnReturnFlag { get; set; }

        public bool GCSMPrintReceiptOnReverseNonStandardCompoundFlag { get; set; } 

        public bool GCSMPrintReceiptOnStandardCompoundDispositionSummaryFlag { get; set; } 

        public bool GCSMPrintReceiptOnSellFlag { get; set; }

        public bool GCSMPrintReceiptOnUnloadFlag { get; set; }

        public bool GCSMPrintReceiptOnWasteFlag { get; set; }

        public bool GCSMPrintLabelOnAccessDestructionBinFlag { get; set; }

        public bool GCSMPrintLabelOnFillPrescriptionFlag { get; set; }

        public bool GCSMPrintLabelOnNonStandardCompoundIngredientFlag { get; set; } 

        public bool GCSMPrintLabelOnNonStandardCompoundMedFlag { get; set; } 

        public bool GCSMPrintLabelOnOutdateFlag { get; set; }

        public bool GCSMPrintLabelOnRecallFlag { get; set; }

        public bool GCSMPrintLabelOnReturnFlag { get; set; }

        public bool GCSMPrintLabelOnSellFlag { get; set; }

        public bool GCSMPrintLabelOnStandardCompoundMedFlag { get; set; } 

        public bool GCSMPrintLabelOnStandardCompoundIngredientFlag { get; set; } 

        public bool GCSMPrintLabelOnWasteFlag { get; set; }

        public bool GCSMPrintPullListOnAutorestockFlag { get; set; }

        public bool GCSMPrintPullListOnDispenseToLocationFlag { get; set; }

        public bool GCSMPrintPullListOnKitFlag { get; set; } 

        public bool GCSMPrintPullListOnKitComponentFlag { get; set; } 

        public bool GCSMPrintPullListOnManageExcessStockFlag { get; set; }

        public bool GCSMPrintPullListOnNonStandardCompoundFlag { get; set; } 

        public bool GCSMPrintPullListOnNonStandardCompoundIngredientFlag { get; set; } 

        public bool GCSMPrintPullListOnStandardCompoundFlag { get; set; } 

        public bool GCSMPrintPullListOnStandardCompoundIngredientFlag { get; set; }

        public bool GCSMHideAreaFilterFlag { get; set; }

        public bool GCSMHideZoneFilterFlag { get; set; }

        public GCSMCompareReportStandardRangeInternalCode? GCSMCompareReportStandardRange { get; set; }

        public bool GCSMCompareReportViewFilterFocusedFlag { get; set; }

        /// <summary>
        /// Gets or sets the the number of hours when the patient is available at a device's local list for dispensing activities even after the patient has been placed on a LOA
        /// </summary>
        
        public Guid? GCSMOriginDispensingDeviceKey { get; set; }

        public Guid? GCSMDestinationDispensingDeviceKey { get; set; }

        public short? LeaveOfAbsenceDelayDurationAmount { get; set; }

        #endregion
    }
}
