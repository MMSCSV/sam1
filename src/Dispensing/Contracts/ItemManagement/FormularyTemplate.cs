using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents properties that by default apply to medication items when
    /// being approved for a facility for a given PIS and given medication class.
    /// </summary>
    [Serializable]
    public class FormularyTemplate : Entity<Guid>
    {
        #region Constructors

        public FormularyTemplate()
        {
        }

        public FormularyTemplate(Guid key)
            : this()
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator FormularyTemplate(Guid key)
        {
            return FromKey(key);
        }

        public static FormularyTemplate FromKey(Guid key)
        {
            return new FormularyTemplate(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of an external system.
        /// </summary>
        public Guid ExternalSystemKey { get; set; }

        /// <summary>
        /// Gets the name of an external system.
        /// </summary>
        public string ExternalSystemName { get; internal set; }

        /// <summary>
        /// Gets or sets the name that is normally used for a formulary template.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the text that describes a formulary template.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether we track (on both dispense and load/refill)
        /// the earliest expiration date for the inventory in a storage space.
        /// </summary>
        public bool OutdateTracking { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a verify count mode.
        /// </summary>
        public VerifyCountModeInternalCode? VerifyCountMode { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether verify count mode applies to
        /// anesthesia dispensing.
        /// </summary>
        public bool VerifyCountAnesthesiaDispensing { get; set; }

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
        /// Gets or sets the value that indicates whether emptying a return bin is witnessed if
        /// the bin contains the item.
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
        /// Gets or sets the value that indicates whether an item is scanned on load or refill.
        /// </summary>
        public bool ScanOnLoadRefill { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is scanned on remove.
        /// </summary>
        public bool ScanOnRemove { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is scanned once on remove.
        /// </summary>
        public bool DoOnceOnRemove { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is scanned on return.
        /// </summary>
        public bool ScanOnReturn { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is scanned on check.
        /// </summary>
        public bool ScanOnCheck { get; set; }

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
        /// Gets or sets the value that indicates whether Inventory Reference ID is required.
        /// </summary>
        public bool RequireInventoryReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether, at an anesthesia dispensing device, a user
        /// must be re-authenticated when dispensing.
        /// </summary>
        public bool Reverification { get; set; }

        /// <summary>
        /// Gets or sets the duration in minutes between removing a medication item and the prior removal of the
        /// same administrable item that if neither met nor exceeded results in a warning.
        /// </summary>
        public short? TooCloseRemoveDuration { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a security group.
        /// NOTE: Variable dose group does not require security group key to be set
        /// </summary>
        public Guid? SecurityGroupKey { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether discrepancies are automatically resolved.
        /// </summary>
        public bool AutoResolveDiscrepancy { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether patients are charged for an item.
        /// </summary>
        public bool IsChargeable { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is a high cost item.
        /// </summary>
        public bool IsHighCost { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether splitting is allowed.
        /// </summary>
        public bool AllowSplitting { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is high risk.
        /// </summary>
        public bool IsHighRisk { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is to be tracked within inventory.
        /// </summary>
        public bool TrackInventoryQuantity { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item may be used in a multi-dose manner.
        /// </summary>
        public bool IsMultiDose { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item is on backlog within a facility.
        /// </summary>
        public bool IsBackOrdered { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a medication return mode.
        /// </summary>
        public MedReturnModeInternalCode? MedReturnMode { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a medication failover return mode.
        /// </summary>
        public MedFailoverReturnModeInternalCode? MedFailoverReturnMode { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a replenishment scan mode.
        /// </summary>
        public ReplenishmentScanModeInternalCode? ReplenishmentScanMode { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a fractional unit of measure type.
        /// </summary>
        public FractionalUOMTypeInternalCode? FractionalUnitOfMeasureType { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies an auto medication label mode.
        /// </summary>
        public AutoMedLabelModeInternalCode? AutoMedLabelMode { get; set; }

        /// <summary>
        /// Gets or sets the notes for pharmacy.
        /// </summary>
        public string PharmacyNotes { get; set; }

        /// <summary>
        /// Gets or sets the notes for nursing.
        /// </summary>
        public string NursingNotes { get; set; }

        /// <summary>
        /// Gets or sets the % of aggregated minimum quantity of an item at a dispensing device that will
        /// cause a Stock Critical Low bulletin to print.
        /// </summary>
        public byte? CriticalLowPercentage { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether stock out bulletins are created for a facility item.
        /// </summary>
        public bool StockOutNotice { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether OMNL bulletins are created for a facility item.
        /// </summary>
        public bool OmnlNotice { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether users can dispense from the my-items feature.
        /// </summary>
        public bool AnesthesiaMyItems { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the Dispensing System tracks undocumented wastes.
        /// </summary>
        public bool ResolveUndocumentedWaste { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a distributor.
        /// </summary>
        public Guid? DistributorKey { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a user needs to check that a load or refill by another
        /// user follows pharmacy policy.
        /// </summary>
        public bool RxCheck { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a remove slip should be printed on remove.
        /// </summary>
        public bool PrintOnRemove { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a return slip/receipt should be printed on return.
        /// </summary>
        public bool PrintOnReturn { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a waste slip/receipt should be printed on waste.
        /// </summary>
        public bool PrintOnDispose { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a load/refill slip/receipt should be printed on load/refill.
        /// </summary>
        public bool PrintOnLoadRefill { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether conversion calculations should be shown on remove.
        /// </summary>
        public bool ShowConversionOnRemove { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether all actual items are scanned on pick, rather than just one
        /// </summary>
        public bool ScanAllOnPick { get; set; }

        /// <summary>
        /// Gets or sets the nonlocalizable code that identifies a count CUBIE eject mode
        /// </summary>
        public CountCUBIEEjectModeInternalCode? CountCubieEjectMode { get; set; }

        public bool PharmacyOrderDispenseQuantityFlag { get; set; }

        public bool InjectableFlag { get; set; }

        /// <summary>
        /// Gets or sets the nonlocalizable code that identifies a Return Mode for a Med into GCSM
        /// </summary>
        public MedReturnModeInternalCode? GCSMMedReturnModeInternalCode { get; set; }

        public VerifyCountModeInternalCode? GCSMVerifyCountModeInternalCode { get; set; }

        public CountCUBIEEjectModeInternalCode? GCSMCountCUBIEEjectModeInternalCode { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Outdate Tracking is on
        /// </summary>
        public bool GCSMOutdateTrackingFlag { get; set; }

        public bool GCSMRequireLotNumberWhenRecallFlag { get; set; }

        public bool GCSMRequireInventoryReferenceNumberFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Witness on Outdate  is on
        /// </summary>
        public bool GCSMWitnessOnOutdateFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Witness on Return is on
        /// </summary>
        public bool GCSMWitnessOnReturnFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Witness on AutoRestock is on
        /// </summary>
        public bool GCSMWitnessOnAutorestockFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Witness on Compounding is on
        /// </summary>
        public bool GCSMWitnessOnCompoundingFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Witness on Accessing Destruction Bin is on
        /// </summary>
        public bool GCSMWitnessOnDestructionBinFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Witness on Access to Destruction Bin is on
        /// </summary>
        public bool GCSMWitnessOnAccessToDestructionBinFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Witness on Add to Destruction Bin is on
        /// </summary>
        public bool GCSMWitnessOnAddToDestructionBinFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Witness on Discrepancy Resolution is on
        /// </summary>
        public bool GCSMWitnessOnDiscrepancyResolutionFlag { get; set; }

        public bool GCSMWitnessOnInventoryCountFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Witness on Issue is on
        /// </summary>
        public bool GCSMWitnessOnIssueFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Witness on Fullfilling a Presciption is on
        /// </summary>
        public bool GCSMWitnessOnPrescriptionFlag { get; set; }

        public bool GCSMWitnessOnRecallFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Witness on Receive is on
        /// </summary>
        public bool GCSMWitnessOnReceiveFlag { get; set; }

        public bool GCSMWitnessOnReverseCompoundingFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Witness on Sell is on
        /// </summary>
        public bool GCSMWitnessOnSellFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Witness on Stock Transfer is on
        /// </summary>
        public bool GCSMWitnessOnStockTransferFlag { get; set; }

        public bool GCSMWitnessOnUnloadFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Witness on Waste is on
        /// </summary>
        public bool GCSMWitnessOnWasteFlag { get; set; }

        public bool GCSMScanOnCompoundingFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Scan on Return is on
        /// </summary>
        public bool GCSMScanOnReturnFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Scan on AutoRestock is on
        /// </summary>
        public bool GCSMScanOnAutorestockFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Scan on Issue is on
        /// </summary>
        public bool GCSMScanOnIssueFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Scan on Prescription is on
        /// </summary>
        public bool GCSMScanOnPrescriptionFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Scan on Receive is on
        /// </summary>
        public bool GCSMScanOnReceiveFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Scan on Sell is on
        /// </summary>
        public bool GCSMScanOnSellFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Scan on Stock Transfer is on
        /// </summary>
        public bool GCSMScanOnStockTransferFlag { get; set; }

        /// <summary>
        /// Gets or sets the Critical Low Percentage 
        /// Gets or sets the flag that indicates if Critical Low Percentage is on
        /// </summary>
        public byte? GCSMCriticalLowPercentage { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if GCSM PrintOnReceiveFlag is on
        /// </summary>
        public bool GCSMPrintOnReceiveFlag { get; set; }
        /// <summary>
        /// Gets or sets the flag that indicates if GCSM PrintSingleMedSheetFlag is on
        /// </summary>
        public bool GCSMPrintSingleMedSheetFlag { get; set; }
        /// <summary>
        /// Gets or sets the flag that indicates if GCSM PrintDripSheetFlag is on
        /// </summary>
        public bool GCSMPrintDripSheetFlag { get; set; }
        /// <summary>
        /// Gets or sets the flag that indicates if GCSM PrintMaximumRowsFlag is on
        /// </summary>
        public bool GCSMPrintMaximumRowsFlag { get; set; }
        /// <summary>
        /// Gets or sets the flag that indicates if GCSM NoSheetOverrideFlag is on
        /// </summary>
        public byte GCSMAdditionalLabelsPrintedQuantity { get; set; }

        public bool GCSMStockOutNoticeFlag { get; set; }

        public int? GCSMADMDispenseQuantity { get; set; }
        public int? GCSMDistributorPackageSizeQuantity { get; set; }
        public short? GCSMTotalDeviceParDurationAmount { get; set; }

        public short? GCSMTotalParDurationAmount { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a distributor.
        /// </summary>
        public Guid? GCSMDistributorKey { get; set; }

        public bool GCSMRequireOriginDestinationFlag { get; set; }

        /// <summary>
        /// Gets the value that indicates whether a formulary template is logically deleted.
        /// </summary>
        public bool IsDeleted { get; internal set; }

        /// <summary>
        /// Gets the surrogate key of the last modified actor.
        /// </summary>
        public Guid? LastModifiedActorKey { get; internal set; }

        /// <summary>
        /// Gets the display name of the last modified actor (user). 
        /// </summary>
        public string LastModifiedUserAccountDisplayName { get; internal set; }

        /// <summary>
        /// Gets the UTC date and time of when last updated.
        /// </summary>
        public DateTime LastModifiedUtcDateTime { get; internal set;  }

        /// <summary>
        /// Gets or sets the override groups associated with a formulary template.
        /// </summary>
        public Guid[] OverrideGroups { get; set; }

        /// <summary>
        /// Gets or sets the med class thata is associated with this formulary template
        /// </summary>
        public Guid? MedicationClassKey { get; set; }

        #endregion
    }
}
