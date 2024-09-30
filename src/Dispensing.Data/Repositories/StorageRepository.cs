using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data.Models;
using Dapper;
using Pyxis.Core.Data;
using Pyxis.Core.Data.InternalCodes;
using Pyxis.Core.Data.Schema.TableTypes;
using LocationDAL = Pyxis.Core.Data.Schema.Location;
using StrgDAL = Pyxis.Core.Data.Schema.Strg;
using CDCat = Pyxis.Core.Data.Schema.CDCat;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;

namespace CareFusion.Dispensing.Data.Repositories
{
    internal class StorageRepository : LinqBaseRepository, IStorageRepository
    {
        private readonly StrgDAL.ISystemBusDeviceRepository _systemBusDeviceRepository;
        private readonly StrgDAL.IDispensingDeviceRepository _dispensingDeviceRepository;
        private readonly StrgDAL.IStorageSpaceRepository _storageSpaceRepository;
        private readonly StrgDAL.IStorageSpaceStateRepository _storageSpaceStateRepository;
        private readonly CDCat.IClinicalDataSubjectRepository _clinicalDataSubjectRepository;
        private readonly StrgDAL.IStorageSpaceItemRepository _storageSpaceItemRepository;
        private readonly LocationDAL.IAreaRepository _areaRepository;
        private readonly StrgDAL.IVirtualStockLocationRepository _virtualStockLocationRepository;
        private readonly StrgDAL.IZoneRepository _zoneRepository;

        public StorageRepository()
        {
            _systemBusDeviceRepository = new StrgDAL.SystemBusDeviceRepository();
            _dispensingDeviceRepository = new StrgDAL.DispensingDeviceRepository();
            _storageSpaceRepository = new StrgDAL.StorageSpaceRepository();
            _storageSpaceStateRepository = new StrgDAL.StorageSpaceStateRepository();
            _clinicalDataSubjectRepository = new CDCat.ClinicalDataSubjectRepository();
            _storageSpaceItemRepository = new StrgDAL.StorageSpaceItemRepository();
            _areaRepository = new LocationDAL.AreaRepository();
            _virtualStockLocationRepository = new StrgDAL.VirtualStockLocationRepository();
            _zoneRepository = new StrgDAL.ZoneRepository();
        }

        #region Dispensing Device Members

