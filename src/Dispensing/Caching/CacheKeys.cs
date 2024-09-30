using System;

namespace CareFusion.Dispensing.Caching
{
    public static class CacheKeys
    {
        #region Common Cache Keys

        // Caching that occurs under shared projects that may be used within multiple executables.

        public const string CommonActiveDirectoryDomains = "Common.ActiveDirectoryDomains";
        public const string CommonDispensingSystem = "Common.DispensingSystem";
        public const string CommonActiveDirectoryDomainPolicy = "Common.ActiveDirectoryDomainPolicy";

        #endregion

        #region Server Cache Keys

        public const string ServerContactMethods = "Server.ContactMethods";
        public const string ServerAuthenticationMethods = "Server.AuthenticationMethods";
        public const string ServerDestructionBinCodes = "Server.DestructionBinCodes";
        public const string ServerEncryptionAlgorithms = "Server.EncryptionAlgorithms";
        public const string ServerPermissions = "Server.Permissions";
        public const string ServerPermissionTypes = "Server.PermissionTypes";
        public const string ServerUnitOfMeasureRoles = "Server.UnitOfMeasureRoles";
        public const string ServerClinicalSubjectTypes = "Server.ClinicalSubjectTypes";
        public const string ServerVerifyCountModes = "Server.VerifyCountModes";
        public const string ServerMedReturnModes = "Server.MedReturnModes";
        public const string ServerEncounterAdmissionStatuses = "Server.EncounterAdmissionStatuses";
        public const string ServerEncounterTypes = "Server.EncounterTypes";
        public const string ServerStandardRepeatPatterns = "Server.StandardRepeatPatterns";
        public const string ServerStorageSpaceSizes = "Server.StorageSpaceSizes";
        public const string ServerStorageSpaceTypes = "Server.StorageSpaceTypes";
        public const string ServerMiniDrawerTrayModes = "Server.MiniDrawerTrayModes";
        public const string ServerStandardTimingRecordPriorities = "Server.StandardTimingRecordPriorities";
        public const string ServerDispensingDeviceTypes = "Server.DispensingDeviceTypes";
        public const string ServerNoticeTypes = "Server.NoticeTypes";
        public const string ServerReplenishmentScanModes = "Server.ReplenishmentScanModes";
        public const string ServerActiveDirectoryPollStatuses = "Server.ActiveDirectoryPollStatuses";
        public const string ServerSendInventoryTypes = "Server.SendInventoryTypes";
        public const string ServerItemTransactionTypes = "Server.ItemTransactionTypes";
        public const string ServerUserScanModes = "Server.UserScanModes";
        public const string ServerWasteModes = "Server.WasteModes";
        public const string ServerMedFailoverReturnModes = "Server.MedFailoverReturnModes";
        public const string ServerFractionalUnitOfMeasureTypes = "Server.FractionalUnitOfMeasureTypes";
        public const string ServerPartialRemoveModes = "Server.PartialRemoveModes";

        public const string ServerActiveDirectoryDomains = "Server.ActiveDirectoryDomains";
        public const string ServerAdministrationRoutes = "Server.AdministrationRoutes";
        public const string ServerAreas = "Server.Areas";
        public const string ServerDeliveryLocations = "Server.DeliveryLocations";
        public const string ServerDeliveryStateChangeReasons = "Server.DeliveryStateChangeReasons";
        public const string ServerDispensingSystem = "Server.DispensingSystem";
        public const string ServerDistributors = "Server.Distributors";
        public const string ServerExternalSystems = "Server.ExternalSystems";
        public const string ServerFacilities = "Server.Facilities";
        public const string ServerFacilityPatientSiloStatuses = "Server.FacilityPatientSiloStatuses";
        public const string ServerFormularyTemplates = "Server.FormularyTemplates";
        public const string ServerMedicationClasses = "Server.MedicationClasses";
        public const string ServerMedClassGroups = "Server.MedClassGroups";
        public const string ServerOverrideGroups = "Server.OverrideGroups";
        public const string ServerPatientSilos = "Server.PatientSilos";
        public const string ServerPickAreas = "Server.PickAreas";
        public const string ServerSecurityGroups = "Server.SecurityGroups";
        public const string ServerServers = "Server.Servers";
        public const string ServerUserTypes = "Server.UserTypes";
        public const string ServerUsersWithFacilityPermissions = "Server.UsersWithFacilityPermissions";
        public const string ServerVirtualLocations = "Server.VirtualLocations";
        public const string ServerZones = "Server.Zones";
        public const string ServerInvoiceTypes = "Server.InvoiceTypes";
        public const string ServerControlledSubstanceLicenses = "Server.ControlledSubstanceLicenses";
        public const string ServerItemDeliveryTrackingStatuses = "Server.ItemDeliveryTrackingStatuses";
        public const string ServerItemDeliveryStateChangeReasonTypes = "Server.ItemDeliveryStateChangeReasonTypes";
        public const string ServerRxCheckReasons = "Server.RxCheckReasons";
        public const string ServerAutoMedLabelModes = "Server.AutoMedLabelModes";
        public const string ServerTransactionSessionTypes = "Server.TransactionSessionTypes";
        public const string ServerWorkflowSteps = "Server.WorkflowSteps";
        public const string ServerAutoDischargeModes = "Server.ServerAutoDischargeModes";
        public const string ServerNoRecentMessageReceivedTypes = "Server.ServerNoRecentMessageReceivedTypes";
        public const string ServerCountCubieEjectModes = "Server.CountCUBIEEjectModes";
        public const string ServerSequentialDrainModes = "Server.SequentialDrainModes";
        public const string ServerStandardUserTypes = "Server.StandardUserTypes";
        public const string ServerSingleMedSheetStyle = "Server.SingleMedSheetStyle";
        public const string ServerMultiMedSheetStyle = "Server.MultiMedSheetStyle";
        public const string ServerDripMedSheetStyle = "Server.DripMedSheetStyle";
        public const string ServerAnesthesiaSheetStyle = "Server.AnesthesiaSheetStyle";
        public const string ServerDeliverySignatureSheetStyle = "Server.DeliverySignatureSheetStyle";
        public const string ServerReceiveReceiptSheetStyle = "Server.ReceiveReceiptSheetStyle";
        public const string ServerEmptyDestructionBinReceiptSheetStyle = "Server.EmptyDestructionBinReceiptSheetStyle";
        public const string GCSMCompareReportStandardRanges = "Server.GCSMCompareReportStandardRanges";

        public static Func<Guid, string> ServerUserFacilityPermissions =
            userAccountKey => string.Format("Server.UserFacilityPermissions_{0}", userAccountKey);

        #endregion

        #region MessagingWindowsService Cache Keys

        public const string MessagingWindowsServiceAdtPatientSilos = "MessagingWindowsService.AdtPatientSilos";
        public const string MessagingWindowsServiceAllergyProviderPatientSilos = "MessagingWindowsService.AllergyProviderPatientSilos";
        public const string MessagingWindowsServiceExternalPatientIdTypeCodes = "MessagingWindowsService.ExternalPatientIDTypeCodes";
        public const string MessagingWindowsServiceDispensingDeviceItems = "MessagingWindowsService.SendInventoryEventWorker.DispensingDeviceItems";
        public const string MessagingWindowsServiceVirtualStockLocationItems = "MessagingWindowsService.SendInventoryEventWorker.VirtualStockLocationItems";

        #endregion

        #region WebAPI Keys

        public const string WebApiAuthenticationTokens = "WebAPI.AuthenticationTokens";

        #endregion
    }
}
