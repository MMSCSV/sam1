using System;
using System.Globalization;
using CareFusion.Dispensing.Models;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;
using Microsoft.Practices.EnterpriseLibrary.Validation;
using Microsoft.Practices.EnterpriseLibrary.Validation.Validators;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a record of an item that is used within a facility 
    /// for a given period of time.
    /// </summary>
    [Serializable]
    [HasSelfValidation]
    public class FacilityItem : Entity<Guid>
    {
        private Guid _snapshotKey;
        private string _medDisplayName;

        #region Constructors

        public FacilityItem()
        {
            // NOTE: 20101214 Martin Orona - TFS Spec #29601 calls for a default value of checked (checkbox value that is the equivalent of boolean true value)
            StockOutNotice = true;
            // NOTE: 20101216 Martin Orona - TFS Spec #30026 calls for a default value of checked (checkbox value that is the equivalent of boolean true value)
            OmnlNotice = true;
            // Note: 20200116 TFS Spec #852816 calls for a default value of checked (checkbox value that is the equivalent of boolean true value)
            GCSMStockOutNoticeFlag = true;

            MedReturnMode = MedReturnModeInternalCode.RET;
        }

        public FacilityItem(Guid key)
            : this()
        {
            Key = key;
        }

        public FacilityItem(Guid key, Guid snapshotKey)
            : this()
        {
            Key = key;
            _snapshotKey = snapshotKey;
        }

        public FacilityItem(Guid key, Guid snapshotKey, string medDisplayName)
            : this()
        {
            Key = key;
            _snapshotKey = snapshotKey;
            _medDisplayName = medDisplayName;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator FacilityItem(Guid key)
        {
            return FromKey(key);
        }

        public static FacilityItem FromKey(Guid key)
        {
            return new FacilityItem(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the item's snapshot key.
        /// </summary>
        public Guid SnapshotKey
        {
            get { return _snapshotKey; }
        }

        /// <summary>
        /// Gets or sets the item's snapshot key.
        /// </summary>
        public string MedDisplayName
        {
            get { return _medDisplayName; }
        }

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        public Guid FacilityKey { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of an item.
        /// </summary>
        [NotNullValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "FacilityItemRequired")]
        public Item Item { get; set; }

        /// <summary>
        /// Gets the <see cref="MedItem"/> if the <see cref="Item"/> associated with this 
        /// facility item is reall a <see cref="MedItem"/>.
        /// </summary>
        public MedItem MedItem
        {
            get { return Item as MedItem; }
        }

        /// <summary>
        /// Gets or sets the name that is normally used for an item (this being a replacement for the generic name).
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.ItemDisplayNameUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "ItemIdOutOfBounds")]
        public string DisplayName { get; set; }
        

        /// <summary>
        /// Gets or sets the alternate identification of an item.
        /// </summary>
        public string AlternateId { get; set; }

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
        /// Gets or sets the value that indicates whether inventory reference ID is required.
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
        [DispensingRangeValidator(ValidationConstants.FacilityItemTooCloseRemoveDurationLowerBound,
            ValidationConstants.FacilityItemTooCloseRemoveDurationUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "FacilityTooCloseRemoveDurationOutOfBounds")]
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
        /// Gets or sets the surrogate key of a hazardous waste class.
        /// </summary>
        public Guid? HazardousWasteClassKey { get; set; }

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
        /// Gets or sets the unit of measure that is used to refill a dispensing device.
        /// </summary>
        public UnitOfMeasure RefillUnitOfMeasure { get; set; }

        /// <summary>
        /// Gets or sets the unit of measure that is used 
        /// on issue (dispense) of an item.
        /// </summary>
        public UnitOfMeasure IssueUnitOfMeasure { get; set; }

        /// <summary>
        /// Gets or sets the number of units of issue for each unit of refill.
        /// </summary>
        public decimal? UnitsOfIssuePerUnitOfRefill { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an item should normally be kept within a
        /// dispensing device.
        /// </summary>
        public bool IsStandardStockWithinDispensingDevice { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies a medication return mode.
        /// </summary>
        public MedReturnModeInternalCode MedReturnMode { get; set; }
        /// <summary>
        /// Gets the internal code that identifies the Item Type Internal Code
        /// </summary>
        public ItemTypeInternalCode? ItemTypeInternalCode { get; set; }
        /// <summary>
        /// Gets the internal code that identifies the Item Sub Type Internal Code
        /// </summary>
        public ItemSubTypeInternalCode? ItemSubTypeInternalCode { get; set; }

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
        [DispensingRangeValidator(ValidationConstants.FacilityItemCriticalLowPercentageLowerBound,
            ValidationConstants.FacilityItemCriticalLowPercentageUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "FacilityItemCriticalLowPercentageOutOfBounds")]
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
        /// Gets or sets the value that indicates whether the Dispensing System resolves undocumented wastes.
        /// </summary>
        public bool ResolveUndocumentedWaste { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a facility item is a combo med.
        /// </summary>
        public bool IsCombo { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a combo is not dispensed when the combo's
        /// components are dispensed.
        /// </summary>
        public bool DispenseComponentsOnly { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a combo med ID is passed to billers when the 
        /// combo itself is dispensed or a combo med ID is passed to billers when a component is dispensed.
        /// </summary>
        public bool ChargeCombo { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether dispense calculation should be displayed to the user
        /// when removing an item.
        /// </summary>
        public bool DisplayCalculationOnDispense { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a replenish pick area.
        /// </summary>
        public Guid? ReplenishmentPickAreaKey { get; set; }

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
        /// Gets or sets the medication class.
        /// </summary>
        public MedicationClass MedicationClass { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether all actual items are scanned on pick, rather than just one
        /// </summary>
        public bool ScanAllOnPick { get; set; }

        /// <summary>
        /// Gets or sets the nonlocalizable code that identifies a count CUBIE eject mode
        /// </summary>
        public CountCUBIEEjectModeInternalCode? CountCubieEjectMode { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates PharmacyOrderDispenseQuantity 
        /// </summary>
        public bool PharmacyOrderDispenseQuantityFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates InjectableFlag 
        /// </summary>
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

        /// <summary>
        /// Gets or sets the flag that indicates if Witness on access to DestructioBin is on
        /// </summary>
        public bool GCSMWitnessOnAccessToDestructionBinFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if Witness on add to the DestructionBin is on
        /// </summary>
        public bool GCSMWitnessOnAddToDestructionBinFlag { get; set; }

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
        public short? GCSMAdditionalLabelsPrintedQuantity { get; set; }

        public bool GCSMStockOutNoticeFlag { get; set; }

        public int? GCSMADMDispenseQuantity { get; set; }
        public int? GCSMDistributorPackageSizeQuantity { get; set; }

        public short? GCSMTotalDeviceParDurationAmount { get; set; }

        public short? GCSMTotalParDurationAmount { get; set; }
        /// <summary>
        /// Gets or sets the surrogate key of a distributor.
        /// </summary>
        public Guid? GCSMDistributorKey { get; set; }

        public Guid? GCSMPreferredProductIDKey { get; set; }

        public string GCSMVendorItemCode { get; set; }

        public bool GCSMRequireOriginDestinationFlag { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates if CheckConcurrency is on
        /// </summary>
        public bool CheckConcurrency { get; set; }

        /// <summary>
        /// Gets or sets the combo components associated with a facility item.
        /// </summary>
        public ComboComponent[] ComboComponents { get; set; }

        /// <summary>
        /// Gets or sets the override groups associated with a facility item.
        /// </summary>
        public OverrideGroup[] OverrideGroups { get; set; }

        /// <summary>
        /// Gets or sets the pick areas associated with a facility item.
        /// </summary>
        public PickArea[] PickAreas { get; set; }

        /// <summary>
        /// Gets or sets the physical capacities that are associated with a facility item.
        /// </summary>
        public PhysicalCapacity[] PhysicalCapacities { get; set; }

        /// <summary>
        /// Gets or sets the override clinical data subjects associated with a facility item.
        /// </summary>
        public Guid[] OverrideClinicalDataSubjects { get; set; }

        /// <summary>
        /// Gets or sets the remove clinical data subjects associated with a facility item.
        /// </summary>
        public Guid[] RemoveClinicalDataSubjects { get; set; }

        /// <summary>
        /// Gets or sets the return clinical data subjects associated with a facility item.
        /// </summary>
        public Guid[] ReturnClinicalDataSubjects { get; set; }

        /// <summary>
        /// Gets or sets the waste clinical data subjects associated with a facility item.
        /// </summary>
        public Guid[] WasteClinicalDataSubjects { get; set; }

        /// <summary>
        /// Gets or sets the blocked devices associated with a facility item.
        /// </summary>
        public Guid[] BlockedDispensingDevices { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool UseFacilityMedClass { get; set; }
        /// <summary>
        /// Gets or sets the facility equivalencies
        /// </summary>
        public FacilityEquivalency[] FacilityEquivalencies { get; set; }

        #endregion

        #region SelfValidation Logic

        [SelfValidation]
        public void CheckAdditionalRequiredFields(ValidationResults results)
        {
            if (UnitsOfIssuePerUnitOfRefill != null &&
                UnitsOfIssuePerUnitOfRefill < ValidationConstants.FacilityItemUnitsOfIssuePerUnitOfRefillLowerBound)
            {
                results.AddResult(new ValidationResult(string.Format(CultureInfo.CurrentCulture,
                    ValidationStrings.FacilityItemUnitsOfIssuePerUnitOfRefillOutOfBounds,
                    ValidationConstants.FacilityItemUnitsOfIssuePerUnitOfRefillLowerBound), 
                    this, "UnitsOfIssuePerUnitOfRefill", "", null));
            }

            // Cannot have a refill unit of measure without an issue unit of measure.
            if (IssueUnitOfMeasure == null && RefillUnitOfMeasure != null)
            {
                results.AddResult(new ValidationResult(ValidationStrings.FacilityItemRefillUnitOfMeasureRequiresIssueUnitOfMeasureRule, this,
                                                       "RefillUnitOfMeasure", "", null));
            }

            if (IssueUnitOfMeasure != null && RefillUnitOfMeasure != null)
            {
                if (IssueUnitOfMeasure.Key == RefillUnitOfMeasure.Key)
                {
                    if (UnitsOfIssuePerUnitOfRefill != null && UnitsOfIssuePerUnitOfRefill.Value > 1)
                    {
                        results.AddResult(new ValidationResult(ValidationStrings.FacilityItemUnitsOfIssuePerUnitOfRefillRule, this,
                                                       "UnitsOfIssuePerUnitOfRefill", "", null));
                    }
                }

                if (UnitsOfIssuePerUnitOfRefill == null)
                {
                    results.AddResult(new ValidationResult(ValidationStrings.FacilityItemUnitsOfIssuePerUnitOfRefillRequiredRule, this,
                                                       "UnitsOfIssuePerUnitOfRefill", "", null));
                }
            }

            if ((IssueUnitOfMeasure == null || RefillUnitOfMeasure == null) && 
                UnitsOfIssuePerUnitOfRefill != null)
            {
                results.AddResult(new ValidationResult(ValidationStrings.FacilityItemUnitsOfIssuePerUnitOfRefillRequiresIssueAndRefillRule, this,
                                                       "UnitsOfIssuePerUnitOfRefill", "", null));
            }
        }

        #endregion
    }
}
