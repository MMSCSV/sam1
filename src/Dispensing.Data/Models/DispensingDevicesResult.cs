using System;
using System.Data.Linq;

namespace CareFusion.Dispensing.Data.Models
{
    public class DispensingDevicesResult
    {
        public Guid DispensingDeviceKey { get; set; }

        public Guid DispensingDeviceSnapshotKey { get; set; }

        public Nullable<DateTime> LastServerCommunicationUTCDateTime { get; set; }

        public Nullable<DateTime> LastServerCommunicationLocalDateTime { get; set; }

        public Nullable<DateTime> SyncUploadFailureUTCDateTime { get; set; }

        public Nullable<DateTime> SyncUploadFailureLocalDateTime { get; set; }

        public Nullable<DateTime> SyncDownloadFailureUTCDateTime { get; set; }

        public Nullable<DateTime> SyncDownloadFailureLocalDateTime { get; set; }

        public Nullable<DateTime> SyncInRetryModeUTCDateTime { get; set; }

        public Nullable<DateTime> SyncInRetryModeLocalDateTime { get; set; }

        public Binary LastDownloadTickValue { get; set; }

        public Binary LastUploadTickValue { get; set; }

        public Nullable<long> LastUploadChangeTrackingValue { get; set; }

        public Nullable<long> LastDownloadChangeTrackingValue { get; set; }

        public Nullable<DateTime> LastSuccessfulUploadUTCDateTime { get; set; }

        public Nullable<DateTime> LastSuccessfulUploadLocalDateTime { get; set; }

        public Nullable<DateTime> LastSuccessfulDownloadUTCDateTime { get; set; }

        public Nullable<DateTime> LastSuccessfulDownloadLocalDateTime { get; set; }

        public string IPAddressValue { get; set; }

        public string SyncDownloadStatusInternalCode { get; set; }

        public Nullable<DateTime> LastFullSyncUTCDateTime { get; set; }

        public Nullable<DateTime> LastFullSyncLocalDateTime { get; set; }

        public Guid FacilityKey { get; set; }

        public string FacilityCode { get; set; }

        public string FacilityName { get; set; }

        public string DispensingDeviceName { get; set; }

        public string DispensingDeviceTypeInternalCode { get; set; }

        public string ComputerName { get; set; }

        public string ServerAddressValue { get; set; }

        public Nullable<Guid> SyncServerKey { get; set; }

        public string AuthenticationMethodInternalCode { get; set; }

        public string BioIDExemptInternalCode { get; set; }

        public string CardExemptInternalCode { get; set; }

        public string UserScanModeInternalCode { get; set; }

        public Nullable<Guid> ZoneKey { get; set; }

        public bool OutOfServiceFlag { get; set; }

        public bool ProfileModeFlag { get; set; }

        public short MenuTimeOutDurationAmount { get; set; }

        public short OpenDrawerTimeOutDurationAmount { get; set; }

        public short EmptyReturnBinTimeOutDurationAmount { get; set; }

        public short InventoryDrawerMapTimeOutDurationAmount { get; set; }

        public short AnesthesiaTimeOutDurationAmount { get; set; }

        public short ReverificationTimeOutDurationAmount { get; set; }

        public bool AnesthesiaSwitchUserFlag { get; set; }

        public Nullable<short> AnesthesiaSwitchUserDurationAmount { get; set; }

        public short PatientCaseTransactionHoldDurationAmount { get; set; }

        public bool AccessInventoryFeatureOffFlag { get; set; }

        public bool TemporaryNonProfileModeFlag { get; set; }

        public Nullable<short> RemoveBeforeOrderStartDurationAmount { get; set; }

        public Nullable<short> RemoveAfterOrderExpiredDurationAmount { get; set; }

        public short BeforeMedDueNowDurationAmount { get; set; }

        public short AfterMedDueNowDurationAmount { get; set; }