        IEnumerable<DispensingDevice> IStorageRepository.GetDispensingDevices(IEnumerable<Guid> dispensingDeviceKeys, bool? deleteFlag, string computerName)
        {
            List<DispensingDevice> dispensingDevices = new List<DispensingDevice>();
            if (dispensingDeviceKeys != null && !dispensingDeviceKeys.Any())
                return dispensingDevices; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (dispensingDeviceKeys != null)
                    selectedKeys = new GuidKeyTable(dispensingDeviceKeys.Distinct());

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var multi = connectionScope.QueryMultiple(
                    "Strg.bsp_GetDispensingDevices",
                    new
                    {
                        SelectedKeys = selectedKeys.AsTableValuedParameter(),
                        ComputerName = computerName,
                        DeleteFlag = deleteFlag
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure);

                    var dispensingDeviceResults = multi.Read<DispensingDevicesResult>();
                    var criticalOverridePeriodResults = multi.Read<CriticalOverridePeriodResult>();
                    var areaResults = multi.Read<DispensingDeviceAreaResult>();
                    var overrideGroupResults = multi.Read<DispensingDeviceOverrideGroupResult>();
                    var clinicalDataSubjectResults = multi.Read<DispensingDeviceClinicalDataSubjectResult>();
                    var facilityKitResults = multi.Read<DispensingDeviceFacilityKitResult>();
                    var restockGroupResults = multi.Read<DispensingDeviceRestockGroupResult>();

                    foreach (var dispensingDeviceResult in dispensingDeviceResults)
                    {
                        DispensingDevice dispensingDevice =
                            new DispensingDevice(dispensingDeviceResult.DispensingDeviceKey,
                                                 dispensingDeviceResult.DispensingDeviceSnapshotKey)
                                {
                                    AdmissionProlongedInactivityDuration = dispensingDeviceResult.AdmissionProlongedInactivityDurationAmount,
                                    AfterMedicationDueNowDuration = dispensingDeviceResult.AfterMedDueNowDurationAmount,
                                    AnesthesiaTimeOutDuration = dispensingDeviceResult.AnesthesiaTimeOutDurationAmount,
                                    AnesthesiaSwitchUser = dispensingDeviceResult.AnesthesiaSwitchUserFlag,
                                    AnesthesiaSwitchUserDuration = dispensingDeviceResult.AnesthesiaSwitchUserDurationAmount,
                                    AutoCriticalOverrideDuration = dispensingDeviceResult.AutoCriticalOverrideDurationAmount,
                                    AuthenticationMethod = dispensingDeviceResult.AuthenticationMethodInternalCode.FromInternalCode<AuthenticationMethodInternalCode>(),
                                    AutoMedLabel = dispensingDeviceResult.AutoMedLabelFlag,
                                    BeforeMedicationDueNowDuration = dispensingDeviceResult.BeforeMedDueNowDurationAmount,
                                    BioIdExempt = dispensingDeviceResult.BioIDExemptInternalCode.FromNullableInternalCode<AuthenticationMethodInternalCode>(),
                                    CardExempt = dispensingDeviceResult.CardExemptInternalCode.FromNullableInternalCode<AuthenticationMethodInternalCode>(),
                                    ClientDatabaseVersion = dispensingDeviceResult.ClientDatabaseVersionText,
                                    ComputerName = dispensingDeviceResult.ComputerName,
                                    CriticalLowNoticePrinterName = dispensingDeviceResult.CriticalLowNoticePrinterName,
                                    CriticalOverride = dispensingDeviceResult.CriticalOverrideFlag,
                                    DefaultGlobalPatientSearch = dispensingDeviceResult.DefaultGlobalPatientSearchFlag,
                                    DischargeDelayDuration = dispensingDeviceResult.DischargeDelayDurationAmount,
                                    DispensingDeviceType = dispensingDeviceResult.DispensingDeviceTypeInternalCode.FromInternalCode<DispensingDeviceTypeInternalCode>(),
                                    EmptyReturnBinTimeOutDuration = dispensingDeviceResult.EmptyReturnBinTimeOutDurationAmount,
                                    FacilityKey = dispensingDeviceResult.FacilityKey,
                                    FacilityCode = dispensingDeviceResult.FacilityCode,
                                    FacilityName = dispensingDeviceResult.FacilityName,
                                    InventoryDrawerMapTimeOutDuration = dispensingDeviceResult.InventoryDrawerMapTimeOutDurationAmount,
                                    IsAccessInventoryFeatureOff = dispensingDeviceResult.AccessInventoryFeatureOffFlag,
                                    IsBlindCount = dispensingDeviceResult.BlindCountFlag,
                                    IsOutOfService = dispensingDeviceResult.OutOfServiceFlag,
                                    IsOverrideReasonRequired = dispensingDeviceResult.OverrideReasonRequiredFlag,
                                    IsPatientSpecificFunctionalityEnabled = dispensingDeviceResult.PatientSpecificFunctionalityFlag,
                                    IsProfileMode = dispensingDeviceResult.ProfileModeFlag,
                                    IsTemporarilyNonProfileMode = dispensingDeviceResult.TemporaryNonProfileModeFlag,
                                    IsTooCloseWarningRequired = dispensingDeviceResult.TooCloseWarningFlag,
                                    ManualUpgrade = dispensingDeviceResult.ManualUpgradeFlag,
                                    MenuTimeOutDuration = dispensingDeviceResult.MenuTimeOutDurationAmount,
                                    Name = dispensingDeviceResult.DispensingDeviceName,
                                    LastServerCommunicationUtcDateTime = dispensingDeviceResult.LastServerCommunicationUTCDateTime,
                                    LastServerCommunicationDateTime = dispensingDeviceResult.LastServerCommunicationLocalDateTime,
                                    OpenDrawerTimeOutDuration = dispensingDeviceResult.OpenDrawerTimeOutDurationAmount,
                                    OutdateTracking = dispensingDeviceResult.OutdateTrackingFlag,
                                    PatientCaseTransactionHoldDuration = dispensingDeviceResult.PatientCaseTransactionHoldDurationAmount,
                                    PreadmissionLeadDuration = dispensingDeviceResult.PreadmissionLeadDurationAmount,
                                    PreadmissionProlongedInactivityDuration = dispensingDeviceResult.PreadmissionProlongedInactivityDurationAmount,
                                    PrintMedicationEmptyReturnBinTransaction = dispensingDeviceResult.PrintMedEmptyReturnBinFlag,
                                    PrintMedicationOutdateTransaction = dispensingDeviceResult.PrintMedOutdateFlag,
                                    PrintMedicationRemoveTransaction = dispensingDeviceResult.PrintMedRemoveFlag,
                                    PrintMedicationDispose = dispensingDeviceResult.PrintMedDisposeFlag,
                                    PrintMedicationReturnTransaction = dispensingDeviceResult.PrintMedReturnFlag,
                                    PrintMedicationUnloadTransaction = dispensingDeviceResult.PrintMedUnloadFlag,
                                    PrintMedicationDestock = dispensingDeviceResult.PrintMedDestockFlag,
                                    PrintMedicationRxCheck = dispensingDeviceResult.PrintMedRxCheckFlag,
                                    PrintMedicationLoadRefill = dispensingDeviceResult.PrintMedLoadRefillFlag,
                                    PrintPatientLabelRemove = dispensingDeviceResult.PrintPatientLabelRemoveFlag,
                                    RemoveAfterOrderExpiredDuration = dispensingDeviceResult.RemoveAfterOrderExpiredDurationAmount,
                                    RemoveBeforeOrderStartDuration = dispensingDeviceResult.RemoveBeforeOrderStartDurationAmount,
                                    ReplenishmentAreaKey = dispensingDeviceResult.ReplenishmentAreaKey,
                                    ReturnToStock = dispensingDeviceResult.ReturnToStockFlag,
                                    ReverificationTimeOutDuration = dispensingDeviceResult.ReverificationTimeOutDurationAmount,
                                    ScanOnLoadRefill = dispensingDeviceResult.ScanOnLoadRefillFlag,
                                    ScanOnRemove = dispensingDeviceResult.ScanOnRemoveFlag,
                                    ScanOnReturn = dispensingDeviceResult.ScanOnReturnFlag,
                                    ServerDatabaseVersion = dispensingDeviceResult.ServerDatabaseVersionText,
                                    ServerAddress = dispensingDeviceResult.ServerAddressValue,
                                    SyncServerKey = dispensingDeviceResult.SyncServerKey,
                                    ShowPreadmission = dispensingDeviceResult.ShowPreadmissionFlag,
                                    ShowRecurringAdmission = dispensingDeviceResult.ShowRecurringAdmissionFlag,
                                    StockOutNoticePrinterName = dispensingDeviceResult.StockOutNoticePrinterName,
                                    SyncAllowDownloadOnUploadFailure = dispensingDeviceResult.SyncAllowDownloadOnUploadFailureFlag,
                                    SyncDownloadFailureDateTime = dispensingDeviceResult.SyncDownloadFailureLocalDateTime,
                                    SyncDownloadFailureUtcDateTime = dispensingDeviceResult.SyncDownloadFailureUTCDateTime,
                                    SyncUploadAllowSkip = dispensingDeviceResult.SyncUploadAllowSkipFlag,
                                    SyncUploadFailureDateTime = dispensingDeviceResult.SyncUploadFailureLocalDateTime,
                                    SyncUploadFailureUtcDateTime = dispensingDeviceResult.SyncUploadFailureUTCDateTime,
                                    SyncUploadMaximumRetryQuantity = dispensingDeviceResult.SyncUploadMaximumRetryQuantity,
                                    SyncUploadRetryInterval = dispensingDeviceResult.SyncUploadRetryIntervalAmount,
                                    SystemRelease = dispensingDeviceResult.SystemReleaseText,
                                    RequireLotIdOnRemove = dispensingDeviceResult.RequireLotIDOnRemoveFlag,
                                    RequireLotIdOnReturn = dispensingDeviceResult.RequireLotIDOnReturnFlag,
                                    RequireSerialIdOnRemove = dispensingDeviceResult.RequireSerialIDOnRemoveFlag,
                                    RequireSerialIdOnReturn = dispensingDeviceResult.RequireSerialIDOnReturnFlag,
                                    RequireExpirationDateOnRemove = dispensingDeviceResult.RequireExpirationDateOnRemoveFlag,
                                    RequireExpirationDateOnReturn = dispensingDeviceResult.RequireExpirationDateOnReturnFlag,
                                    RxCheck = dispensingDeviceResult.RxCheckFlag,
                                    TransferDelayDuration = dispensingDeviceResult.TransferDelayDurationAmount,
                                    LeaveOfAbsenceDelayDurationAmount = dispensingDeviceResult.LeaveOfAbsenceDelayDurationAmount,
                                    UpgradeTimeOfDay = dispensingDeviceResult.UpgradeTimeOfDayValue,
                                    UserScanMode = dispensingDeviceResult.UserScanModeInternalCode.FromNullableInternalCode<UserScanModeInternalCode>(),
                                    VirtualStockLocationKey = dispensingDeviceResult.VirtualStockLocationKey,
                                    WitnessOnDispense = dispensingDeviceResult.WitnessOnDispenseFlag,
                                    WitnessOnDispose = dispensingDeviceResult.WitnessOnDisposeFlag,
                                    WitnessOnEmptyReturnBin = dispensingDeviceResult.WitnessOnEmptyReturnBinFlag,
                                    WitnessOnInventory = dispensingDeviceResult.WitnessOnInventoryFlag,
                                    WitnessOnLoadRefill = dispensingDeviceResult.WitnessOnLoadRefillFlag,
                                    WitnessOnUnload = dispensingDeviceResult.WitnessOnUnloadFlag,
                                    WitnessOnOverride = dispensingDeviceResult.WitnessOnOverrideFlag,
                                    WitnessOnOutdate = dispensingDeviceResult.WitnessOnOutdateFlag,
                                    WitnessOnReturn = dispensingDeviceResult.WitnessOnReturnFlag,
                                    WitnessOnDestock = dispensingDeviceResult.WitnessOnDestockFlag,
                                    WitnessOnRxCheck = dispensingDeviceResult.WitnessOnRxCheckFlag,
                                    ZoneKey = dispensingDeviceResult.ZoneKey,
                                    LastDownloadTickValue = dispensingDeviceResult.LastDownloadTickValue != null?dispensingDeviceResult.LastDownloadTickValue.ToArray():null,
                                    LastUploadTickValue = dispensingDeviceResult.LastUploadTickValue != null? dispensingDeviceResult.LastUploadTickValue.ToArray():null,
                                    LastUploadChangeTrackingValue = dispensingDeviceResult.LastUploadChangeTrackingValue,
                                    LastDownloadChangeTrackingValue = dispensingDeviceResult.LastDownloadChangeTrackingValue,
                                    LastSuccessfulUploadUtcDateTime = dispensingDeviceResult.LastSuccessfulUploadUTCDateTime,
                                    LastSuccessfulUploadDateTime = dispensingDeviceResult.LastSuccessfulUploadLocalDateTime,
                                    LastSuccessfulDownloadUtcDateTime = dispensingDeviceResult.LastSuccessfulDownloadUTCDateTime,
                                    LastSuccessfulDownloadDateTime = dispensingDeviceResult.LastSuccessfulDownloadLocalDateTime,
                                    IpAddress = dispensingDeviceResult.IPAddressValue,
                                    SyncDownloadStatus = dispensingDeviceResult.SyncDownloadStatusInternalCode.FromNullableInternalCode<SyncDownloadStatusInternalCode>(),
                                    WitnessOnCubieEject = dispensingDeviceResult.WitnessOnCUBIEEjectFlag,
                                    WasteMode = dispensingDeviceResult.WasteModeInternalCode.FromNullableInternalCode<WasteModeInternalCode>(),
                                    FutureTaskWarningDurationAmount = dispensingDeviceResult.FutureTaskWarningDurationAmount,
                                    PharmacyOrderDispenseQuantity = dispensingDeviceResult.PharmacyOrderDispenseQuantityFlag,
                                    OneTimePasswordSecretKeyValue = dispensingDeviceResult.OneTimePasswordSecretKeyValue,
                                    OneTimePasswordTimeoutDurationAmount = dispensingDeviceResult.OneTimePasswordTimeoutDurationAmount,
                                    IdentityServerClientID = dispensingDeviceResult.IdentityServerClientID,
                                    IdentityServerClientSecretValue = dispensingDeviceResult.IdentityServerClientSecretValue,
                                    UseEquivalencies = dispensingDeviceResult.UseEquivalenciesFlag,
                                    IsGrabScan = dispensingDeviceResult.GrabScanFlag,
                                    ReceiveBarcodeInventoryDecrementExternalFlag = dispensingDeviceResult.ReceiveBarcodeInventoryDecrementExternalFlag,
                                    ExternalSystemDeviceHostValue = dispensingDeviceResult.ExternalSystemDeviceHostValue,
                                    ExternalSystemDevicePortNumber = dispensingDeviceResult.ExternalSystemDevicePortNumber,
                                    ExternalSystemDeviceAdminUserPasswordValue = dispensingDeviceResult.ExternalSystemDeviceAdminUserPasswordValue,
                                    ExternalSystemDeviceCommandTimeoutDurationAmount = dispensingDeviceResult.ExternalSystemDeviceCommandTimeoutDurationAmount,
                                    OpenBinTimeoutDurationAmount = dispensingDeviceResult.OpenBinTimeoutDurationAmount,
                                    BarcodeReceiverDevicePortNumber = dispensingDeviceResult.BarcodeReceiverDevicePortNumber,
                                    GCSMDestructionBinTimeOutDurationAmount = dispensingDeviceResult.GCSMDestructionBinTimeOutDurationAmount,
                                    GCSMScanOnCompoundingFlag = dispensingDeviceResult.GCSMScanOnCompoundingFlag,         
                                    GCSMScanOnIssueFlag = dispensingDeviceResult.GCSMScanOnIssueFlag,
                                    GCSMScanOnPrescriptionFlag = dispensingDeviceResult.GCSMScanOnPrescriptionFlag,         
                                    GCSMScanOnReturnFlag = dispensingDeviceResult.GCSMScanOnReturnFlag,
                                    GCSMScanOnReceiveFlag = dispensingDeviceResult.GCSMScanOnReceiveFlag,
                                    GCSMScanOnRestockADMFlag = dispensingDeviceResult.GCSMScanOnRestockADMFlag,
                                    GCSMScanOnSellFlag = dispensingDeviceResult.GCSMScanOnSellFlag,
                                    GCSMScanOnStockTransferFlag = dispensingDeviceResult.GCSMScanOnStockTransferFlag,
                                    GCSMWitnessOnCompoundingFlag = dispensingDeviceResult.GCSMWitnessOnCompoundingFlag,
                                    GCSMWitnessOnEmptyDestructionBinFlag = dispensingDeviceResult.GCSMWitnessOnEmptyDestructionBinFlag,
                                    GCSMWitnessOnInventoryFlag = dispensingDeviceResult.GCSMWitnessOnInventoryFlag,
                                    GCSMWitnessOnIssueFlag = dispensingDeviceResult.GCSMWitnessOnIssueFlag,
                                    GCSMWitnessOnOutdateFlag = dispensingDeviceResult.GCSMWitnessOnOutdateFlag,
                                    GCSMWitnessOnPrescriptionFlag = dispensingDeviceResult.GCSMWitnessOnPrescriptionFlag,
                                    GCSMWitnessOnRecallFlag = dispensingDeviceResult.GCSMWitnessOnRecallFlag,
                                    GCSMWitnessOnReceiveFlag = dispensingDeviceResult.GCSMWitnessOnReceiveFlag,
                                    GCSMWitnessOnRestockADMFlag = dispensingDeviceResult.GCSMWitnessOnRestockADMFlag,
                                    GCSMWitnessOnReturnFlag = dispensingDeviceResult.GCSMWitnessOnReturnFlag,
                                    GCSMWitnessOnReverseCompoundingFlag = dispensingDeviceResult.GCSMWitnessOnReverseCompoundingFlag,
                                    GCSMWitnessOnSellFlag = dispensingDeviceResult.GCSMWitnessOnSellFlag,
                                    GCSMWitnessOnStockTransferFlag = dispensingDeviceResult.GCSMWitnessOnStockTransferFlag,
                                    GCSMWitnessOnUnloadFlag = dispensingDeviceResult.GCSMWitnessOnUnloadFlag,
                                    GCSMWitnessOnWasteFlag = dispensingDeviceResult.GCSMWitnessOnWasteFlag,
                                    GCSMWitnessOnceInventoryFlag = dispensingDeviceResult.GCSMWitnessOnceInventoryFlag,
                                    GCSMWitnessOnRecoverFlag = dispensingDeviceResult.GCSMWitnessOnRecoverFlag,
                                    GCSMWitnessOnAccessToDestructionBinFlag = dispensingDeviceResult.GCSMWitnessOnAccessToDestructionBinFlag,
                                    GCSMWitnessOnAddToDestructionBinFlag = dispensingDeviceResult.GCSMWitnessOnAddToDestructionBinFlag,
                                    GCSMBlindCountFlag = dispensingDeviceResult.GCSMBlindCountFlag,
                                    GCSMLabelPrinterName = dispensingDeviceResult.GCSMLabelPrinterName,
                                    GCSMSheetPrinterName = dispensingDeviceResult.GCSMSheetPrinterName,
                                    ControlledSubstanceLicenseKey = dispensingDeviceResult.ControlledSubstanceLicenseKey,
                                    DisplayPatientPreferredNameFlag = dispensingDeviceResult.DisplayPatientPreferredNameFlag,
                                    GCSMPrintReceiptOnAccessDestructionBinFlag = dispensingDeviceResult.GCSMPrintReceiptOnAccessDestructionBinFlag,
                                    GCSMPrintReceiptOnDiscrepancyResolutionFlag = dispensingDeviceResult.GCSMPrintReceiptOnDiscrepancyResolutionFlag,
                                    GCSMPrintReceiptOnEmptyDestructionBinFlag = dispensingDeviceResult.GCSMPrintReceiptOnEmptyDestructionBinFlag,
                                    GCSMPrintReceiptOnFillPrescriptionFlag = dispensingDeviceResult.GCSMPrintReceiptOnFillPrescriptionFlag,
                                    GCSMPrintReceiptOnManageStockFlag = dispensingDeviceResult.GCSMPrintReceiptOnManageStockFlag,
                                    GCSMPrintReceiptOnNonStandardCompoundRecordFlag = dispensingDeviceResult.GCSMPrintReceiptOnNonStandardCompoundRecordFlag,
                                    GCSMPrintReceiptOnOutdateFlag = dispensingDeviceResult.GCSMPrintReceiptOnOutdateFlag,
                                    GCSMPrintReceiptOnPendingStandardCompoundRecordFlag = dispensingDeviceResult.GCSMPrintReceiptOnPendingStandardCompoundRecordFlag,
                                    GCSMPrintReceiptOnRecallFlag = dispensingDeviceResult.GCSMPrintReceiptOnRecallFlag,
                                    GCSMPrintReceiptOnReceiveFlag = dispensingDeviceResult.GCSMPrintReceiptOnReceiveFlag,
                                    GCSMPrintReceiptOnReturnFlag = dispensingDeviceResult.GCSMPrintReceiptOnReturnFlag,
                                    GCSMPrintReceiptOnReverseNonStandardCompoundFlag = dispensingDeviceResult.GCSMPrintReceiptOnReverseNonStandardCompoundFlag,
                                    GCSMPrintReceiptOnStandardCompoundDispositionSummaryFlag = dispensingDeviceResult.GCSMPrintReceiptOnStandardCompoundDispositionSummaryFlag,
                                    GCSMPrintReceiptOnSellFlag = dispensingDeviceResult.GCSMPrintReceiptOnSellFlag,
                                    GCSMPrintReceiptOnUnloadFlag = dispensingDeviceResult.GCSMPrintReceiptOnUnloadFlag,
                                    GCSMPrintReceiptOnWasteFlag = dispensingDeviceResult.GCSMPrintReceiptOnWasteFlag,
                                    GCSMPrintLabelOnAccessDestructionBinFlag = dispensingDeviceResult.GCSMPrintLabelOnAccessDestructionBinFlag,
                                    GCSMPrintLabelOnFillPrescriptionFlag = dispensingDeviceResult.GCSMPrintLabelOnFillPrescriptionFlag,
                                    GCSMPrintLabelOnNonStandardCompoundIngredientFlag = dispensingDeviceResult.GCSMPrintLabelOnNonStandardCompoundIngredientFlag,
                                    GCSMPrintLabelOnNonStandardCompoundMedFlag = dispensingDeviceResult.GCSMPrintLabelOnNonStandardCompoundMedFlag,
                                    GCSMPrintLabelOnOutdateFlag = dispensingDeviceResult.GCSMPrintLabelOnOutdateFlag,
                                    GCSMPrintLabelOnRecallFlag = dispensingDeviceResult.GCSMPrintLabelOnRecallFlag,
                                    GCSMPrintLabelOnReturnFlag = dispensingDeviceResult.GCSMPrintLabelOnReturnFlag,
                                    GCSMPrintLabelOnSellFlag = dispensingDeviceResult.GCSMPrintLabelOnSellFlag,
                                    GCSMPrintLabelOnStandardCompoundMedFlag = dispensingDeviceResult.GCSMPrintLabelOnStandardCompoundMedFlag,
                                    GCSMPrintLabelOnStandardCompoundIngredientFlag = dispensingDeviceResult.GCSMPrintLabelOnStandardCompoundIngredientFlag,
                                    GCSMPrintLabelOnWasteFlag = dispensingDeviceResult.GCSMPrintLabelOnWasteFlag,
                                    GCSMPrintPullListOnAutorestockFlag = dispensingDeviceResult.GCSMPrintPullListOnAutorestockFlag,
                                    GCSMPrintPullListOnDispenseToLocationFlag = dispensingDeviceResult.GCSMPrintPullListOnDispenseToLocationFlag,
                                    GCSMPrintPullListOnKitFlag = dispensingDeviceResult.GCSMPrintPullListOnKitFlag,
                                    GCSMPrintPullListOnKitComponentFlag = dispensingDeviceResult.GCSMPrintPullListOnKitComponentFlag,
                                    GCSMPrintPullListOnManageExcessStockFlag = dispensingDeviceResult.GCSMPrintPullListOnManageExcessStockFlag,
                                    GCSMPrintPullListOnNonStandardCompoundFlag = dispensingDeviceResult.GCSMPrintPullListOnNonStandardCompoundFlag,
                                    GCSMPrintPullListOnNonStandardCompoundIngredientFlag = dispensingDeviceResult.GCSMPrintPullListOnNonStandardCompoundIngredientFlag,
                                    GCSMPrintPullListOnStandardCompoundFlag = dispensingDeviceResult.GCSMPrintPullListOnStandardCompoundFlag,
                                    GCSMPrintPullListOnStandardCompoundIngredientFlag = dispensingDeviceResult.GCSMPrintPullListOnStandardCompoundIngredientFlag,
                                    GCSMHideAreaFilterFlag = dispensingDeviceResult.GCSMHideAreaFilterFlag,
                                    GCSMHideZoneFilterFlag = dispensingDeviceResult.GCSMHideZoneFilterFlag,
                                    GCSMCompareReportStandardRange = dispensingDeviceResult.GCSMCompareReportStandardRangeInternalCode.FromNullableInternalCode<GCSMCompareReportStandardRangeInternalCode>(),
                                    GCSMCompareReportViewFilterFocusedFlag = dispensingDeviceResult.GCSMCompareReportViewFilterFocusedFlag,
                                    GCSMOriginDispensingDeviceKey = dispensingDeviceResult.GCSMOriginDispensingDeviceKey,
                                    GCSMDestinationDispensingDeviceKey = dispensingDeviceResult.GCSMDestinationDispensingDeviceKey,
                                    LastModified = dispensingDeviceResult.LastModifiedBinaryValue.ToArray()
                            };

                        // Critical override periods
                        dispensingDevice.CriticalOverridePeriods = criticalOverridePeriodResults
                            .Where(cop => cop.DispensingDeviceKey == dispensingDeviceResult.DispensingDeviceKey)
                            .Select(cop => new CriticalOverridePeriod(cop.CriticalOverridePeriodKey)
                                {
                                    Name = cop.CriticalOverridePeriodName,
                                    DispensingDeviceName = dispensingDeviceResult.DispensingDeviceName,
                                    DispensingDeviceKey = cop.DispensingDeviceKey,
                                    StartTimeOfDay = cop.StartTimeOfDayValue,
                                    EndTimeOfDay = cop.EndTimeOfDayValue,
                                    Monday = cop.MondayFlag,
                                    Tuesday = cop.TuesdayFlag,
                                    Wednesday = cop.WednesdayFlag,
                                    Thursday = cop.ThursdayFlag,
                                    Friday = cop.FridayFlag,
                                    Saturday = cop.SaturdayFlag,
                                    Sunday = cop.SundayFlag,
                                    LastModified = cop.LastModifiedBinaryValue,
                                    CreatedUTCDateTime = cop.CreatedUTCDateTime,
                                    CreatedLocalDateTime = cop.CreatedLocalDateTime,
                                    CreatedActorKey = cop.CreatedActorKey,
                                    CreatedActorName = cop.CreatedActorName
                            })
                            .ToArray();

                        // Areas
                        dispensingDevice.Areas = areaResults
                            .Where(ar => ar.DispensingDeviceKey == dispensingDeviceResult.DispensingDeviceKey)
                            .Select(ar => new Area(ar.AreaKey)
                                {
                                    Name = ar.AreaName
                                })
                            .ToArray();

                        // Override groups
                        dispensingDevice.OverrideGroups = overrideGroupResults
                            .Where(og => og.DispensingDeviceKey == dispensingDeviceResult.DispensingDeviceKey)
                            .Select(og => new OverrideGroup(og.OverrideGroupKey)
                                {
                                    DisplayCode = og.DisplayCode,
                                    Description = og.DescriptionText,
                                    IsActive = og.ActiveFlag,
                                })
                            .ToArray();

                        // Clinical data subjects
                        dispensingDevice.ClinicalDataSubjects = clinicalDataSubjectResults
                            .Where(cds => cds.DispensingDeviceKey == dispensingDeviceResult.DispensingDeviceKey)
                            .Select(cds => cds.ClinicalDataSubjectKey)
                            .ToArray();

                        // Facility Kits
                        dispensingDevice.FacilityKits = facilityKitResults
                            .Where(fk => fk.DispensingDeviceKey == dispensingDeviceResult.DispensingDeviceKey &&
                                         !fk.StatFlag)
                            .Select(fk => fk.FacilityKitKey)
                            .ToArray();

                        // Stat Facility Kits
                        dispensingDevice.StatFacilityKits = facilityKitResults
                            .Where(fk => fk.DispensingDeviceKey == dispensingDeviceResult.DispensingDeviceKey &&
                                         fk.StatFlag)
                            .Select(fk => fk.FacilityKitKey)
                            .ToArray();

                        // Restock Groups
                        dispensingDevice.RestockGroups = restockGroupResults
                                .Where(rg => rg.DispensingDeviceKey == dispensingDeviceResult.DispensingDeviceKey)
                                .Select(rg => new RestockGroup(rg.GCSMRestockGroupKey)
                                {
                                    RestockGroupName = rg.RestockGroupName,
                                    Description = rg.DescriptionText,
                                    GcsmDispensingDeviceName = rg.GCSMDispensingDeviceName,
                                    GcsmDispensingDeviceFacility = rg.FacilityName,
                                    IsActive = rg.ActiveFlag
                                })
                                .ToArray();

                        dispensingDevices.Add(dispensingDevice);
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return dispensingDevices;
        }

        DispensingDevice IStorageRepository.GetDispensingDevice(string computerName)
        {
            IEnumerable<DispensingDevice> dispensingDevices = ((IStorageRepository) this).GetDispensingDevices(
                null, false, computerName);

            return dispensingDevices.FirstOrDefault();
        }

        DispensingDevice IStorageRepository.GetDispensingDevice(Guid dispensingDeviceKey)
        {
            IEnumerable<DispensingDevice> dispensingDevices = ((IStorageRepository)this).GetDispensingDevices(
                new [] { dispensingDeviceKey }, false);

            return dispensingDevices.FirstOrDefault();
        }

        Guid IStorageRepository.InsertDispensingDevice(Context context, DispensingDevice dispensingDevice)
        {
            Guard.ArgumentNotNull(context, "Context");
            Guard.ArgumentNotNull(dispensingDevice, "dispensingDevice");

            Guid? dispensingDeviceKey = null;

            try
            {                
                using (ITransactionScope tx = TransactionScopeFactory.Create())                
                {
                    dispensingDeviceKey = _dispensingDeviceRepository.InsertDispensingDevice(context.ToActionContext(), dispensingDevice.ToModel());

                    if (dispensingDeviceKey.HasValue)
                    {
                        if (dispensingDevice.CriticalOverridePeriods != null)
                        {
                            InsertDispensingDeviceCriticalOverridePeriods(                                
                                context,
                                dispensingDeviceKey.Value,
                                dispensingDevice.CriticalOverridePeriods);
                        }

                        if (dispensingDevice.Areas != null)
                        {
                            InsertDispensingDeviceAreas(
                                context,
                                dispensingDeviceKey.Value,
                                dispensingDevice.Areas.Select(a => a.Key));
                        }

                        if (dispensingDevice.OverrideGroups != null)
                        {
                            InsertDispensingDeviceOverrideGroups(                                
                                context,
                                dispensingDeviceKey.Value,
                                dispensingDevice.OverrideGroups.Select(og => og.Key));
                        }

                        if (dispensingDevice.ClinicalDataSubjects != null)
                        {
                            InsertDispensingDeviceClinicalDataSubjects(
                                context,
                                dispensingDeviceKey.Value,
                                dispensingDevice.ClinicalDataSubjects);
                        }

                        if (dispensingDevice.FacilityKits != null)
                        {
                            InsertDispensingDeviceFacilityKits(
                                context,
                                dispensingDeviceKey.Value,
                                false,
                                dispensingDevice.FacilityKits);
                        }

                        if (dispensingDevice.StatFacilityKits != null)
                        {
                            InsertDispensingDeviceFacilityKits(
                                context,
                                dispensingDeviceKey.Value,
                                true,
                                dispensingDevice.StatFacilityKits);
                        }
                    }

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return dispensingDeviceKey.GetValueOrDefault();
        }

        void IStorageRepository.UpdateDispensingDevice(Context context, DispensingDevice dispensingDevice)
        {
            Guard.ArgumentNotNull(context, "Context");
            Guard.ArgumentNotNull(dispensingDevice, "dispensingDevice");

            try
            {
                using (ConnectionScopeFactory.Create())
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                {
                    _dispensingDeviceRepository.UpdateDispensingDevice(context.ToActionContext(), dispensingDevice.ToModel());                    

                    UpdateDispensingDeviceCriticalOverridePeriods(
                        context,
                        dispensingDevice.Key,
                        dispensingDevice.CriticalOverridePeriods ?? new CriticalOverridePeriod[0]);

                    UpdateDispensingDeviceAreas(
                        context,
                        dispensingDevice.Key,
                        (dispensingDevice.Areas != null)
                            ? dispensingDevice.Areas.Select(a => a.Key)
                            : new Guid[0]);

                    UpdateDispensingDeviceOverrideGroups(
                        context,
                        dispensingDevice.Key,
                        (dispensingDevice.OverrideGroups != null)
                            ? dispensingDevice.OverrideGroups.Select(og => og.Key)
                            : new Guid[0]);

                    UpdateDispensingDeviceClinicalDataSubjects(
                        context,
                        dispensingDevice.Key,
                        dispensingDevice.ClinicalDataSubjects ?? new Guid[0]);

                    UpdateDispensingDeviceFacilityKits(
                        context,
                        dispensingDevice.Key,
                        false,
                        dispensingDevice.FacilityKits ?? new Guid[0]);

                    UpdateDispensingDeviceFacilityKits(
                        context,
                        dispensingDevice.Key,
                        true,
                        dispensingDevice.StatFacilityKits ?? new Guid[0]);

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IStorageRepository.DeleteDispensingDevice(Context context, Guid dispensingDeviceKey)
        {
            Guard.ArgumentNotNull(context, "Context");

            try
            {
                _dispensingDeviceRepository.DeleteDispensingDevice(context.ToActionContext(), dispensingDeviceKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IStorageRepository.DeleteDispensingDeviceCriticalOverridePeriods(Context context, Guid facilityKey)
        {
            Guard.ArgumentNotNull(context, "Context");

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(new Dictionary<string, object>{
                        { "@StartUTCDateTime",  context.ActionUtcDateTime},
                        { "@StartLocalDateTime", context.ActionDateTime },
                        { "@LastModifiedActorKey",  (Guid?)context.Actor},
                        { "@FacilityKey", facilityKey },
                    });

                    connectionScope.Execute(
                       "Strg.bsp_MultiCriticalOverridePeriodDelete",
                       parameters,
                       commandTimeout: connectionScope.DefaultCommandTimeout,
                       commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        string IStorageRepository.GetDispensingDeviceSerialId(Guid dispensingDeviceKey)
        {
            var serialId = string.Empty;
            try
            {
                var query = new SqlBuilder();
                query.SELECT()
                    ._("SerialID")
                    .FROM("Strg.StorageSpaceSnapshot sss")
                    .INNER_JOIN("Strg.DispensingDeviceStorageSpace ddss on ddss.StorageSpaceKey = sss.StorageSpaceKey AND ddss.DisassociationUTCDateTime IS NULL")
                    .WHERE("sss.EndUTCDateTime IS NULL")
                    .WHERE("sss.DeleteFlag = 0")
                    .WHERE("sss.StorageSpaceTypeInternalCode = @StorageSpaceTypeInternalCode")
                    .WHERE("ddss.DispensingDeviceKey = @DispensingDeviceKey");

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    serialId = connectionScope.ExecuteScalar<string>(
                    query.ToString(),
                    new
                    {
                        DispensingDeviceKey = dispensingDeviceKey,
                        StorageSpaceTypeInternalCode = StorageSpaceTypeInternalCode.MEDMAINCAB1.ToInternalCode()
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.Text);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
            return serialId;
        }

        #endregion

        #region VirtualStockLocation Members

        IEnumerable<VirtualStockLocation> IStorageRepository.GetVirtualStockLocations(IEnumerable<Guid> virutalStockLocationKeys, bool? deleted, 
            Guid? facilityKey)
        {
            IEnumerable<VirtualStockLocation> virtualStockLocations = new List<VirtualStockLocation>();
            if (virutalStockLocationKeys != null && !virutalStockLocationKeys.Any())
                return virtualStockLocations; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (virutalStockLocationKeys != null)
                    selectedKeys = new GuidKeyTable(virutalStockLocationKeys.Distinct());

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var vslResults = connectionScope.Query<VirtualStockLocationsResult>(
                        "Strg.bsp_GetVirtualStockLocations",
                        new { SelectedKeys = selectedKeys.AsTableValuedParameter(), DeleteFlag = deleted, FacilityKey = facilityKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);

                    virtualStockLocations = vslResults
                        .Select(result => new VirtualStockLocation(result.VirtualStockLocationKey)
                        {
                            FacilityKey = result.FacilityKey,
                            FacilityCode = result.FacilityCode,
                            FacilityName = result.FacilityName,
                            Name = result.VirtualStockLocationName,
                            IsDeleted = result.DeleteFlag,
                            LastModified = result.LastModifiedBinaryValue
                        })
                        .ToList();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return virtualStockLocations;
        }

        VirtualStockLocation IStorageRepository.GetVirtualStockLocation(Guid virtualStockLocationKey)
        {
            var virtualStockLocations =
                ((IStorageRepository)this).GetVirtualStockLocations(new[] { virtualStockLocationKey }, false);

            return virtualStockLocations.FirstOrDefault();
        }

        Guid IStorageRepository.InsertVirtualStockLocation(Context context, VirtualStockLocation virtualStockLocation)
        {
            Guard.ArgumentNotNull(context, "Context");
            Guard.ArgumentNotNull(virtualStockLocation, "virtualStockLocation");

            Guid? virtualStockLocationKey = null;

            try
            {
                var model = new StrgDAL.Models.VirtualStockLocation()
                {
                    FacilityKey = virtualStockLocation.FacilityKey,
                    VirtualStockLocationName = virtualStockLocation.Name
                };
                virtualStockLocationKey = _virtualStockLocationRepository.InsertVirtualStockLocation(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return virtualStockLocationKey.GetValueOrDefault();
        }

        void IStorageRepository.UpdateVirtualStockLocation(Context context, VirtualStockLocation virtualStockLocation)
        {
            Guard.ArgumentNotNull(context, "Context");
            Guard.ArgumentNotNull(virtualStockLocation, "virtualStockLocation");

            try
            {
                var model = new StrgDAL.Models.VirtualStockLocation()
                {
                    VirtualStockLocationName = virtualStockLocation.Name,
                    LastModifiedBinaryValue = virtualStockLocation.LastModified ?? new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
                    VirtualStockLocationKey = virtualStockLocation.Key
                };

                _virtualStockLocationRepository.UpdateVirtualStockLocation(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IStorageRepository.DeleteVirtualStockLocation(Context context, Guid virtualStockLocationKey)
        {
            Guard.ArgumentNotNull(context, "Context");

            try
            {
                _virtualStockLocationRepository.DeleteVirtualStockLocation(context.ToActionContext(), virtualStockLocationKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region Zone Members

        IEnumerable<Zone> IStorageRepository.GetZones(IEnumerable<Guid> zoneKeys, bool? deleted, Guid? facilityKey)
        {
            IEnumerable<Zone> zones = new List<Zone>();
            if (zoneKeys != null && !zoneKeys.Any())
                return zones; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (zoneKeys != null)
                    selectedKeys = new GuidKeyTable(zoneKeys.Distinct());

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var zoneResults = connectionScope.Query<ZonesResult>(
                        "Strg.bsp_GetZones",
                        new { SelectedKeys = selectedKeys.AsTableValuedParameter(), DeleteFlag = deleted, FacilityKey = facilityKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);

                    zones = zoneResults
                            .Select(result => new Zone(result.ZoneKey)
                            {
                                FacilityKey = result.FacilityKey,
                                FacilityCode = result.FacilityCode,
                                FacilityName = result.FacilityName,
                                Name = result.ZoneName,
                                Number = result.ZoneNumber,
                                LastModified = result.LastModifiedBinaryValue
                            })
                            .ToList();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return zones;
        }

        Zone IStorageRepository.GetZone(Guid zoneKey)
        {
            var zones =
                ((IStorageRepository)this).GetZones(new[] { zoneKey }, false);

            return zones.FirstOrDefault();
        }

        Guid IStorageRepository.InsertZone(Context context, Zone zone)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(zone, "zone");

            Guid? zoneKey = null;

            try
            {
                var model = new StrgDAL.Models.Zone()
                {
                    FacilityKey = zone.FacilityKey,
                    ZoneName = zone.Name,
                    ZoneNumber = zone.Number,
                };

                zoneKey = _zoneRepository.InsertZone(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return zoneKey.GetValueOrDefault();
        }

        void IStorageRepository.UpdateZone(Context context, Zone zone)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(zone, "zone");

            try
            {
                var model = new StrgDAL.Models.Zone()
                {
                    ZoneKey = zone.Key,
                    ZoneName = zone.Name,
                    ZoneNumber = zone.Number,
                    LastModifiedBinaryValue = zone.LastModified ?? new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }
                };

                _zoneRepository.UpdateZone(context.ToActionContext(), model);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IStorageRepository.DeleteZone(Context context, Guid zoneKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                _zoneRepository.DeleteZone(context.ToActionContext(), zoneKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }
        #endregion

        #region System Bus Device Members
        SystemBusDevice IStorageRepository.GetSystemBusDevice(Guid systemBusDeviceKey)
        {
            SystemBusDevice systemBusDevice = null;

            try
            {
                var sql = new SqlBuilder();
                sql.SELECT()
                    ._("sbd.SystemBusDeviceKey")
                    ._("ddbd.DispensingDeviceKey")
                    ._("sbd.ControllingSystemBusDeviceKey")
                    ._("sbd.DeviceNumber")
                    ._("sbd.PositionNumber")
                    ._("sbd.WidthQuantity")
                    ._("sbd.HeightQuantity")
                    ._("sbd.DepthQuantity")
                    ._("sbd.OffsetQuantity")
                    ._("sbd.SystemBusDeviceTypeInternalCode")
                    ._("sbd.CommunicationCubeTypeInternalCode")
                    ._("sbd.LastModifiedBinaryValue")
                    .FROM("Strg.SystemBusDeviceSnapshot sbd")
                    .LEFT_JOIN("Strg.DispensingDeviceSystemBusDevice ddbd ON sbd.SystemBusDeviceKey = ddbd.SystemBusDeviceKey AND ddbd.DisassociationUTCDateTime IS NULL")
                    .WHERE("sbd.EndUTCDateTime IS NULL")
                    .WHERE("sbd.DeleteFlag = 0")
                    .WHERE("sbd.SystemBusDeviceKey = @SystemBusDeviceKey");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    systemBusDevice = connectionScope.QueryFirstOrDefault<SystemBusDeviceResult>(
                         sql.ToString(),
                         new
                         {
                             SystemBusDeviceKey = systemBusDeviceKey
                         },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text)?.ToContract();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return systemBusDevice;
        }

        IEnumerable<SystemBusDevice> IStorageRepository.GetSystemBusDevices(long deviceNumber)
        {
            IEnumerable<SystemBusDevice> systemBusDevices = null;

            try
            {
                var sql = new SqlBuilder();
                sql.SELECT()
                    ._("sbd.SystemBusDeviceKey")
                    ._("ddbd.DispensingDeviceKey")
                    ._("sbd.ControllingSystemBusDeviceKey")
                    ._("sbd.DeviceNumber")
                    ._("sbd.PositionNumber")
                    ._("sbd.WidthQuantity")
                    ._("sbd.HeightQuantity")
                    ._("sbd.DepthQuantity")
                    ._("sbd.OffsetQuantity")
                    ._("sbd.SystemBusDeviceTypeInternalCode")
                    ._("sbd.CommunicationCubeTypeInternalCode")
                    ._("sbd.LastModifiedBinaryValue")
                    .FROM("Strg.SystemBusDeviceSnapshot sbd")
                    .LEFT_JOIN("Strg.DispensingDeviceSystemBusDevice ddbd ON sbd.SystemBusDeviceKey = ddbd.SystemBusDeviceKey AND ddbd.DisassociationUTCDateTime IS NULL")
                    .WHERE("sbd.EndUTCDateTime IS NULL")
                    .WHERE("sbd.DeleteFlag = 0")
                    .WHERE("sbd.DeviceNumber = @DeviceNumber");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    systemBusDevices = connectionScope.Query<SystemBusDeviceResult>(
                         sql.ToString(),
                         new
                         {
                             DeviceNumber = deviceNumber
                         },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text)
                        .Select(s => s.ToContract())
                        .ToArray();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return systemBusDevices;
        }

        Guid IStorageRepository.InsertSystemBusDevice(Context context, SystemBusDevice systemBusDevice)
        {
            Guard.ArgumentNotNull(() => context);
            Guard.ArgumentNotNull(() => systemBusDevice);

            Guid? systemBusDeviceKey = null;

            try
            {
                systemBusDeviceKey = _systemBusDeviceRepository.InsertSystemBusDevice(context.ToActionContext(), new StrgDAL.Models.SystemBusDevice
                {
                    CommunicationCubeTypeInternalCode = systemBusDevice.CommunicationCubeType != null ? systemBusDevice.CommunicationCubeType.ToInternalCode() : default(string),
                    ControllingSystemBusDeviceKey = systemBusDevice.ParentSystemBusKey,
                    DepthQuantity = systemBusDevice.Depth,
                    DeviceNumber = systemBusDevice.SerialNumber,
                    HeightQuantity = systemBusDevice.Height,
                    OffsetQuantity = systemBusDevice.Offset,
                    PositionNumber = systemBusDevice.Position,                    
                    SystemBusDeviceTypeInternalCode = systemBusDevice.SystemBusDeviceType.ToInternalCode(),
                    WidthQuantity = systemBusDevice.Width
                });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return systemBusDeviceKey.GetValueOrDefault();
        }

        void IStorageRepository.UpdateSystemBusDevice(Context context, SystemBusDevice systemBusDevice)
        {
            Guard.ArgumentNotNull(() => context);
            Guard.ArgumentNotNull(() => systemBusDevice);

            try
            {
                _systemBusDeviceRepository.UpdateSystemBusDevice(context.ToActionContext(), new StrgDAL.Models.SystemBusDevice
                {
                    CommunicationCubeTypeInternalCode = systemBusDevice.CommunicationCubeType != null ? systemBusDevice.CommunicationCubeType.ToInternalCode() : default(string),
                    ControllingSystemBusDeviceKey = systemBusDevice.ParentSystemBusKey,
                    DepthQuantity = systemBusDevice.Depth,
                    DeviceNumber = systemBusDevice.SerialNumber,
                    HeightQuantity = systemBusDevice.Height,
                    OffsetQuantity = systemBusDevice.Offset,
                    PositionNumber = systemBusDevice.Position,
                    SystemBusDeviceKey = systemBusDevice.Key,
                    SystemBusDeviceTypeInternalCode = systemBusDevice.SystemBusDeviceType.ToInternalCode(),
                    WidthQuantity = systemBusDevice.Width,
                    LastModifiedBinaryValue = systemBusDevice.LastModified
                });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IStorageRepository.DeleteSystemBusDevice(Context context, Guid systemBusDeviceKey)
        {
            Guard.ArgumentNotNull(() => context);

            try
            {
                _systemBusDeviceRepository.DeleteSystemBusDevice(context.ToActionContext(), systemBusDeviceKey);                
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IStorageRepository.InsertDispensingDeviceSystemBusDevice(Context context, Guid dispensingDeviceKey, Guid systemBusDeviceKey)
        {
            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Strg.usp_DispensingDeviceSystemBusDeviceInsert",
                        new
                        {
                            AssociationUTCDateTime = context.ActionUtcDateTime,
                            AssociationLocalDateTime = context.ActionDateTime,
                            AssociationActorKey = (Guid?)context.Actor,
                            SystemBusDeviceKey = systemBusDeviceKey,
                            DispensingDeviceKey = dispensingDeviceKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IStorageRepository.DeleteDispensingDeviceSystemBusDevice(Context context, Guid dispensingDeviceKey, Guid systemBusDeviceKey)
        {
            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Strg.usp_DispensingDeviceSystemBusDeviceDelete",
                        new
                        {
                            DisassociationUTCDateTime = context.ActionUtcDateTime,
                            DisassociationLocalDateTime = context.ActionDateTime,
                            DisassociationActorKey = (Guid?)context.Actor,
                            SystemBusDeviceKey = systemBusDeviceKey,
                            DispensingDeviceKey = dispensingDeviceKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }
        #endregion

        #region Storage Space Members
        void IStorageRepository.InsertDispensingDeviceStorageSpace(Context context, Guid dispensingDeviceKey, Guid storageSpaceKey)
        {
            Guard.ArgumentNotNull(() => context);

            try
            {
                _dispensingDeviceRepository.AssociateDispensingDeviceStorageSpace(context.ToActionContext(), dispensingDeviceKey, storageSpaceKey);                
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IStorageRepository.DeleteDispensingDeviceStorageSpace(Context context, Guid dispensingDeviceKey, Guid storageSpaceKey)
        {
            Guard.ArgumentNotNull(() => context);

            try
            {
                _dispensingDeviceRepository.DisassociateDispensingDeviceStorageSpace(context.ToActionContext(), dispensingDeviceKey, storageSpaceKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        StorageSpace IStorageRepository.GetStorageSpace(Guid storageSpaceKey)
        {
            StorageSpace storageSpace = null;

            try
            {
                var sql = GetStorageSpaceQuery();
                sql.WHERE("sss.StorageSpaceKey = @StorageSpaceKey");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    storageSpace = connectionScope.QueryFirstOrDefault<StorageSpaceResult>(
                        sql.ToString(),
                        new
                        {
                            StorageSpaceKey = storageSpaceKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        ?.ToContract();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return storageSpace;
        }

        StorageSpace IStorageRepository.GetStorageSpaceBySystemBusDeviceKey(Guid systemBusDeviceKey)
        {
            StorageSpace storageSpace = null;

            try
            {
                var sql = GetStorageSpaceQuery();
                sql.WHERE("sss.SystemBusDeviceKey = @SystemBusDeviceKey");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    storageSpace = connectionScope.QueryFirstOrDefault<StorageSpaceResult>(
                        sql.ToString(),
                        new
                        {
                            SystemBusDeviceKey = systemBusDeviceKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        ?.ToContract();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return storageSpace;
        }

        IEnumerable<StorageSpace> IStorageRepository.GetStorageSpaces(long deviceNumber, StorageSpaceTypeInternalCode storageSpaceType)
        {
            IEnumerable<StorageSpace> storageSpaces = null;

            try
            {
                var sql = GetStorageSpaceBaseQuery();
                sql.JOIN("Strg.SystemBusDeviceSnapshot sbd ON sbd.SystemBusDeviceKey = sss.SystemBusDeviceKey AND sbd.EndUTCDateTime IS NULL AND sbd.DeleteFlag = 0");
                sql.WHERE("sss.EndUTCDateTime IS NULL");
                sql.WHERE("sss.DeleteFlag = 0");
                sql.WHERE("sbd.DeviceNumber = @DeviceNumber");
                sql.WHERE("sss.StorageSpaceTypeInternalCode = @StorageSpaceTypeInternalCode");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    storageSpaces = connectionScope.Query<StorageSpaceResult>(
                        sql.ToString(),
                        new
                        {
                            DeviceNumber = deviceNumber,
                            StorageSpaceTypeInternalCode = storageSpaceType.ToInternalCode(),
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Select(s => s.ToContract());
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return storageSpaces;
        }

        IEnumerable<StorageSpace> IStorageRepository.GetChildrenStorageSpaces(Guid storageSpaceKey)
        {
            var childrenStorageSpaces = new List<StorageSpace>();

            try
            {
                var sql = GetStorageSpaceQuery();
                sql.WHERE("sss.ParentStorageSpaceKey = @ParentStorageSpaceKey");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var result = connectionScope.Query<StorageSpaceResult>(
                        sql.ToString(),
                        new
                        {
                            ParentStorageSpaceKey = storageSpaceKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text);

                    foreach (var item in result)
                    {
                        childrenStorageSpaces.Add(item.ToContract());
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return childrenStorageSpaces;
        }

        Guid IStorageRepository.InsertStorageSpace(Context context, StorageSpace storageSpace)
        {
            Guard.ArgumentNotNull(() => context);
            Guard.ArgumentNotNull(() => storageSpace);

            Guid storageSpaceKey = default(Guid);

            try
            {
                using (var scope = TransactionScopeFactory.Create())                
                {
                    Guid? newKey = null;
                    newKey = _storageSpaceRepository.InsertStorageSpace(context.ToActionContext(), new StrgDAL.Models.StorageSpace
                    {
                        AnchorStorageSpaceKey = storageSpace.AnchorStrorageSpaceKey,
                        FacilityKey = storageSpace.FacilityKey,
                        MiniDrawerTrayModeInternalCode = storageSpace.MiniDrawerTrayMode != null ? storageSpace.MiniDrawerTrayMode.ToInternalCode() : default(string),
                        MobileFlag = storageSpace.Mobile,
                        MoreThanOneItemFlag = storageSpace.MoreThanOneItem,
                        ParentStorageSpaceKey = storageSpace.ParentStorageSpaceKey,
                        PendingMiniDrawerTrayModeInternalCode = storageSpace.PendingMiniDrawerTrayMode != null ? storageSpace.PendingMiniDrawerTrayMode.ToInternalCode() : default(string),
                        PositionId = storageSpace.PositionID,
                        ProductName = storageSpace.ProductName,
                        RegistrySerialId = storageSpace.RegistrySerialID,
                        RestrictedAccessFlag = storageSpace.RestrictedAccess,
                        SecondSystemBusDeviceKey = storageSpace.SecondSystemBusDeviceKey,
                        SerialId = storageSpace.SerialID,
                        StackedSerialId = storageSpace.StackedSerialID,
                        ShipToID = storageSpace.ShipToID,
                        InteriorCabinetLightIntensityModeInternalCode = storageSpace.InteriorCabinetLightIntensityModeInternalCode != null ? storageSpace.InteriorCabinetLightIntensityModeInternalCode.ToInternalCode() : default(string),
                        StorageSpaceTypeInternalCode = storageSpace.StorageSpaceTypeInternalCode.ToInternalCode(),
                        StorageSpaceFormKey = storageSpace.StorageSpaceFormKey,
                        StorageSpaceName = storageSpace.StorageSpaceName,
                        SystemBusDeviceKey = storageSpace.SystemBusDeviceKey,
                        UnavailableForInventoryFlag = storageSpace.UnavailableForInventory
                    });

                    if (newKey.HasValue)
                    {
                        storageSpaceKey = newKey.Value;

                        if (null != storageSpace.StorageSpaceState)
                        {
                            storageSpace.StorageSpaceState.StorageSpaceKey = newKey.Value;

                            Guid? newStateKey = null;
                            StorageSpaceState storageSpaceState = storageSpace.StorageSpaceState;

                            newStateKey = _storageSpaceStateRepository.InsertStorageSpaceState(context.ToActionContext(), new StrgDAL.Models.StorageSpaceState
                            {
                                ClosedFlag = storageSpaceState.Closed,
                                DefrostFlag = storageSpaceState.Defrost,
                                FailedFlag = storageSpaceState.Failed,
                                FailureRequiresMaintenanceFlag = storageSpaceState.FailureRequiresMaintenance,
                                LockedFlag = storageSpaceState.Locked,
                                OnBatteryFlag = storageSpaceState.OnBattery,
                                StorageSpaceAbbreviatedName = storageSpaceState.StorageSpaceAbbreviatedName,
                                StorageSpaceFailureReasonInternalCode = storageSpaceState.FailureReason != null ? storageSpaceState.FailureReason.InternalCode.ToInternalCode() : default(string),
                                StorageSpaceShortName = storageSpaceState.StorageSpaceShortName,
                                StorageSpaceKey = storageSpaceState.StorageSpaceKey
                            });
                        }
                    }

                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return storageSpaceKey;
        }

        void IStorageRepository.UpdateStorageSpace(Context context, StorageSpace storageSpace)
        {
            Guard.ArgumentNotNull(() => context);
            Guard.ArgumentNotNull(() => storageSpace);

            try
            {
                using (var tx = TransactionScopeFactory.Create())                
                {
                    _storageSpaceRepository.UpdateStorageSpace(context.ToActionContext(), new StrgDAL.Models.StorageSpace
                    {
                        AnchorStorageSpaceKey = storageSpace.AnchorStrorageSpaceKey,
                        FacilityKey = storageSpace.FacilityKey,
                        LastModifiedBinaryValue = storageSpace.LastModified,
                        MiniDrawerTrayModeInternalCode = storageSpace.PendingMiniDrawerTrayMode != null ? storageSpace.PendingMiniDrawerTrayMode.ToInternalCode() : default(string),
                        MobileFlag = storageSpace.Mobile,
                        MoreThanOneItemFlag = storageSpace.MoreThanOneItem,
                        ParentStorageSpaceKey = storageSpace.ParentStorageSpaceKey,
                        PendingMiniDrawerTrayModeInternalCode = storageSpace.PendingMiniDrawerTrayMode != null ? storageSpace.PendingMiniDrawerTrayMode.ToInternalCode() : default(string),
                        PositionId = storageSpace.PositionID,
                        ProductName = storageSpace.ProductName,
                        RegistrySerialId = storageSpace.RegistrySerialID,
                        RestrictedAccessFlag = storageSpace.RestrictedAccess,
                        SecondSystemBusDeviceKey = storageSpace.SecondSystemBusDeviceKey,
                        SerialId = storageSpace.SerialID,
                        StackedSerialId = storageSpace.StackedSerialID,
                        ShipToID = storageSpace.ShipToID,
                        InteriorCabinetLightIntensityModeInternalCode = storageSpace.InteriorCabinetLightIntensityModeInternalCode != null ? storageSpace.InteriorCabinetLightIntensityModeInternalCode.ToInternalCode() : default(string),
                        StorageSpaceFormKey = storageSpace.StorageSpaceFormKey,
                        StorageSpaceKey = storageSpace.Key,
                        StorageSpaceName = storageSpace.StorageSpaceName,
                        StorageSpaceTypeInternalCode = storageSpace.StorageSpaceTypeInternalCode.ToInternalCode(),
                        SystemBusDeviceKey = storageSpace.SystemBusDeviceKey,
                        UnavailableForInventoryFlag = storageSpace.UnavailableForInventory
                    });

                    if (storageSpace.StorageSpaceState != null)
                    {
                        StorageSpaceState storageSpaceState = storageSpace.StorageSpaceState;
                        _storageSpaceStateRepository.UpdateStorageSpaceState(context.ToActionContext(), new StrgDAL.Models.StorageSpaceState
                        {
                            ClosedFlag = storageSpaceState.Closed,
                            DefrostFlag = storageSpaceState.Defrost,
                            FailedFlag = storageSpaceState.Failed,
                            FailureRequiresMaintenanceFlag = storageSpaceState.FailureRequiresMaintenance,
                            LastModifiedBinaryValue = storageSpaceState.LastModified,
                            LockedFlag = storageSpaceState.Locked,
                            OnBatteryFlag = storageSpaceState.OnBattery,
                            StorageSpaceAbbreviatedName = storageSpaceState.StorageSpaceAbbreviatedName,
                            StorageSpaceFailureReasonInternalCode = storageSpaceState.FailureReason != null ? storageSpaceState.FailureReason.InternalCode.ToInternalCode() : default(string),
                            StorageSpaceKey = storageSpaceState.StorageSpaceKey,
                            StorageSpaceShortName = storageSpaceState.StorageSpaceShortName,
                            StorageSpaceStateKey = storageSpaceState.Key,
                        }, false);
                    }

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IStorageRepository.UpdateStorageSpaceDerivedValues(Context context, Guid storageSpaceKey)
        {
            Guard.ArgumentNotNull(() => context);

            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                connectionScope.Execute(
                    "Strg.bsp_UpdateStorageSpaceDerivedValues",
                    new
                    {
                        StorageSpaceKey = storageSpaceKey,
                        LastModifiedDispensingDeviceKey = (Guid?)context.Device
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure);
            }            
        }

        void IStorageRepository.UpdateStorageSpaceMiniDrawerTrayMode(
            Context context,
            Guid trayStorageSpaceKey,
            MiniDrawerTrayModeInternalCode? pendingMiniDrawerTrayMode,
            MiniDrawerTrayModeInternalCode? miniDrawerTrayMode)
        {
            Guard.ArgumentNotNull(() => context);

            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                connectionScope.Execute(
                    "Strg.bsp_StorageSpaceMiniDrawerTrayModeUpdate",
                    new
                    {
                        StartUTCDateTime = context.ActionUtcDateTime,
                        StartLocalDateTime = context.ActionDateTime,
                        TrayStorageSpaceKey = trayStorageSpaceKey,
                        MiniDrawerTrayModeInternalCode = miniDrawerTrayMode.ToInternalCode(),
                        PendingMiniDrawerTrayModeInternalCode = pendingMiniDrawerTrayMode.ToInternalCode(),
                        LastModifiedDispensingDeviceKey = (Guid?)context.Device,
                        LastModifiedActorKey = (Guid?)context.Actor
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure);
            }
        }

        void IStorageRepository.UpdateStorageSpaceAnchor(Context context, Guid storageSpaceKey, Guid? anchorStorageSpaceKey, bool unavailableForInventory)
        {
            Guard.ArgumentNotNull(() => context);

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Strg.usp_StorageSpaceAnchorUpdate",
                        new
                        {
                            StartUTCDateTime = context.ActionUtcDateTime,
                            StartLocalDateTime = context.ActionDateTime,
                            AnchorStorageSpaceKey = anchorStorageSpaceKey,
                            UnavailableForInventoryFlag = unavailableForInventory,
                            LastModifiedDispensingDeviceKey = (Guid?)context.Device,
                            LastModifiedActorKey = (Guid?)context.Actor,
                            LastModifiedBinaryValue = default(byte?),
                            CheckConcurrency = false,
                            StorageSpaceKey = storageSpaceKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IStorageRepository.DeleteStorageSpace(Context context, Guid storageSpaceKey)
        {
            Guard.ArgumentNotNull(() => context);

            try
            {
                _storageSpaceRepository.DeleteStorageSpace(context.ToActionContext(), storageSpaceKey);                
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        StorageSpaceState IStorageRepository.GetStorageSpaceState(Guid storageSpaceStateKey)
        {
            StorageSpaceState state = null;

            try
            {
                using(var connectionScope = ConnectionScopeFactory.Create())
                {
                    var sql = new SqlBuilder();
                    sql.SELECT()
                        ._("sst.StorageSpaceStateKey")
                        ._("sst.StorageSpaceKey")
                        ._("sst.ClosedFlag")
                        ._("sst.LockedFlag ")
                        ._("sst.FailedFlag")
                        ._("sst.DefrostFlag")
                        ._("sst.OnBatteryFlag")
                        ._("sst.FailureRequiresMaintenanceFlag")
                        ._("sst.StorageSpaceFailureReasonInternalCode")
                        ._("sst.StorageSpaceShortName")
                        ._("sst.StorageSpaceAbbreviatedName")
                        ._("sst.LastModifiedActorKey")
                        ._("sst.LastModifiedBinaryValue")
                        ._("sfr.StorageSpaceFailureReasonInternalCode")
                        ._("sfr.DescriptionText")
                        ._("sfr.SortValue")
                        .FROM("Strg.StorageSpaceState sst")
                        .JOIN("Strg.StorageSpaceSnapshot sss ON sss.StorageSpaceKey = sst.StorageSpaceKey AND sss.EndUTCDateTime IS NULL AND sss.DeleteFlag = 0")
                        .LEFT_JOIN("Strg.StorageSpaceFailureReason sfr ON sst.StorageSpaceFailureReasonInternalCode = sfr.StorageSpaceFailureReasonInternalCode")
                        .WHERE("sst.EndUTCDateTime IS NULL")
                        .WHERE("sst.StorageSpaceStateKey = @StorageSpaceStateKey");

                    state = connectionScope.QueryFirstOrDefault<StorageSpaceStateResult>(
                            sql.ToString(),
                            new
                            {
                                StorageSpaceStateKey = storageSpaceStateKey
                            },
                            commandTimeout: connectionScope.DefaultCommandTimeout,
                            commandType: CommandType.Text)
                            ?.ToContract();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return state;
        }

        StorageSpaceState IStorageRepository.GetStorageSpaceStateByStorageSpace(Guid storageSpaceKey)
        {
            StorageSpaceState state = null;

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var sql = new SqlBuilder();
                    sql.SELECT()
                        ._("sst.StorageSpaceStateKey")
                        ._("sst.StorageSpaceKey")
                        ._("sst.ClosedFlag")
                        ._("sst.LockedFlag ")
                        ._("sst.FailedFlag")
                        ._("sst.DefrostFlag")
                        ._("sst.OnBatteryFlag")
                        ._("sst.FailureRequiresMaintenanceFlag")
                        ._("sst.StorageSpaceFailureReasonInternalCode")
                        ._("sst.StorageSpaceShortName")
                        ._("sst.StorageSpaceAbbreviatedName")
                        ._("sst.LastModifiedActorKey")
                        ._("sst.LastModifiedBinaryValue")
                        ._("sfr.StorageSpaceFailureReasonInternalCode")
                        ._("sfr.DescriptionText")
                        ._("sfr.SortValue")
                        .FROM("Strg.StorageSpaceState sst")
                        .JOIN("Strg.StorageSpaceSnapshot sss ON sss.StorageSpaceKey = sst.StorageSpaceKey AND sss.EndUTCDateTime IS NULL AND sss.DeleteFlag = 0")
                        .LEFT_JOIN("Strg.StorageSpaceFailureReason sfr ON sst.StorageSpaceFailureReasonInternalCode = sfr.StorageSpaceFailureReasonInternalCode")
                        .WHERE("sst.EndUTCDateTime IS NULL")
                        .WHERE("sst.StorageSpaceKey = @StorageSpaceKey");

                    state = connectionScope.QueryFirstOrDefault<StorageSpaceStateResult>(
                            sql.ToString(),
                            new
                            {
                                StorageSpaceKey = storageSpaceKey
                            },
                            commandTimeout: connectionScope.DefaultCommandTimeout,
                            commandType: CommandType.Text)
                            ?.ToContract();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return state;
        }

        Guid IStorageRepository.InsertStorageSpaceState(Context context, StorageSpaceState storageSpaceState)
        {
            Guard.ArgumentNotNull(() => context);
            Guard.ArgumentNotNull(() => storageSpaceState);

            Guid? storageSpaceStateKey = default(Guid?);

            try
            {
                storageSpaceStateKey = _storageSpaceStateRepository.InsertStorageSpaceState(context.ToActionContext(), new StrgDAL.Models.StorageSpaceState
                {
                    ClosedFlag = storageSpaceState.Closed,
                    DefrostFlag = storageSpaceState.Defrost,
                    OnBatteryFlag = storageSpaceState.OnBattery,
                    FailedFlag = storageSpaceState.Failed,
                    FailureRequiresMaintenanceFlag = storageSpaceState.FailureRequiresMaintenance,
                    LockedFlag = storageSpaceState.Locked,
                    StorageSpaceAbbreviatedName = storageSpaceState.StorageSpaceAbbreviatedName,
                    StorageSpaceKey = storageSpaceState.StorageSpaceKey,
                    StorageSpaceShortName = storageSpaceState.StorageSpaceShortName,
                    StorageSpaceFailureReasonInternalCode = storageSpaceState.FailureReason != null ? storageSpaceState.FailureReason.InternalCode.ToInternalCode() : default(string)
                });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return storageSpaceStateKey.GetValueOrDefault();
        }

        void IStorageRepository.UpdateStorageSpaceState(Context context, StorageSpaceState storageSpaceState)
        {
            Guard.ArgumentNotNull(() => context);

            _storageSpaceStateRepository.UpdateStorageSpaceState(context.ToActionContext(), new StrgDAL.Models.StorageSpaceState
            {
                ClosedFlag = storageSpaceState.Closed,
                DefrostFlag = storageSpaceState.Defrost,
                OnBatteryFlag = storageSpaceState.OnBattery,
                FailedFlag = storageSpaceState.Failed,
                FailureRequiresMaintenanceFlag = storageSpaceState.FailureRequiresMaintenance,
                LockedFlag = storageSpaceState.Locked,
                StorageSpaceAbbreviatedName = storageSpaceState.StorageSpaceAbbreviatedName,
                StorageSpaceKey = storageSpaceState.StorageSpaceKey,
                StorageSpaceShortName = storageSpaceState.StorageSpaceShortName,
                StorageSpaceFailureReasonInternalCode = storageSpaceState.FailureReason != null ? storageSpaceState.FailureReason.InternalCode.ToInternalCode() : default(string),
                LastModifiedBinaryValue = storageSpaceState.LastModified,                
            }, false);
        }

        private SqlBuilder GetStorageSpaceQuery()
        {
            var sql = GetStorageSpaceBaseQuery();
            sql.WHERE("sss.EndUTCDateTime IS NULL");
            sql.WHERE("sss.DeleteFlag = 0");

            return sql;
        }

        private SqlBuilder GetStorageSpaceBaseQuery()
        {
            var sql = new SqlBuilder();
            sql.SELECT()
                ._("sss.StorageSpaceSnapshotKey")
                ._("sss.StorageSpaceKey")
                ._("sss.FacilityKey")
                ._("sss.StorageSpaceTypeInternalCode ")
                ._("sss.StorageSpaceFormKey")
                ._("ssf.DescriptionText AS StorageSpaceFormDescription")
                ._("sss.ParentStorageSpaceKey")
                ._("sss.StorageSpaceName")
                ._("sss.SerialID")
                ._("sss.RegistrySerialID")
                ._("sss.StackedSerialID")
                ._("sss.ProductName")
                ._("sss.PositionID")
                ._("sss.AnchorStorageSpaceKey")
                ._("sss.SystemBusDeviceKey")
                ._("sss.SecondSystemBusDeviceKey")
                ._("sss.MiniDrawerTrayModeInternalCode")
                ._("sss.PendingMiniDrawerTrayModeInternalCode")
                ._("sss.InteriorCabinetLightIntensityModeInternalCode")
                ._("sss.MoreThanOneItemFlag")
                ._("sss.UnavailableForInventoryFlag")
                ._("sss.RestrictedAccessFlag")
                ._("sss.LastModifiedBinaryValue")
                ._("stt.StorageSpaceStateKey")
                ._("stt.ClosedFlag")
                ._("stt.LockedFlag")
                ._("stt.FailedFlag")
                ._("stt.DefrostFlag")
                ._("stt.OnBatteryFlag")
                ._("stt.FailureRequiresMaintenanceFlag")
                ._("stt.StorageSpaceFailureReasonInternalCode")
                ._("sfr.DescriptionText AS StorageSpaceFailureReasonDescription")
                ._("stt.LastModifiedBinaryValue AS StorageSpaceLastModified")
                ._("ssf.StorageSpaceFormInternalCode")                
                ._("ss.SortValue")
                ._("ss.StorageSpaceShortName")
                ._("ss.StorageSpaceAbbreviatedName")                
                ._("ss.LevelNumber")                
                ._("ss.ParentMiniDrawerTrayModeInternalCode")
                ._("ss.ParentPendingMiniDrawerTrayModeInternalCode")                
                ._("ss.FailureInHierarchyFlag")
                ._("ss.DefrostInHierarchyFlag")
                ._("ddss.DispensingDeviceKey")
                .FROM("Strg.StorageSpaceSnapshot sss")
                .JOIN("Strg.StorageSpace ss ON sss.StorageSpaceKey = ss.StorageSpaceKey")
                .LEFT_JOIN("Strg.StorageSpaceState stt ON stt.StorageSpaceKey = sss.StorageSpaceKey AND stt.EndUTCDateTime IS NULL")
                .LEFT_JOIN("Strg.StorageSpaceForm ssf ON ssf.StorageSpaceFormKey = sss.StorageSpaceFormKey")
                .LEFT_JOIN("Strg.StorageSpaceFailureReason sfr ON sfr.StorageSpaceFailureReasonInternalCode = stt.StorageSpaceFailureReasonInternalCode")
                .LEFT_JOIN("Strg.DispensingDeviceStorageSpace ddss ON ddss.StorageSpaceKey = ss.StorageSpaceKey AND ddss.DisassociationUTCDateTime IS NULL");                

            return sql;
        }
        #endregion

        #region Storage Space Item Members
        StorageSpaceItem IStorageRepository.GetStorageSpaceItem(Guid storageSpaceItemKey)
        {
            StorageSpaceItem storageSpaceItem = null;

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var sql = new SqlBuilder();
                    sql.SELECT()
                        ._("spt.StorageSpaceItemSnapshotKey")
                        ._("itm.StorageSpaceItemKey")
                        ._("itm.StorageSpaceKey")
                        ._("itm.ItemKey")
                        ._("itm.DispensingDeviceEjectWithInventoryFlag")                                                                                      
                        ._("itm.LastDispenseUTCDateTime")
                        ._("itm.LastDispenseLocalDateTime")
                        ._("itm.LastInventoryUTCDateTime")
                        ._("itm.LastInventoryLocalDateTime")
                        ._("itm.LastLoadRefillUTCDateTime")
                        ._("itm.LastLoadRefillLocalDateTime")
                        ._("itm.LastLoadRefillSuccessfulScanFlag")
                        ._("itm.LastLoadRefillActorKey")
                        ._("itm.LastKnownLocalDateTime")
                        ._("itm.LastKnownUtcDateTime")
                        ._("itm.LastRxCheckLocalDateTime")
                        ._("itm.LastRxCheckUtcDateTime")
                        ._("itm.LoadUTCDateTime")
                        ._("itm.LoadLocalDateTime")
                        ._("itm.PendedAtServerFlag")
                        ._("spt.FromExternalSystemFlag")
                        ._("spt.StandardStockWithinDispensingDeviceFlag")
                        ._("spt.IssueUOMKey")
                        ._("spt.OutdateTrackingFlag")
                        ._("spt.ParQuantity")
                        ._("spt.DispenseFractionFlag")
                        ._("spt.PhysicalMaximumQuantity")
                        ._("spt.RefillPointQuantity")
                        ._("spt.CriticalLowQuantity")
                        ._("spt.SystemBusDeviceKey")
                        ._("spt.LastModifiedBinaryValue")
                        ._("spt.LastModifiedActorKey")
                        ._("StorageSpaceItemStatusInternalCode")
                        ._("inv.InventoryQuantity")
                        ._("inv.EarliestNextExpirationDate")
                        .FROM("Strg.StorageSpaceItem itm")
                        .INNER_JOIN("Strg.StorageSpaceItemSnapshot spt ON itm.StorageSpaceItemKey = spt.StorageSpaceItemKey AND spt.EndUTCDateTime IS NULL")
                        .INNER_JOIN("Strg.StorageSpaceSnapshot sst ON itm.StorageSpaceKey = sst.StorageSpaceKey AND sst.EndUTCDateTime IS NULL AND sst.DeleteFlag = 0")
                        .INNER_JOIN("Item.ItemSnapshot its ON itm.ItemKey = its.ItemKey AND its.EndUTCDateTime IS NULL AND its.DeleteFlag = 0")
                        .INNER_JOIN("Tx.StorageSpaceInventory inv ON itm.StorageSpaceItemKey = inv.StorageSpaceItemKey AND inv.EndUTCDateTime IS NULL")                        
                        .WHERE("itm.StorageSpaceItemKey = @StorageSpaceItemKey");

                    var result = connectionScope.QueryFirstOrDefault<StorageSpaceItemResult>(
                            sql.ToString(),
                            new
                            {
                                StorageSpaceItemKey = storageSpaceItemKey
                            },
                            commandTimeout: connectionScope.DefaultCommandTimeout,
                            commandType: CommandType.Text);

                    if (result != null)
                    {
                        storageSpaceItem = new StorageSpaceItem(result.StorageSpaceItemKey, result.StorageSpaceItemSnapshotKey)
                        {
                            CriticalLowQuantity = result.CriticalLowQuantity,
                            DispenseFraction = result.DispenseFractionFlag,
                            DispensingDeviceEjectWithInventory = result.DispensingDeviceEjectWithInventoryFlag,
                            EarliestNextExpirationDate = result.EarliestNextExpirationDate,
                            FromExternalSystem = result.FromExternalSystemFlag,
                            InventoryQuantity = result.InventoryQuantity,
                            IsStandardStockWithinDispensingDevice = result.StandardStockWithinDispensingDeviceFlag,
                            IssueUnitOfMeasureKey = result.IssueUOMKey,
                            ItemKey = result.ItemKey,                                                 
                            LastDispenseDateTime = result.LastDispenseLocalDateTime,
                            LastDispenseUtcDateTime = result.LastDispenseUtcDateTime,
                            LastInventoryDateTime = result.LastInventoryLocalDateTime,
                            LastInventoryUtcDateTime = result.LastInventoryUtcDateTime,
                            LastKnownDateTime = result.LastKnownLocalDateTime,
                            LastKnownUtcDateTime = result.LastKnownUtcDateTime,
                            LastLoadRefillActorKey = result.LastLoadRefillActorKey,
                            LastLoadRefillDateTime = result.LastLoadRefillLocalDateTime,
                            LastLoadRefillUtcDateTime = result.LastLoadRefillUtcDateTime,
                            LastLoadRefillSuccessfulScan = result.LastLoadRefillSuccessfulScanFlag,
                            LastModified = result.LastModifiedBinaryValue,
                            LastRxCheckDateTime = result.LastRxCheckLocalDateTime,
                            LastRxCheckUTCDateTime = result.LastRxCheckUtcDateTime,
                            LoadDateTime = result.LoadLocalDateTime,
                            LoadUtcDateTime = result.LoadUtcDateTime,
                            OutdateTracking = result.OutdateTrackingFlag,
                            ParQuantity = result.ParQuantity,
                            PendedAtServer = result.PendedAtServerFlag,
                            PhysicalMaximumQuantity = result.PhysicalMaximumQuantity,
                            RefillPointQuantity = result.RefillPointQuantity,                            
                            StorageSpaceItemStatus = result.StorageSpaceItemStatusInternalCode.FromInternalCode<StorageSpaceItemStatusInternalCode>(),
                            StorageSpaceKey = result.StorageSpaceKey,
                            SystemBusDeviceKey = result.SystemBusDeviceKey
                        };
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return storageSpaceItem;
        }

        public Guid InsertStorageSpaceItem(Context context, StorageSpaceItem storageSpaceItem)
        {
            Guid? storageSpaceItemKey = null;

            try
            {
                storageSpaceItemKey = _storageSpaceItemRepository.InsertStorageSpaceItem(context.ToActionContext(), storageSpaceItem.ToModel());
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return storageSpaceItemKey.GetValueOrDefault();
        }

        public Guid InsertDissassociatedStorageSpaceItem(Context context, StorageSpaceItem storageSpaceItem)
        {
            Guid? storageSpaceItemKey = null;

            try
            {
                storageSpaceItemKey = _storageSpaceItemRepository.InsertStorageSpaceItem(context.ToActionContext(), storageSpaceItem.ToModel(), true);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return storageSpaceItemKey.GetValueOrDefault();
        }

        void IStorageRepository.UpdateStorageSpaceItem(Context context, StorageSpaceItem storageSpaceItem)
        {
            Guard.ArgumentNotNull(() => context);
            Guard.ArgumentNotNull(() => storageSpaceItem);

            try
            {
                _storageSpaceItemRepository.UpdateStorageSpaceItem(context.ToActionContext(), storageSpaceItem.ToModel());                
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        public void UpdateStorageSpaceItemDispenseActivity(Context context, Guid storageSpaceItemKey,
            DateTime activityLocalDateTime, DateTime activityUtcDateTime)
        {
            UpdateStorageSpaceItemDispenseActivity(
                (Guid?) context.Device,
                storageSpaceItemKey,
                activityLocalDateTime,
                activityUtcDateTime);
        }

        public void UpdateStorageSpaceItemDispenseActivity(Guid? deviceKey, Guid storageSpaceItemKey, DateTime activityLocalDateTime, DateTime activityUtcDateTime)
        {
            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Strg.usp_StorageSpaceItemParentUpdate",
                        new
                        {
                            LastDispenseUTCDateTime = activityUtcDateTime,
                            LastDispenseUTCDateTimeSpecifiedFlag = true,
                            LastDispenseLocalDateTime = activityLocalDateTime,
                            LastDispenseLocalDateTimeSpecifiedFlag = true,
                            LastInventoryUTCDateTime = default(DateTime?),
                            LastInventoryUTCDateTimeSpecifiedFlag = false,
                            LastInventoryLocalDateTime = default(DateTime?),
                            LastInventoryLocalDateTimeSpecifiedFlag = false,
                            LastLoadRefillUTCDateTime = default(DateTime?),
                            LastLoadRefillUTCDateTimeSpecifiedFlag = false,
                            LastLoadRefillLocalDateTime = default(DateTime?),
                            LastLoadRefillLocalDateTimeSpecifiedFlag = false,
                            LastLoadRefillSuccessfulScanFlag = default(bool?),
                            LastLoadRefillSuccessfulScanFlagSpecifiedFlag = false,
                            LastLoadRefillActorKey = default(Guid?),
                            LastLoadRefillActorKeySpecifiedFlag = false,
                            LastRxCheckUTCDateTime = default(DateTime?),
                            LastRxCheckUTCDateTimeSpecifiedFlag = false,
                            LastRxCheckLocalDateTime = default(DateTime?),
                            LastRxCheckLocalDateTimeSpecifiedFlag = false,
                            LoadUTCDateTime = default(DateTime?),
                            LoadUTCDateTimeSpecifiedFlag = false,
                            LoadLocalDateTime = default(DateTime?),
                            LoadLocalDateTimeSpecifiedFlag = false,
                            LastKnownUTCDateTime = activityUtcDateTime,
                            LastKnownUTCDateTimeSpecifiedFlag = true,
                            LastKnownLocalDateTime = activityLocalDateTime,
                            LastKnownLocalDateTimeSpecifiedFlag = true,
                            DispensingDeviceEjectWithInventoryFlag = false,
                            DispensingDeviceEjectWithInventoryFlagSpecifiedFlag = false,
                            LastModifiedDispensingDeviceKey = deviceKey,
                            StorageSpaceItemKey = storageSpaceItemKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IStorageRepository.UpdateStorageSpaceItemInventoryActivity(Context context, Guid storageSpaceItemKey, DateTime activityLocalDateTime, DateTime activityUtcDateTime)
        {
            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Strg.usp_StorageSpaceItemParentUpdate",
                        new
                        {
                            LastDispenseUTCDateTime = default(DateTime?),
                            LastDispenseUTCDateTimeSpecifiedFlag = false,
                            LastDispenseLocalDateTime = default(DateTime?),
                            LastDispenseLocalDateTimeSpecifiedFlag = false,
                            LastInventoryUTCDateTime = activityUtcDateTime,
                            LastInventoryUTCDateTimeSpecifiedFlag = true,
                            LastInventoryLocalDateTime = activityLocalDateTime,
                            LastInventoryLocalDateTimeSpecifiedFlag = true,
                            LastLoadRefillUTCDateTime = default(DateTime?),
                            LastLoadRefillUTCDateTimeSpecifiedFlag = false,
                            LastLoadRefillLocalDateTime = default(DateTime?),
                            LastLoadRefillLocalDateTimeSpecifiedFlag = false,
                            LastLoadRefillSuccessfulScanFlag = default(bool?),
                            LastLoadRefillSuccessfulScanFlagSpecifiedFlag = false,
                            LastLoadRefillActorKey = default(Guid?),
                            LastLoadRefillActorKeySpecifiedFlag = false,
                            LastRxCheckUTCDateTime = default(DateTime?),
                            LastRxCheckUTCDateTimeSpecifiedFlag = false,
                            LastRxCheckLocalDateTime = default(DateTime?),
                            LastRxCheckLocalDateTimeSpecifiedFlag = false,
                            LoadUTCDateTime = default(DateTime?),
                            LoadUTCDateTimeSpecifiedFlag = false,
                            LoadLocalDateTime = default(DateTime?),
                            LoadLocalDateTimeSpecifiedFlag = false,
                            LastKnownUTCDateTime = activityUtcDateTime,
                            LastKnownUTCDateTimeSpecifiedFlag = true,
                            LastKnownLocalDateTime = activityLocalDateTime,
                            LastKnownLocalDateTimeSpecifiedFlag = true,
                            DispensingDeviceEjectWithInventoryFlag = false,
                            DispensingDeviceEjectWithInventoryFlagSpecifiedFlag = false,
                            LastModifiedDispensingDeviceKey = (Guid?)context.Device,
                            StorageSpaceItemKey = storageSpaceItemKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IStorageRepository.UpdateStorageSpaceItemLoadRefillActivity(Context context, Guid storageSpaceItemKey, bool isLoadTransaction, DateTime activityLocalDateTime, DateTime activityUtcDateTime, bool loadRefillSuccessfulScan,
             DateTime? lastLoadrefillLocalDateTime, DateTime? lastLoadrefillUtcDateTime, DateTime? lastLoadLocalDateTime, DateTime? lastLoadUtcDateTime)
        {
            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Strg.usp_StorageSpaceItemParentUpdate",
                        new
                        {
                            LastDispenseUTCDateTime = default(DateTime?),
                            LastDispenseUTCDateTimeSpecifiedFlag = false,
                            LastDispenseLocalDateTime = default(DateTime?),
                            LastDispenseLocalDateTimeSpecifiedFlag = false,
                            LastInventoryUTCDateTime = default(DateTime?),
                            LastInventoryUTCDateTimeSpecifiedFlag = false,
                            LastInventoryLocalDateTime = default(DateTime?),
                            LastInventoryLocalDateTimeSpecifiedFlag = false,
                            LastLoadRefillUTCDateTime = lastLoadrefillUtcDateTime ?? activityUtcDateTime,
                            LastLoadRefillUTCDateTimeSpecifiedFlag = true,
                            LastLoadRefillLocalDateTime = lastLoadrefillLocalDateTime ?? activityLocalDateTime,
                            LastLoadRefillLocalDateTimeSpecifiedFlag = true,
                            LastLoadRefillSuccessfulScanFlag = loadRefillSuccessfulScan,
                            LastLoadRefillSuccessfulScanFlagSpecifiedFlag = true,
                            LastLoadRefillActorKey = (Guid?)context.User,
                            LastLoadRefillActorKeySpecifiedFlag = true,
                            LastRxCheckUTCDateTime = default(DateTime?),
                            LastRxCheckUTCDateTimeSpecifiedFlag = false,
                            LastRxCheckLocalDateTime = default(DateTime?),
                            LastRxCheckLocalDateTimeSpecifiedFlag = false,
                            LoadUTCDateTime = isLoadTransaction ? (lastLoadUtcDateTime ?? activityUtcDateTime) : default(DateTime?),
                            LoadUTCDateTimeSpecifiedFlag = isLoadTransaction,
                            LoadLocalDateTime = isLoadTransaction ? (lastLoadLocalDateTime ?? activityLocalDateTime) : default(DateTime?),
                            LoadLocalDateTimeSpecifiedFlag = isLoadTransaction,
                            LastKnownUTCDateTime = activityUtcDateTime,
                            LastKnownUTCDateTimeSpecifiedFlag = true,
                            LastKnownLocalDateTime = activityLocalDateTime,
                            LastKnownLocalDateTimeSpecifiedFlag = true,
                            DispensingDeviceEjectWithInventoryFlag = false,
                            DispensingDeviceEjectWithInventoryFlagSpecifiedFlag = false,
                            LastModifiedDispensingDeviceKey = (Guid?)context.Device,
                            StorageSpaceItemKey = storageSpaceItemKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }                
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IStorageRepository.UpdateStorageSpaceItemsFractionalFlag(Context context, Guid dispensingDeviceKey, Guid itemKey, bool dispenseFractional)
        {
            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Strg.bsp_StorageSpaceItemDispenseFractionalFlagUpdate",
                        new
                        {
                            StartUTCDateTime = context.ActionUtcDateTime,
                            StartLocalDateTime = context.ActionDateTime,
                            DispensingDeviceKey = dispensingDeviceKey,
                            ItemKey = itemKey,
                            DispenseFractionFlag = dispenseFractional,
                            LastModifiedDispensingDeviceKey = (Guid?)context.Device,
                            LastModifiedActorKey = (Guid?)context.Actor
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IStorageRepository.UpdateStorageSpaceItemRxCheckLoadRefillActivity(Context context, Guid storageSpaceItemKey, DateTime rxCheckLocalDateTime, DateTime rxCheckUtcDateTime)
        {
            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Strg.usp_StorageSpaceItemParentUpdate",
                        new
                        {
                            LastDispenseUTCDateTime = default(DateTime?),
                            LastDispenseUTCDateTimeSpecifiedFlag = false,
                            LastDispenseLocalDateTime = default(DateTime?),
                            LastDispenseLocalDateTimeSpecifiedFlag = false,
                            LastInventoryUTCDateTime = default(DateTime?),
                            LastInventoryUTCDateTimeSpecifiedFlag = false,
                            LastInventoryLocalDateTime = default(DateTime?),
                            LastInventoryLocalDateTimeSpecifiedFlag = false,
                            LastLoadRefillUTCDateTime = default(DateTime?),
                            LastLoadRefillUTCDateTimeSpecifiedFlag = false,
                            LastLoadRefillLocalDateTime = default(DateTime?),
                            LastLoadRefillLocalDateTimeSpecifiedFlag = false,
                            LastLoadRefillSuccessfulScanFlag = false,
                            LastLoadRefillSuccessfulScanFlagSpecifiedFlag = false,
                            LastLoadRefillActorKey = default(Guid?),
                            LastLoadRefillActorKeySpecifiedFlag = false,
                            LastRxCheckUTCDateTime = rxCheckUtcDateTime,
                            LastRxCheckUTCDateTimeSpecifiedFlag = true,
                            LastRxCheckLocalDateTime = rxCheckLocalDateTime,
                            LastRxCheckLocalDateTimeSpecifiedFlag = true,
                            LoadUTCDateTime = default(DateTime?),
                            LoadUTCDateTimeSpecifiedFlag = false,
                            LoadLocalDateTime = default(DateTime?),
                            LoadLocalDateTimeSpecifiedFlag = false,
                            LastKnownUTCDateTime = rxCheckUtcDateTime,
                            LastKnownUTCDateTimeSpecifiedFlag = true,
                            LastKnownLocalDateTime = rxCheckLocalDateTime,
                            LastKnownLocalDateTimeSpecifiedFlag = true,
                            DispensingDeviceEjectWithInventoryFlag = false,
                            DispensingDeviceEjectWithInventoryFlagSpecifiedFlag = false,
                            LastModifiedDispensingDeviceKey = (Guid?)context.Device,
                            StorageSpaceItemKey = storageSpaceItemKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IStorageRepository.DeleteStorageSpaceItem(Context context, Guid storageSpaceKey, Guid itemKey, bool dispensingDeviceEjectWithInventoryFlag /* = false */)
        {
            Guard.ArgumentNotNull(() => context);

            try
            {
                // storageSpaceItemDelete removes active row from storageSpaceInventory as well
                _storageSpaceItemRepository.DeleteStorageSpaceItem(context.ToActionContext(), storageSpaceKey, itemKey, dispensingDeviceEjectWithInventoryFlag);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }
        #endregion

        #region CriticalOverrideMode Members

        void IStorageRepository.InsertCriticalOverrideMode(Context context, Guid dispensingDeviceKey, CriticalOverrideModeReasonInternalCode criticalOverrideReason)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Strg.usp_DispensingDeviceCriticalOverrideModeReasonInsert",
                        new
                        {
                            DispensingDeviceKey = dispensingDeviceKey,
                            CriticalOverrideModeReasonInternalCode = criticalOverrideReason.ToInternalCode(),
                            AssociationUTCDateTime = context.ActionUtcDateTime,
                            AssociationLocalDateTime = context.ActionDateTime,
                            AssociationActorKey = (Guid?)context.Actor
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }                
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IStorageRepository.DeleteCriticalOverrideMode(Context context, Guid dispensingDeviceKey, CriticalOverrideModeReasonInternalCode criticalOverrideReason)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Strg.usp_DispensingDeviceCriticalOverrideModeReasonDelete",
                        new
                        {
                            DispensingDeviceKey = dispensingDeviceKey,
                            CriticalOverrideModeReasonInternalCode = criticalOverrideReason.ToInternalCode(),
                            DisassociationUTCDateTime = context.ActionUtcDateTime,
                            DisassociationLocalDateTime = context.ActionDateTime                            
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region Private Members

        private void InsertDispensingDeviceCriticalOverridePeriods(Context context, Guid dispensingDeviceKey, IEnumerable<CriticalOverridePeriod> criticalOverridePeriods)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(criticalOverridePeriods, "criticalOverridePeriods");

            Guid? key = null;
            foreach (CriticalOverridePeriod criticalOverridePeriod in criticalOverridePeriods)
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Strg.usp_CriticalOverridePeriodInsert",
                        new
                        {
                            StartUTCDateTime = context.ActionUtcDateTime,
                            StartLocalDateTime = context.ActionDateTime,
                            DispensingDeviceKey = dispensingDeviceKey,
                            CriticalOverridePeriodName = criticalOverridePeriod.Name,
                            StartTimeOfDayValue = criticalOverridePeriod.StartTimeOfDay,
                            EndTimeOfDayValue = criticalOverridePeriod.EndTimeOfDay,
                            MondayFlag = criticalOverridePeriod.Monday,
                            TuesdayFlag = criticalOverridePeriod.Tuesday,
                            WednesdayFlag = criticalOverridePeriod.Wednesday,
                            ThursdayFlag = criticalOverridePeriod.Thursday,
                            FridayFlag = criticalOverridePeriod.Friday,
                            SaturdayFlag = criticalOverridePeriod.Saturday,
                            SundayFlag = criticalOverridePeriod.Sunday,
                            CreatedUTCDateTime = criticalOverridePeriod.CreatedUTCDateTime,
                            CreatedLocalDateTime = criticalOverridePeriod.CreatedLocalDateTime,
                            CreatedActorKey = criticalOverridePeriod.CreatedActorKey,
                            LastModifiedActorKey = (Guid?)context.Actor,
                            CriticalOverridePeriodKey = key
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }                
            }
        }

        private void UpdateDispensingDeviceCriticalOverridePeriods(Context context, Guid dispensingDeviceKey, IEnumerable<CriticalOverridePeriod> criticalOverridePeriods)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(criticalOverridePeriods, "criticalOverridePeriods");

            // Get the list of critical override period keys associated with this device.
            IEnumerable<Guid> currentCriticalOverridePeriodKeys;
            using (var connectionScope = ConnectionScopeFactory.Create())
            {
                var query = new SqlBuilder();
                query.SELECT("cop.CriticalOverridePeriodKey")
                    .FROM("Strg.CriticalOverridePeriodSnapshot cop")
                    .WHERE("cop.DispensingDeviceKey = @DispensingDeviceKey")
                    ._("cop.DeleteFlag = 0")
                    ._("cop.EndUTCDateTime IS NULL");

                currentCriticalOverridePeriodKeys = connectionScope.Query<Guid>(
                    query.ToString(),
                    new
                    {
                        DispensingDeviceKey = dispensingDeviceKey,
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.Text);
            }

            // Find the critical override period keys that were removed.
            IEnumerable<Guid> removedCriticalOverridePeriodKeys = currentCriticalOverridePeriodKeys
                .Except(criticalOverridePeriods.Select(cop => cop.Key));

            // Remove critical override that are no longer associated with this device.
            foreach (Guid criticalOverridePeriodKey in removedCriticalOverridePeriodKeys)
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(new Dictionary<string, object>{
                        { "@StartUTCDateTime",  context.ActionUtcDateTime},
                        { "@StartLocalDateTime", context.ActionDateTime },
                        { "@LastModifiedActorKey",  (Guid?)context.Actor},
                        { "@CriticalOverridePeriodKey", criticalOverridePeriodKey },
                    });

                    connectionScope.Execute(
                       "Strg.usp_CriticalOverridePeriodDelete",
                       parameters,
                       commandTimeout: connectionScope.DefaultCommandTimeout,
                       commandType: CommandType.StoredProcedure);
                }
            }

            //Find the critical override periods that were added or updated.
            List<CriticalOverridePeriod> addedCrtiticalOverridePeriods = new List<CriticalOverridePeriod>();
            foreach (CriticalOverridePeriod criticalOverridePeriod in criticalOverridePeriods)
            {
                if (criticalOverridePeriod.IsTransient())
                {
                    addedCrtiticalOverridePeriods.Add(criticalOverridePeriod);
                    continue;
                }

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(new Dictionary<string, object>
                    {
                        {"@StartUTCDateTime", context.ActionUtcDateTime },
                        {"@StartLocalDateTime", context.ActionDateTime },
                        {"@DispensingDeviceKey", dispensingDeviceKey },
                        {"@CriticalOverridePeriodName", criticalOverridePeriod.Name },
                        {"@StartTimeOfDayValue", criticalOverridePeriod.StartTimeOfDay },
                        {"@EndTimeOfDayValue", criticalOverridePeriod.EndTimeOfDay },
                        {"@MondayFlag", criticalOverridePeriod.Monday },
                        {"@TuesdayFlag", criticalOverridePeriod.Tuesday },
                        {"@WednesdayFlag", criticalOverridePeriod.Wednesday },
                        {"@ThursdayFlag", criticalOverridePeriod.Thursday },
                        {"@FridayFlag", criticalOverridePeriod.Friday },
                        {"@SaturdayFlag", criticalOverridePeriod.Saturday },
                        {"@SundayFlag", criticalOverridePeriod.Sunday },
                        {"@CreatedUTCDateTime", criticalOverridePeriod.CreatedUTCDateTime },
                        {"@CreatedLocalDateTime", criticalOverridePeriod.CreatedLocalDateTime },
                        {"@CreatedActorKey", criticalOverridePeriod.CreatedActorKey },
                        {"@LastModifiedActorKey", (Guid?)context.Actor },
                        {"@LastModifiedBinaryValue", criticalOverridePeriod.LastModified ?? new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 } },
                        {"@CheckConcurrency", true },
                        {"@CriticalOverridePeriodKey", criticalOverridePeriod.Key },
                    });

                    connectionScope.Execute(
                       "Strg.usp_CriticalOverridePeriodUpdate",
                       parameters,
                       commandTimeout: connectionScope.DefaultCommandTimeout,
                       commandType: CommandType.StoredProcedure);
                }
            }

            // Add the new critical override periods.
            InsertDispensingDeviceCriticalOverridePeriods(                
                context,
                dispensingDeviceKey,
                addedCrtiticalOverridePeriods);
        }

        private void InsertDispensingDeviceAreas(Context context, Guid dispensingDeviceKey, IEnumerable<Guid> areaKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(areaKeys, "areaKeys");            
            
            foreach (Guid areaKey in areaKeys)
            {
                _areaRepository.AssociateAreaDispensingDevice(
                    context.ToActionContext(),
                    areaKey,
                    dispensingDeviceKey);
            }
        }

        private void UpdateDispensingDeviceAreas(Context context, Guid dispensingDeviceKey, IEnumerable<Guid> areaKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(areaKeys, "areaKeys");            

            // Get the list of area keys associated with this device.
            IEnumerable<Guid> currentAreaKeys = GetAssociatedAreasByDispensingDevice(dispensingDeviceKey);

            // Find the area keys that were removed.
            IEnumerable<Guid> removedAreaKeys = currentAreaKeys.Except(areaKeys);

            // Remove areas that are no longer associated with this device.
            foreach (Guid areaKey in removedAreaKeys)
            {
                _areaRepository.DisassociateAreaDispensingDevice(
                    context.ToActionContext(),
                    areaKey,
                    dispensingDeviceKey);
            }

            //Find the areas that were added
            IEnumerable<Guid> addedAreaKeys = areaKeys.Except(currentAreaKeys);
            InsertDispensingDeviceAreas(context, dispensingDeviceKey, addedAreaKeys);
        }

        private static IReadOnlyCollection<Guid> GetAssociatedAreasByDispensingDevice(Guid dispensingDeviceKey)
        {
            IReadOnlyCollection<Guid> areaKeys;

            SqlBuilder query = new SqlBuilder();
            query.SELECT("[add].AreaKey")
                    .FROM("Location.AreaDispensingDevice [add]")
                    .WHERE("[add].DisassociationUTCDateTime IS NULL")
                    .WHERE("[add].DispensingDeviceKey = @DispensingDeviceKey");

            using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
            {
                areaKeys = connectionScope.Query(
                     query.ToString(),
                     new { DispensingDeviceKey = dispensingDeviceKey },
                     commandTimeout: connectionScope.DefaultCommandTimeout,
                     commandType: CommandType.Text)
                     .Select(add => (Guid)add.AreaKey)
                     .ToList();
            }

            return areaKeys;
        }

        private void InsertDispensingDeviceOverrideGroups(Context context, Guid dispensingDeviceKey, IEnumerable<Guid> overrideGroupKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(overrideGroupKeys, "overrideGroupKeys");

            foreach (Guid overrideGroupKey in overrideGroupKeys)
            {
                _dispensingDeviceRepository.AssociateDispensingDeviceOverrideGroup(context.ToActionContext(), dispensingDeviceKey, overrideGroupKey);                
            }
        }

        private void UpdateDispensingDeviceOverrideGroups(Context context, Guid dispensingDeviceKey, IEnumerable<Guid> overrideGroupKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(overrideGroupKeys, "overrideGroupKeys");

            // Get the list of override group keys associated with this device.
            IEnumerable<Guid> currentOverrideGroupKeys;
            var query = new SqlBuilder();
            query.SELECT("ddogc.OverrideGroupKey")
                    .FROM("Strg.DispensingDeviceOverrideGroup ddogc")
                    .JOIN("Strg.DispensingDeviceSnapshot dds ON ddogc.DispensingDeviceKey = dds.DispensingDeviceKey AND dds.ENDUTCDateTime IS NULL AND dds.DeleteFlag = 0")
                    .JOIN("Strg.DispensingDevice dd ON dds.DispensingDeviceKey = dd.DispensingDeviceKey")
                    .WHERE("ddogc.DispensingDeviceKey = @DispensingDeviceKey");

            using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
            {
                currentOverrideGroupKeys = connectionScope.Query<Guid>(
                     query.ToString(),
                     new
                     {
                         DispensingDeviceKey = dispensingDeviceKey
                     },
                     commandTimeout: connectionScope.DefaultCommandTimeout,
                     commandType: CommandType.Text);
            }

            // Find the override group keys that were removed.
            IEnumerable<Guid> removedOverrideGroupKeys = currentOverrideGroupKeys.Except(overrideGroupKeys);

            // Remove override groups that are no longer associated with this device.
            foreach (Guid overrideGroupKey in removedOverrideGroupKeys)
            {
                _dispensingDeviceRepository.DisassociateDispensingDeviceOverrideGroup(context.ToActionContext(), dispensingDeviceKey, overrideGroupKey);
            }

            //Find the override groups that were added
            IEnumerable<Guid> addedOverrideGroupKeys = overrideGroupKeys.Except(currentOverrideGroupKeys);
            InsertDispensingDeviceOverrideGroups(context, dispensingDeviceKey, addedOverrideGroupKeys);
        }

        private void InsertDispensingDeviceClinicalDataSubjects(Context context, Guid dispensingDeviceKey, IEnumerable<Guid> clinicalDataSubjectKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(clinicalDataSubjectKeys, "clinicalDataSubjectKeys");
                       
            foreach (Guid clinicalDataSubjectKey in clinicalDataSubjectKeys)
            {
                _clinicalDataSubjectRepository.AssociateClinicalDataSubjectDispensingDevice(context.ToActionContext(), clinicalDataSubjectKey, dispensingDeviceKey);                
            }            
        }

        private void UpdateDispensingDeviceClinicalDataSubjects(Context context, Guid dispensingDeviceKey, IEnumerable<Guid> clinicalDataSubjectKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(clinicalDataSubjectKeys, "clinicalDataSubjectKeys");

            // Get the list of clinical data subject keys associated with this device.
            IEnumerable<Guid> currentClinicalDataSubjectKeys;
            var query = new SqlBuilder();
            query.SELECT("cdsdd.ClinicalDataSubjectKey")
                    .FROM("CDCat.ClinicalDataSubjectDispensingDevice cdsdd")
                    .JOIN("CDCat.ClinicalDataSubjectSnapshot cdss ON cdsdd.ClinicalDataSubjectKey = cdss.ClinicalDataSubjectKey AND cdss.EndUTCDateTime IS NULL AND cdss.DeleteFlag = 0")
                    .JOIN("Strg.DispensingDeviceSnapshot dds ON cdsdd.DispensingDeviceKey = dds.DispensingDeviceKey AND dds.EndUTCDateTime IS NULL AND dds.DeleteFlag = 0")
                    .JOIN("Strg.DispensingDevice dd ON dds.DispensingDeviceKey = dd.DispensingDeviceKey")
                    .WHERE("cdsdd.DispensingDeviceKey = @DispensingDeviceKey")
                    ._("cdsdd.DisassociationUTCDateTime IS NULL");

            using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
            {
                currentClinicalDataSubjectKeys = connectionScope.Query<Guid>(
                        query.ToString(),
                        new
                        {
                            DispensingDeviceKey = dispensingDeviceKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text);
            }

            // Find the clinical data subject keys that were removed.
            IEnumerable<Guid> removedClinicalDataSubjectKeys = currentClinicalDataSubjectKeys.Except(clinicalDataSubjectKeys);

            // Remove override groups that are no longer associated with this device.
            foreach (Guid removedClinicalDataSubjectKey in removedClinicalDataSubjectKeys)
            {
                _clinicalDataSubjectRepository.DisassociateClinicalDataSubjectDispensingDevice(context.ToActionContext(), removedClinicalDataSubjectKey, dispensingDeviceKey);
            }

            // Find the clinical data subject keys that were added
            IEnumerable<Guid> addedClinicalDataSubjectKeys = clinicalDataSubjectKeys.Except(currentClinicalDataSubjectKeys);
            InsertDispensingDeviceClinicalDataSubjects(context, dispensingDeviceKey, addedClinicalDataSubjectKeys);
        }

        private void InsertDispensingDeviceFacilityKits(Context context, Guid dispensingDeviceKey, bool stat, IEnumerable<Guid> facilityKitKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(facilityKitKeys, "facilityKitKeys");
            
            foreach (Guid facilityKitKey in facilityKitKeys)
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Item.usp_FacilityKitDispensingDeviceInsert",
                        new
                        {
                            @DispensingDeviceKey = dispensingDeviceKey,
                            @FacilityKitKey = facilityKitKey,
                            @StatFlag = stat,
                            @AssociationUTCDateTime = context.ActionUtcDateTime,
                            @AssociationLocalDateTime = context.ActionDateTime,
                            @AssociationActorKey = (Guid?)context.Actor
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }            
        }

        private void UpdateDispensingDeviceFacilityKits(Context context, Guid dispensingDeviceKey, bool stat, IEnumerable<Guid> facilityKitKeys)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(facilityKitKeys, "facilityKitKeys");

            // Get the list of facility kit keys associated with this device.
            IEnumerable<Guid> currentFacilityKitKeys;
            var query = new SqlBuilder();
            query.SELECT("fkdd.FacilityKitKey")
                    .FROM("Item.FacilityKitDispensingDevice fkdd")
                    .JOIN("Item.FacilityKitSnapshot fks ON fkdd.FacilityKitKey = fks.FacilityKitKey AND fks.EndUTCDateTime IS NULL AND fks.DeleteFlag = 0")
                    .WHERE("fkdd.DispensingDeviceKey = @DispensingDeviceKey")
                    ._("fkdd.StatFlag = @Stat")
                    ._("fkdd.DisassociationUTCDateTime IS NULL");

            using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
            {
                currentFacilityKitKeys = connectionScope.Query<Guid>(
                        query.ToString(),
                        new
                        {
                            DispensingDeviceKey = dispensingDeviceKey,
                            Stat = stat,
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text);
            }

            // Find the facility kit keys that were removed.
            IEnumerable<Guid> removedFacilityKitKeys = currentFacilityKitKeys.Except(facilityKitKeys);

            // Remove facility kits that are no longer associated with this device.
            foreach (Guid removedFacilityKitKey in removedFacilityKitKeys)
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(new Dictionary<string, object>
                    {
                        { "@DispensingDeviceKey", dispensingDeviceKey },
                        { "@FacilityKitKey", removedFacilityKitKey },
                        { "@StatFlag", stat },
                        { "@DisassociationUTCDateTime", context.ActionUtcDateTime },
                        { "@DisassociationLocalDateTime", context.ActionDateTime },
                        { "@DisassociationActorKey", (Guid?)context.Actor }
                    });

                    connectionScope.Execute(
                        "Item.usp_FacilityKitDispensingDeviceDelete",
                        parameters,
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }

            // Find the facility kit keys that were added
            IEnumerable<Guid> addedFacilityKitKeys = facilityKitKeys.Except(currentFacilityKitKeys);
            InsertDispensingDeviceFacilityKits(context, dispensingDeviceKey, stat, addedFacilityKitKeys);
        }

        #endregion
    }
}


