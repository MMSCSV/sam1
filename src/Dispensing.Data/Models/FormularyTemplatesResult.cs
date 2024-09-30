using System;

namespace CareFusion.Dispensing.Data.Models
{
    public class FormularyTemplatesResult
    {
        public Guid FormularyTemplateKey { get; set; }

        public Guid ExternalSystemKey { get; set; }

        public string ExternalSystemName { get; set; }

        public string FormularyTemplateName { get; set; }

        public string DescriptionText { get; set; }

        public bool OutdateTrackingFlag { get; set; }

        public string VerifyCountModeInternalCode { get; set; }

        public bool VerifyCountAnesthesiaDispensingFlag { get; set; }

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

        public bool ScanOnCheckFlag { get; set; }

        public bool DoOnceOnRemoveFlag { get; set; }

        public bool RequireLotIDOnRemoveFlag { get; set; }

        public bool RequireLotIDOnReturnFlag { get; set; }

        public bool RequireSerialIDOnRemoveFlag { get; set; }

        public bool RequireSerialIDOnReturnFlag { get; set; }

        public bool RequireExpirationDateOnRemoveFlag { get; set; }

        public bool RequireExpirationDateOnReturnFlag { get; set; }

        public bool RequireInventoryReferenceIDFlag { get; set; }

        public bool ReverificationFlag { get; set; }

        public Nullable<short> TooCloseRemoveDurationAmount { get; set; }

        public Nullable<Guid> SecurityGroupKey { get; set; }

        public bool ActiveFlag { get; set; }

        public bool AutoResolveDiscrepancyFlag { get; set; }

        public bool ChargeableFlag { get; set; }

        public bool HighCostFlag { get; set; }

        public bool AllowSplittingFlag { get; set; }

        public bool HighRiskFlag { get; set; }

        public bool TrackInventoryQuantityFlag { get; set; }

        public bool MultiDoseFlag { get; set; }

        public bool BackorderedFlag { get; set; }

        public string MedReturnModeInternalCode { get; set; }

        public string MedFailoverReturnModeInternalCode { get; set; }

        public string ReplenishmentScanModeInternalCode { get; set; }

        public string FractionalUOMTypeInternalCode { get; set; }

        public string AutoMedLabelModeInternalCode { get; set; }

        public string PharmacyNotesText { get; set; }

        public string NursingNotesText { get; set; }

        public Nullable<byte> CriticalLowPercentage { get; set; }

        public bool StockOutNoticeFlag { get; set; }

        public bool OMNLNoticeFlag { get; set; }

        public bool AnesthesiaMyItemsFlag { get; set; }

        public bool ResolveUndocumentedWasteFlag { get; set; }

        public Nullable<Guid> DistributorKey { get; set; }

        public bool RxCheckFlag { get; set; }

        public bool PrintOnRemoveFlag { get; set; }

        public bool PrintOnReturnFlag { get; set; }

        public bool PrintOnDisposeFlag { get; set; }

        public bool PrintOnLoadRefillFlag { get; set; }

        public bool ShowConversionOnRemoveFlag { get; set; }

        public bool ScanAllOnPickFlag { get; set; }

        public string CountCUBIEEjectModeInternalCode { get; set; }

        public bool PharmacyOrderDispenseQuantityFlag { get; set; }

        public bool InjectableFlag { get; set; }

        public string GCSMMedReturnModeInternalCode { get; set; }

        public string GCSMVerifyCountModeInternalCode { get; set; }

        public string GCSMCountCUBIEEjectModeInternalCode { get; set; }

        public bool GCSMOutdateTrackingFlag { get; set; }

        public bool GCSMRequireLotNumberWhenRecallFlag { get; set; }

        public bool GCSMRequireInventoryReferenceNumberFlag { get; set; }

        public bool GCSMWitnessOnOutdateFlag { get; set; }

        public bool GCSMWitnessOnAccessToDestructionBinFlag { get; set; }

        public bool GCSMWitnessOnAddToDestructionBinFlag { get; set; }

        public bool GCSMWitnessOnReturnFlag { get; set; }

        public bool GCSMWitnessOnAutorestockFlag { get; set; }

        public bool GCSMWitnessOnCompoundingFlag { get; set; }

        public bool GCSMWitnessOnDestructionBinFlag { get; set; }

        public bool GCSMWitnessOnDiscrepancyResolutionFlag { get; set; }

        public bool GCSMWitnessOnInventoryCountFlag { get; set; }

        public bool GCSMWitnessOnIssueFlag { get; set; }

        public bool GCSMWitnessOnPrescriptionFlag { get; set; }

        public bool GCSMWitnessOnRecallFlag { get; set; }

        public bool GCSMWitnessOnReceiveFlag { get; set; }
        public bool GCSMWitnessOnReverseCompoundingFlag { get; set; }

        public bool GCSMWitnessOnSellFlag { get; set; }

        public bool GCSMWitnessOnStockTransferFlag { get; set; }

        public bool GCSMWitnessOnWasteFlag { get; set; }

        public bool GCSMScanOnReturnFlag { get; set; }

        public bool GCSMScanOnAutorestockFlag { get; set; }

        public bool GCSMScanOnIssueFlag { get; set; }

        public bool GCSMScanOnPrescriptionFlag { get; set; }

        public bool GCSMScanOnReceiveFlag { get; set; }

        public bool GCSMScanOnSellFlag { get; set; }

        public bool GCSMScanOnStockTransferFlag { get; set; }

        public Nullable<byte> GCSMCriticalLowPercentage { get; set; }

        public bool GCSMPrintOnReceiveFlag { get; set; }

        public bool GCSMPrintDripSheetFlag { get; set; }

        public bool GCSMPrintSingleMedSheetFlag { get; set; }

        public bool GCSMPrintMaximumRowsFlag { get; set; }

        public byte GCSMAdditionalLabelsPrintedQuantity { get; set; }

        public bool GCSMStockOutNoticeFlag { get; set; }

        public bool GCSMWitnessOnUnloadFlag { get; set; }

        public bool GCSMScanOnCompoundingFlag { get; set; }

        public int? GCSMADMDispenseQuantity{ get; set; }
        public int? GCSMDistributorPackageSizeQuantity { get; set; }
        public short? GCSMTotalDeviceParDurationAmount { get; set; }
        public short? GCSMTotalParDurationAmount { get; set; }

        public Nullable<Guid> GCSMDistributorKey { get; set; }

        public bool GCSMRequireOriginDestinationFlag { get; set; }

        public Nullable<Guid> LastModifiedActorKey { get; set; }

        public string LastModifiedUserAccountDisplayName { get; set; }

        public DateTime LastModifiedUTCDateTime { get; set; }

        public byte[] LastModifiedBinaryValue { get; set; }
    }
}