        public bool OverrideReasonRequiredFlag { get; set; }

        public bool TooCloseWarningFlag { get; set; }

        public bool PatientSpecificFunctionalityFlag { get; set; }

        public bool WitnessOnDispenseFlag { get; set; }

        public bool WitnessOnReturnFlag { get; set; }

        public bool WitnessOnDisposeFlag { get; set; }

        public bool WitnessOnLoadRefillFlag { get; set; }

        public bool WitnessOnUnloadFlag { get; set; }

        public bool WitnessOnOverrideFlag { get; set; }

        public bool WitnessOnOutdateFlag { get; set; }

        public bool WitnessOnInventoryFlag { get; set; }

        public bool WitnessOnEmptyReturnBinFlag { get; set; }

        public bool WitnessOnDestockFlag { get; set; }

        public bool WitnessOnRxCheckFlag { get; set; }

        public bool ScanOnLoadRefillFlag { get; set; }

        public bool ScanOnRemoveFlag { get; set; }

        public bool ScanOnReturnFlag { get; set; }

        public bool RequireLotIDOnRemoveFlag { get; set; }

        public bool RequireLotIDOnReturnFlag { get; set; }

        public bool RequireSerialIDOnRemoveFlag { get; set; }

        public bool RequireSerialIDOnReturnFlag { get; set; }

        public bool RequireExpirationDateOnRemoveFlag { get; set; }

        public bool RequireExpirationDateOnReturnFlag { get; set; }

        public bool CriticalOverrideFlag { get; set; }

        public Nullable<short> AutoCriticalOverrideDurationAmount { get; set; }

        public bool BlindCountFlag { get; set; }

        public bool OutdateTrackingFlag { get; set; }

        public bool ReturnToStockFlag { get; set; }

        public short UpgradeTimeOfDayValue { get; set; }

        public bool ManualUpgradeFlag { get; set; }

        public bool DefaultGlobalPatientSearchFlag { get; set; }

        public bool PrintMedOutdateFlag { get; set; }

        public bool PrintMedUnloadFlag { get; set; }

        public bool PrintMedEmptyReturnBinFlag { get; set; }

        public bool PrintMedReturnFlag { get; set; }

        public bool PrintMedRemoveFlag { get; set; }

        public bool PrintMedDisposeFlag { get; set; }

        public bool PrintMedDestockFlag { get; set; }

        public bool PrintMedRxCheckFlag { get; set; }

        public bool PrintMedLoadRefillFlag { get; set; }

        public string CriticalLowNoticePrinterName { get; set; }

        public string StockOutNoticePrinterName { get; set; }

        public bool ShowPreadmissionFlag { get; set; }

        public bool ShowRecurringAdmissionFlag { get; set; }

        public short PreadmissionLeadDurationAmount { get; set; }

        public short PreadmissionProlongedInactivityDurationAmount { get; set; }

        public short AdmissionProlongedInactivityDurationAmount { get; set; }

        public short DischargeDelayDurationAmount { get; set; }

        public short TransferDelayDurationAmount { get; set; }

        public Nullable<Guid> ReplenishmentAreaKey { get; set; }

        public Nullable<Guid> VirtualStockLocationKey { get; set; }

        public bool RxCheckFlag { get; set; }

        public bool AutoMedLabelFlag { get; set; }

        public bool SyncUploadAllowSkipFlag { get; set; }

        public bool SyncAllowDownloadOnUploadFailureFlag { get; set; }

        public short SyncUploadMaximumRetryQuantity { get; set; }

        public short SyncUploadRetryIntervalAmount { get; set; }

        public bool WitnessOnCUBIEEjectFlag { get; set; }

        public string WasteModeInternalCode { get; set; }

        public short FutureTaskWarningDurationAmount { get; set; }

        public bool PrintPatientLabelRemoveFlag { get; set; }

        public bool PharmacyOrderDispenseQuantityFlag { get; set; }

        public string IdentityServerClientSecretValue { get; set; }

        public string IdentityServerClientID { get; set; }

        public string OneTimePasswordSecretKeyValue { get; set; }

        public short OneTimePasswordTimeoutDurationAmount { get; set; }

        public bool UseEquivalenciesFlag { get; set; }

        public bool GrabScanFlag { get; set; }

        public bool ReceiveBarcodeInventoryDecrementExternalFlag { get; set; }

        public string ExternalSystemDeviceHostValue { get; set; }

        public int? ExternalSystemDevicePortNumber { get; set; }

        public string ExternalSystemDeviceAdminUserPasswordValue { get; set; }

        public short? ExternalSystemDeviceCommandTimeoutDurationAmount { get; set; }

        public short? OpenBinTimeoutDurationAmount { get; set; }

        public int? BarcodeReceiverDevicePortNumber { get; set; }

        public Nullable<Guid> ControlledSubstanceLicenseKey { get; set; }

        public bool DisplayPatientPreferredNameFlag { get; set; }

        public short GCSMDestructionBinTimeOutDurationAmount { get; set; }

        public bool GCSMScanOnCompoundingFlag { get; set; }

        public bool GCSMScanOnIssueFlag { get; set; }

        public bool GCSMScanOnPrescriptionFlag { get; set; }

        public bool GCSMScanOnReturnFlag { get; set; }

        public bool GCSMScanOnReceiveFlag { get; set; }

        public bool GCSMScanOnRestockADMFlag { get; set; }

        public bool GCSMScanOnSellFlag { get; set; }

        public bool GCSMScanOnStockTransferFlag { get; set; }

        public bool GCSMWitnessOnCompoundingFlag { get; set; }

        public bool GCSMWitnessOnEmptyDestructionBinFlag { get; set; }

        public bool GCSMWitnessOnInventoryFlag { get; set; }

        public bool GCSMWitnessOnIssueFlag { get; set; }

        public bool GCSMWitnessOnOutdateFlag { get; set; } 

        public bool GCSMWitnessOnPrescriptionFlag { get; set; }

        public bool GCSMWitnessOnRecallFlag { get; set; }

        public bool GCSMWitnessOnReceiveFlag { get; set; }

        public bool GCSMWitnessOnRestockADMFlag { get; set; }

        public bool GCSMWitnessOnReturnFlag { get; set; }

        public bool GCSMWitnessOnReverseCompoundingFlag { get; set; }

        public bool GCSMWitnessOnSellFlag { get; set; }

        public bool GCSMWitnessOnStockTransferFlag { get; set; }

        public bool GCSMWitnessOnUnloadFlag { get; set; }

        public bool GCSMWitnessOnWasteFlag { get; set; }

        public bool GCSMWitnessOnceInventoryFlag { get; set; }

        public bool GCSMWitnessOnRecoverFlag { get; set; }
        public bool GCSMWitnessOnAccessToDestructionBinFlag { get; set; }
        public bool GCSMWitnessOnAddToDestructionBinFlag { get; set; }

        public bool GCSMBlindCountFlag { get; set; }

        public string GCSMSheetPrinterName { get; set; }

        public string GCSMLabelPrinterName { get; set; }

        public short LeaveOfAbsenceDelayDurationAmount { get; set; }

        public bool DeleteFlag { get; set; }

        public Binary LastModifiedBinaryValue { get; set; }

        public string ServerDatabaseVersionText { get; set; }

        public string ClientDatabaseVersionText { get; set; }

        public string SystemReleaseText { get; set; }

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

        public string GCSMCompareReportStandardRangeInternalCode { get; set; }

        public bool GCSMCompareReportViewFilterFocusedFlag { get; set; }

        public Nullable<Guid> GCSMOriginDispensingDeviceKey { get; set; }

        public Nullable<Guid> GCSMDestinationDispensingDeviceKey { get; set; }
    }
}
