using System;
using System.ComponentModel.DataAnnotations.Schema;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents the most general place that a patient is located
    /// at.
    /// </summary>
    [Serializable]
    public class Facility : IEntity<Guid>
    {
        #region Constructors

        public Facility()
        {
        }

        public Facility(Guid key)
            : this()
        {
            Key = key;
        }

        public static Facility Default
        {
            get
            {
                return new Facility
                {
                    BioIdFailoverInternalCode = AuthenticationMethodInternalCodes.UIDPWDW,
                    RfidFailoverInternalCode = AuthenticationMethodInternalCodes.UIDFP,
                    GCSMDestructionBinEmptyWithDiscrepancyModeInternalCode = DestructionBinEmptyWithDiscrepancyModeInternalCodes.CHKANYDSCR,
                    UserScanModeInternalCode = UserScanModeInternalCodes.NOTREQD,
                    SelectAllForRemoveList = ValidationConstants.FacilitySelectAllForRemoveListDefaultValue,
                    TooCloseRemoveDuration = ValidationConstants.FacilityTooCloseRemoveDurationDefaultValue,
                    LateRemoveDuration = ValidationConstants.FacilityLateRemoveDurationDefaultValue,
                    TemporaryPatientDuration = ValidationConstants.FacilityTemporaryPatientDurationDefaultValue,
                    ResendAfterTemporaryRemainsUnmergedDuration = ValidationConstants.FacilityResendAfterTemporaryRemainsUnmergedDurationDefaultValue,
                    CriticalOverrideScheduling = ValidationConstants.FacilityCriticalOverrideSchedulingDefaultValue,
                    MedicationExpirationCheckDuration = ValidationConstants.FacilityMedicationExpirationCheckDurationDefaultValue,
                    MedicationTemporaryUserAccessDuration = ValidationConstants.FacilityMedicationTemporaryUserAccessDurationDefaultValue,
                    NoticeDeviceNotCommunicatingDelayDuration = ValidationConstants.FacilityNoticeDeviceNotCommunicatingDelayDefaultValue,
                    MedicationQueueDuration = ValidationConstants.FacilityMedicationQueueDurationDefaultValue,
                    MedicationQueueBeforeDueDuration = ValidationConstants.FacilityMedicationQueueBeforeDueDurationDefaultValue,
                    MedicationQueueBeforeDueNowDuration = ValidationConstants.FacilityMedicationQueueBeforeDueNowDurationDefaultValue,
                    MedicationQueueAfterDueNowDuration = ValidationConstants.FacilityMedicationQueueAfterDueNowDurationDefaultValue,
                    PreadmissionLeadDuration = ValidationConstants.FacilityPreadmissionLeadDurationDefaultValue,
                    PreadmissionProlongedInactivityDuration = ValidationConstants.FacilityPreadmissionProlongedInactivityDurationDefaultValue,
                    AdmissionProlongedInactivityDuration = ValidationConstants.FacilityAdmissionProlongedInactivityDurationDefaultValue,
                    DischargeDelayDuration = ValidationConstants.FacilityDischargeDelayDurationDefaultValue,
                    TransferDelayDuration = ValidationConstants.FacilityTransferDelayDurationDefaultValue,
                    LeaveOfAbsenceDelayDuration = ValidationConstants.FacilityLeaveOfAbsenceDelayDurationAmountDefaultValue,
                    IsActive = ValidationConstants.FacilityIsActiveDefaultValue,
                    AllowFreeFormReason = ValidationConstants.FacilityAllowFreeFormReasonDefaultValue,
                    DisplayEncounterId = true,
                    DisplayAlternateEncounterId = false,
                    RemoveMedicationLabelProductId = true,
                    DisplayAccountId = false,
                    DeliveryStatusDisplayDuration = ValidationConstants.FacilityDeliveryStatusDisplayDurationDefaultValue,
                    MyItemsNotificationDuration = ValidationConstants.FacilityMyItemsNotificationDurationDefaultValue,
                    RxCheckExpirationDuration = ValidationConstants.FacilityRxCheckExpirationDurationLowerBound,
                    DisablePendAssignOutdateTracking = false, //TFS 788323
                    NoticeEtlDelayDuration = ValidationConstants.FacilityReportETLFailureDelayPeriodDefaultValue,
                    RequestPharmacyOrderDoseDuration = 120, // spec 396723
                    AttentionNoticeCriticalThresholdDuration = 0,
                    StatusBoardDoseRequestDisplayDuration = 480,
                    StatusBoardNewDoseRequestDisplayDuration = 30,
                    UnknownAdmissionStatusRetentionDuration = 365,
                    MixedDeviceTypeQueuingModeProfile = true, // spec 394500
                    CriteriaBasedFill = true, // spec 478567
                    RemovePatientIdBarcode = ValidationConstants.RemovePatientIdBarcode, //Tfs 637943
                    RemoveMedLabelItemIdBarcode = ValidationConstants.RemoveMedLabelItemIdBarcode, //Tfs 637943
                    ExternalRefillRequestExpirationDuration = ValidationConstants.FacilityExternalRefillRequestDurationDefaultValue, // TFS 738530
                    ReverseDischargeDuration = ValidationConstants.FacilityReverseDischargeDurationDefaultValue,
                    MedSearchStringLength = ValidationConstants.MedSearchStringLengthDefaultValue,
                    DisablePyxisBarcodeScanOnLoadRefill = false, //TFS 1261179
                    UseEquivalencies = true, //TFS 788396
                    MedSearchString = ValidationConstants.MedSearchStringDefaultValue,

                    MedicationQueueBeforeOrderStartDuration = ValidationConstants.FacilityMedicationQueueBeforeOrderStartDurationDefaultValue,
                    MedicationQueueAfterOrderExpiredDuration = ValidationConstants.FacilityMedicationQueueAfterOrderExpiredDurationDefaultValue,
                    RepickWaitDuration = ValidationConstants.FacilityRepickWaitDurationDefaultValue,
                    GCSMDispenseMultiMedSheetReconciliation = ValidationConstants.GCSMDispenseMultiMedSheetReconciliation,
                    GCSMShowInvoiceType = ValidationConstants.GCSMShowInvoiceType,
                    GCSMAddItemFromCountDestructionBinFlag = ValidationConstants.GCSMAddItemFromCountDestructionBinFlag,
                    GCSMAddItemFromEmptyDestructionBinFlag = ValidationConstants.GCSMAddItemFromEmptyDestructionBinFlag,
                    GCSMChangeEmptyDestructionBinQuantityFlag = ValidationConstants.GCSMChangeEmptyDestructionBinQuantityFlag
                };
            }
        }

        #endregion

        #region Operator Overloads

        public static implicit operator Facility(Guid key)
        {
            return FromKey(key);
        }

        public static Facility FromKey(Guid key)
        {
            return new Facility(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a facility.
        /// </summary>
        [Column("FacilityKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a facility snapshot.
        /// </summary>
        [Column("FacilitySnapshotKey")]
        public Guid SnapshotKey { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies a facility.
        /// </summary>
        [Column("FacilityCode")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name of the facility.
        /// </summary>
        /// <value>The name.</value>
        [Column("FacilityName")]
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the text that describes the facility.
        /// </summary>
        /// <value>The description.</value>
        [Column("DescriptionText")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this facility is active.
        /// </summary>
        /// <value><c>true</c> if this facility is active; otherwise, <c>false</c>.</value>
        [Column("ActiveFlag")]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the part of a facility's address that is more specific than a city name.
        /// </summary>
        [Column("StreetAddressText")]
        public string StreetAddress { get; set; }

        /// <summary>
        /// Gets or sets the name of a city in a facility's address.
        /// </summary>
        [Column("CityName")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the name of a part of a country, such as a state or country, in
        /// a facility's address.
        /// </summary>
        [Column("SubdivisionName")]
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies a postal location in a facility's address.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the name of a country where a facility is located.
        /// </summary>
        [Column("CountryName")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the free-form notes about a facility.
        /// </summary>
        [Column("NotesText")]
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the ID of a facility as per the company.
        /// </summary>
        public string SiteId { get; set; }

        /// <summary>
        /// Gets or sets the ID for a time zone that a facility resides in.
        /// </summary>
        public string TimeZoneId { get; set; }

        /// <summary>
        /// Gets or sets the default duration (in hours) that is use to filter encounters
        /// on patient reconciliation UI's.
        /// </summary>
        [Column("PatientReconciliationDefaultFilterDurationAmount")]
        public short? PatientReconciliationDefaultFilterDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) that a temporary patient is shown.
        /// </summary>
        [Column("TemporaryPatientDurationAmount")]
        public short TemporaryPatientDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) after which transactions for unreconciled temporary
        /// encounters are resent outbound.
        /// </summary>
        [Column("ResendAfterTemporaryRemainsUnmergedDurationAmount")]
        public short ResendAfterTemporaryRemainsUnmergedDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) that users have a temporary access to an area
        /// for med purposes.
        /// </summary>
        [Column("MedTemporaryUserAccessDurationAmount")]
        public short MedicationTemporaryUserAccessDuration { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the roles a visitor has are used to restrict what
        /// rights the visitor has.
        /// </summary>
        [Column("UseVisitorRolesInMedTemporaryUserAccessFlag")]
        public bool UseVisitorRolesInMedTemporaryUserAccess { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) that defaults into a new facility item's
        /// too-close removal duration.
        /// </summary>
        [Column("TooCloseRemoveDurationAmount")]
        public short? TooCloseRemoveDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) after which there is deemed to be out-of-date ADT data 
        /// because there is a lack of recently received ADT messages
        /// </summary>
        [Column("NoADTMessageReceivedDurationAmount")]
        public short? NoADTMessageReceivedDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) after which there is deemed to be out-of-date PIS data
        /// because there is a lack of recently received pharmacy-order messages
        /// </summary>
        [Column("NoPISMessageReceivedDurationAmount")]
        public short? NoPISMessageReceivedDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) after which there is deemed to be out-of-date PIS data
        /// because the processing of order messages is behind
        /// </summary>
        [Column("PISMessageProcessingBehindDurationAmount")]
        public short? PISMessageProcessingBehindDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) after which there is deemed to be out-of-date ADT data
        /// because the processing of ADT messages is behind
        /// </summary>
        [Column("ADTMessageProcessingBehindDurationAmount")]
        public short? ADTMessageProcessingBehindDuration { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of an external system that is a pharmacy
        /// information system.
        /// </summary>
        public Guid? PharmacyInformationSystemKey { get; set; }

        /// <summary>
        /// Gets the pharmacy information system name.
        /// </summary>
        public string PharmacyInformationSystemName { get; internal set; }

        /// <summary>
        /// Gets or sets the authentication method.
        /// </summary>
        public string BioIdFailoverInternalCode { get; set; }

        /// <summary>
        /// Gets or sets the authentication method.
        /// </summary>
        public string RfidFailoverInternalCode { get; set; }

        /// <summary>
        /// Gets or sets the destruction bin emptying discrepancies option
        /// </summary>
        public string GCSMDestructionBinEmptyWithDiscrepancyModeInternalCode { get; set; }

        /// <summary>
        /// Gets authentication method.
        /// </summary>
        public AuthenticationMethodInternalCode BioIdFailover
        {
            get { return BioIdFailoverInternalCode.FromInternalCode<AuthenticationMethodInternalCode>(); }
        }

        /// <summary>
        /// Gets destruction bin codes
        /// </summary>
        public DestructionBinEmptyWithDiscrepancyModeInternalCode DestructionBinEmptyWithDiscrepancyMode
        {
            get {
                return !string.IsNullOrEmpty(GCSMDestructionBinEmptyWithDiscrepancyModeInternalCode) ?
                         GCSMDestructionBinEmptyWithDiscrepancyModeInternalCode.FromInternalCode<DestructionBinEmptyWithDiscrepancyModeInternalCode>() :
                         Default.GCSMDestructionBinEmptyWithDiscrepancyModeInternalCode.FromInternalCode<DestructionBinEmptyWithDiscrepancyModeInternalCode>(); }
        }

        /// <summary>
        /// Gets or sets the internal code that identifies a user scan mode.
        /// </summary>
        public string UserScanModeInternalCode { get; set; }

        /// <summary>
        /// Gets internal code that identifies a user scan mode.
        /// </summary>
        public UserScanModeInternalCode UserScanMode
        {
            get { return UserScanModeInternalCode.FromInternalCode<UserScanModeInternalCode>(); }
        }

        /// <summary>
        /// Gets or sets the internal code that identifies a sequential drain mode.
        /// </summary>
        public string SequentialDrainModeInternalCode { get; set; }

        /// <summary>
        /// Gets internal code that identifies a sequential drain mode.
        /// </summary>
        public SequentialDrainModeInternalCode? SequentialDrainMode
        {
            get { return SequentialDrainModeInternalCode.FromNullableInternalCode<SequentialDrainModeInternalCode>(); }
        }

        /// <summary>
        /// Gets or sets the value that indicates whether a free-form override or too-close reason
        /// may be entered against an item transaction.
        /// </summary>
        [Column("FreeFormReasonFlag")]
        public bool AllowFreeFormReason { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether or not an unverified scan code is used during
        /// inventory and dispensing functions.
        /// </summary>
        [Column("UseUnverifiedScanCodeFlag")]
        public bool UseUnverifiedScanCode { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) which when reached makes an order task past due.
        /// </summary>
        [Column("LateRemoveDurationAmount")]
        public short LateRemoveDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in days) before a defined storage space inventory's earliest expiration
        /// date to notify a dispensing device user to check for expiring meds.
        /// </summary>
        [Column("MedExpirationCheckDurationAmount")]
        public short MedicationExpirationCheckDuration { get; set; }

        /// <summary>
        /// Gets or sets the value that indicated whether critical override scheduling is enabled.
        /// </summary>
        [Column("CriticalOverrideSchedulingFlag")]
        public bool CriticalOverrideScheduling { get; set; }

        /// <summary>
        /// Gets or sets the duration (in days) after which a loaded medication is considered "least removed"
        /// for inventory purposes.
        /// </summary>
        [Column("LeastRemovedDurationAmount")]
        public short? LeastRemovedDuration { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether all items on a remove list is by default selected.
        /// </summary>
        [Column("SelectAllForRemoveListFlag")]
        public bool SelectAllForRemoveList { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) that is used to determine which discrepancies are included
        /// for attention-notice purposes such that each discrepancy must be unresolved for at least the delay
        /// duration.
        /// </summary>
        [Column("NoticeDiscrepancyDelayDurationAmount")]
        public short NoticeDiscrepancyDelayDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) that is used to determine which not communicating dispensing
        /// devices are included for attention-notice purposes such that each device must not have been 
        /// communicating for at least the delay duration.
        /// </summary>
        [Column("NoticeDeviceNotCommunicatingDelayDurationAmount")]
        public short NoticeDeviceNotCommunicatingDelayDuration { get; set; }

        /// <summary>
        /// Gets or sets the value that controls whether each dispensing device that is on critical override because
        /// of a scheduled critical override is included within the list of devices on critical override for
        /// attention-notice purposes.
        /// </summary>
        [Column("NoticeIncludeScheduledCriticalOverrideFlag")]
        public bool NoticeIncludeScheduledCriticalOverride { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) before an attention notice is displayed when the ETL for reporting failed.
        /// </summary>
        [Column("NoticeETLDelayDurationAmount")]
        public short NoticeEtlDelayDuration { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a remove quantity be less than an ordered dose.
        /// </summary>
        [Column("DecreaseOrderedDoseFlag")]
        public bool DecreaseOrderedDose { get; set; }

        /// <summary>
        /// Gets or sets the value tha indicates whether OMNL should be print for ordered meds with equivalencies when
        /// the ordered med is not loaded at the dispensing device.
        /// </summary>
        [Column("OMNLToPrintEquivalenciesFlag")]
        public bool OMNLToPrintEquivalencies { get; set; }

        /// <summary>
        /// Gets or sets the text displayed as temporary patient ID when no temporary ID is provided.
        /// </summary>
        public string TemporaryPatientNoIdText { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the Encounter ID is used as the primary
        /// patient display ID for the facility.
        /// </summary>
        [Column("DisplayEncounterIDFlag")]
        public bool DisplayEncounterId { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the Alternate Encounter ID is used as the primary
        /// patient display ID for the facility.
        /// </summary>
        [Column("DisplayAlternateEncounterIDFlag")]
        public bool DisplayAlternateEncounterId { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the Account ID is used as the primary
        /// patient display ID for the facility.
        /// </summary>
        [Column("DisplayAccountIDFlag")]
        public bool DisplayAccountId { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates the patient Id type that is used as the
        /// primary patient display ID for the facility.
        /// </summary>
        [Column("DisplayPatientIDTypeKey")]
        public Guid? DisplayPatientIdentificationTypeKey { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a user can enter an encounter ID for a 
        /// temporary patient, in addition to the facility display ID.
        /// </summary>
        [Column("OptionalTemporaryEncounterIDFlag")]
        public bool OptionalTemporaryEncounterId { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a user can enter an alternate encounter ID
        /// for a temporary patient, in addition to the facility display ID.
        /// </summary>
        [Column("OptionalTemporaryAlternateEncounterIDFlag")]
        public bool OptionalTemporaryAlternateEncounterId { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a user can enter an account ID for a
        /// temporary patient, in addtion to the facility display ID.
        /// </summary>
        [Column("OptionalTemporaryAccountIDFlag")]
        public bool OptionalTemporaryAccountId { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a patient identification type that a user
        /// can enter for a temporary patient, in addition to the facility display ID.
        /// </summary>
        [Column("OptionalTemporaryPatientIDTypeKey")]
        public Guid? OptionalTemporaryPatientIdentificationTypeKey { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the remove med label patient ID
        /// contains a display ID.
        /// </summary>
        [Column("RemoveMedLabelDisplayIDFlag")]
        public bool RemoveMedicationLabelDisplayId { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the remove med label patient ID
        /// contains an encounter ID.
        /// </summary>
        [Column("RemoveMedLabelEncounterIDFlag")]
        public bool RemoveMedicationLabelEncounterId { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the remove med label patient ID
        /// contains an alternative encounter ID.
        /// </summary>
        [Column("RemoveMedLabelAlternateEncounterIDFlag")]
        public bool RemoveMedicationLabelAlternateEncounterId { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the remove med label patient ID contains
        /// an account ID.
        /// </summary>
        [Column("RemoveMedLabelAccountIDFlag")]
        public bool RemoveMedicationLabelAccountId { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a patient identification type that is shown as the remove
        /// mdeication label patient ID.
        /// </summary>
        [Column("RemoveMedLabelPatientIDTypeKey")]
        public Guid? RemoveMedicationLabelPatientIdentificationTypeKey { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the remove med label contains a product ID,
        /// else a medication ID.
        /// </summary>
        [Column("RemoveMedLabelProductIDFlag")]
        public bool RemoveMedicationLabelProductId { get; set; }

        /// <summary>
        /// Gets or sets the not returnable medication message.
        /// </summary>
        [Column("NotReturnableMedMessageText")]
        public string NotReturnableMedicationMessage { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a user can cancel a dispense transaction at either
        /// a med or anesthesia device for transactions from a mini tray in single-dose mode.
        /// </summary>
        [Column("AllowSingleMultiDoseDispenseCancelFlag")]
        public bool AllowSingleMultiDoseDispenseCancel { get; set; }

        /// <summary>
        /// Gets or sets the value that determines whether a user may remove an ordered med even though the 
        /// pharmacy order is without an ordered dose.
        /// </summary>
        [Column("RemoveOrderedItemWithoutDoseFlag")]
        public bool RemoveOrderedItemWithoutDose { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether remote dispensing is enabled for a facility.
        /// </summary>
        [Column("RemoteDispensingFlag")]
        public bool RemoteDispensing { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether facility users may access the delivery feature.
        /// </summary>
        [Column("DeliveryFlag")]
        public bool Delivery { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) before a patient medication queue expires.
        /// </summary>
        [Column("MedQueueDurationAmount")]
        public short MedicationQueueDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) prior to a scheduled task's due time before it
        /// can be queued.
        /// </summary>
        [Column("MedQueueBeforeDueDurationAmount")]
        public short MedicationQueueBeforeDueDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) prior to a pharmacy order's start time before it
        /// can be queued.
        /// </summary>
        [Column("MedQueueBeforeOrderStartDurationAmount")]
        public short? MedicationQueueBeforeOrderStartDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) after a pharmacy order's expired time that it
        /// can be queued.
        /// </summary>
        [Column("MedQueueAfterOrderExpiredDurationAmount")]
        public short? MedicationQueueAfterOrderExpiredDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) prior to a scheduled task's due time before it
        /// is displayed in the Due Now window.
        /// </summary>
        [Column("MedQueueBeforeDueNowDurationAmount")]
        public short MedicationQueueBeforeDueNowDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) after a scheduled task's due time before it is
        /// displayed in the Due Now window.
        /// </summary>
        [Column("MedQueueAfterDueNowDurationAmount")]
        public short MedicationQueueAfterDueNowDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) that is subtracted from a preadmit date time and that
        /// is used to determine whether an encounter is shown.
        /// </summary>
        [Column("PreadmissionLeadDurationAmount")]
        public short PreadmissionLeadDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) that is added to an expected admit date time and that
        /// is used to determine whether an encounter is shown.
        /// </summary>
        [Column("PreadmissionProlongedInactivityDurationAmount")]
        public short PreadmissionProlongedInactivityDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in days) of inactivity after which an encounter is considered to be
        /// in a state of prolonged inactivity.
        /// </summary>
        [Column("AdmissionProlongedInactivityDurationAmount")]
        public short AdmissionProlongedInactivityDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) that is added to a discharge date time and that is used
        /// to determine whether dispensing activities may be performed against an encounter.
        /// </summary>
        [Column("DischargeDelayDurationAmount")]
        public short DischargeDelayDuration { get; set; }

        [Column("DischargeDelayCancelFlag")]
        public bool DischargeDelayCancel { get; set; }

        [Column("UseEquivalenciesFlag")]
        public bool UseEquivalencies { get; set; }

        [Column("FacilitySpecificEquivalenciesFlag")]
        public bool AllowFacilitySpecificEquivalencies { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) that is used to determine whether an encounter is
        /// associated with a dispensing device following unit or facility transfer.
        /// </summary>
        [Column("TransferDelayDurationAmount")]
        public short TransferDelayDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) that the system uses to determine when to notify the user
        /// with items in their my-items list.
        /// </summary>
        [Column("LeaveOfAbsenceDelayDurationAmount")]
        public short LeaveOfAbsenceDelayDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) that the system uses to determine when to notify the user
        /// with items in their my-items list.
        /// </summary>
        [Column("MyItemsNotificationDurationAmount")]
        public short? MyItemsNotificationDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) of an availability of a medication's delivery status for a 
        /// clinician to view.
        /// </summary>
        [Column("DeliveryStatusDisplayDurationAmount")]
        public short DeliveryStatusDisplayDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) of when orders should be discontinued on readmit of an encounter.
        /// </summary>
        [Column("DiscontinueOrdersOnReadmitDurationAmount")]
        public short? DiscontinueOrdersOnReadmitDuration { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a muli-component pharmacy order is removable even if
        /// some of the components are not removable at a dispensing device.
        /// </summary>
        [Column("PartialMultiComponentOrderRemoveFlag")]
        public bool PartialMultiComponentOrderRemove { get; set; }

        /// <summary>
        /// Gets or sets the value of RemovePatientIdBarcode
        /// </summary>
        [Column("RemovePatientIDBarcodeFlag")]
        public bool RemovePatientIdBarcode { get; set; }

        /// <summary>
        /// Gets or sets the value of RemoveMedLabelItemIdBarcode
        /// </summary>
        [Column("RemoveMedLabelItemIDBarcodeFlag")]
        public bool RemoveMedLabelItemIdBarcode { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a removed quantity may be greater than an ordered dose.
        /// </summary>
        [Column("IncreaseOrderedDoseFlag")]
        public bool IncreaseOrderedDose { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) an item displays in an RxCheck list at a dispensing device
        /// provided all other RxCheck rules are met.
        /// </summary>
        [Column("RxCheckExpirationDurationAmount")]
        public short RxCheckExpirationDuration { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether CUBIES and single/multi-dose mode MiniDrawer trays
        /// are excluded from being RxChecked.
        /// </summary>
        [Column("ExcludeRxCheckCUBIEOrSingleMultiDoseMiniFlag")]
        public bool ExcludeRxCheckCubieOrSingleMultiDoseMini { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether items scanned on load/refill are excluded from being RxChecked.
        /// </summary>
        [Column("ExcludeRxCheckScanOnLoadRefillFlag")]
        public bool ExcludeRxCheckScanOnLoadRefill { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) before an attention notice displays for a device with items
        /// not RxChecked.
        /// </summary>
        [Column("RxCheckDelayDurationAmount")]
        public short RxCheckDelayDuration { get; set; }

        /// <summary>
        /// Gets or sets the URL for the JIT management console that uses the JIT database for the facility.
        /// </summary>
        [Column("JITMgmtURLID")]
        public string JitManagementUrlId { get; set; }

        /// <summary>
        /// Gets or sets the URL for the JIT management console that uses the JIT database for the facility.
        /// </summary>
        public string PharmogisticsUrlId { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether only non-med items may be added as a facility-level item.
        /// </summary>
        [Column("AddFacilityNonMedItemOnlyFlag")]
        public bool AddFacilityNonMedItemOnly { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the system disables the pend and assign outdate tracking flags
        /// so a user cannot turn outdate tracking 'off' for a formulary item with the flag 'on'.
        /// </summary>
        [Column("DisablePendAssignOutdateTrackingFlag")]
        public bool DisablePendAssignOutdateTracking { get; set; }

        /// <summary>
        /// Gets or sets the text that shows if an allergen description is truncated.
        /// </summary>
        [Column("TruncatedAllergyCommentText")]
        public string TruncatedAllergyComment { get; set; }

        /// <summary>
        /// Gets or sets the text that shows if either an order dispensing instruction or an order administration instruction
        /// is truncated.
        /// </summary>
        [Column("TruncatedOrderCommentText")]
        public string TruncatedOrderComment { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a user is required to scan either their user ID
        /// or some other scan code when being authenticated by the device.
        /// </summary>
        [Column("DispensingDeviceUserScanFlag")]
        public bool DispensingDeviceUserScan { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether to match scan against user scan code (rather than user ID).
        /// </summary>
        [Column("MatchByUserScanCodeFlag")]
        public bool MatchByUserScanCode { get; set; }

        /// <summary>
        /// Gets or sets the text that is embedded within a user ID barcode at the beginning of the barcode.
        /// </summary>
        [Column("UserIDScanCodePrefixText")]
        public string UserIdScanCodePrefix { get; set; }

        /// <summary>
        /// Gets or sets the text that is embedded within a user ID barcode at the end of the barcode.
        /// </summary>
        [Column("UserIDScanCodeSuffixText")]
        public string UserIdScanCodeSuffix { get; set; }

        /// <summary>
        /// Gets or sets the text that is embedded within a order ID barcode at the beginning of the barcode.
        /// </summary>
        [Column("OrderIDScanCodePrefixText")]
        public string OrderIdScanCodePrefix { get; set; }

        /// <summary>
        /// Gets or sets the text that is embedded within a order ID barcode at the end of the barcode.
        /// </summary>
        [Column("OrderIDScanCodeSuffixText")]
        public string OrderIdScanCodeSuffix { get; set; }

        /// <summary>
        /// Gets or sets the number of leading characters that are trimmed from an order ID scan code.
        /// </summary>
        [Column("OrderIDScanCodePrefixLengthQuantity")]
        public byte? OrderIdScanCodePrefixLength { get; set; }

        /// <summary>
        /// Gets or sets the number of trailing characters that are trimmed from an order ID scan code.
        /// </summary>
        [Column("OrderIDScanCodeSuffixLengthQuantity")]
        public byte? OrderIdScanCodeSuffixLength { get; set; }

        /// <summary>
        /// Gets or sets the delimiter used to parse the text that follows it.
        /// </summary>
        [Column("OrderIDScanCodeSuffixDelimiterValue")]
        public string OrderIdScanCodeSuffixDelimiterValue { get; set; }

        /// <summary>
        /// Gets or sets the custom expression used to trim an order ID scan code.
        /// </summary>
        [Column("OrderIDScanCodeSuffixCustomExpressionText")]
        public string OrderIdScanCodeSuffixCustomExpressionText { get; set; }

        /// <summary>
        /// Gets or sets the duration (in minutes) that passes after which an item is eligible to be picked again.
        /// </summary>
        [Column("RepickWaitDurationAmount")]
        public short? RepickWaitDuration { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether CUBIE ES functionality is enabled.
        /// </summary>
        [Column("CUBIEESFlag")]
        public bool CubieES { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a station(s) will display
        /// a list of items for tasks sent by HealthSight
        /// </summary>
        [Column("HealthSightInventoryTasksFlag")]
        public bool HealthSightInventoryTasks { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether unchecked CUBIE pockets may be inserted
        /// </summary>
        [Column("InsertUncheckedCUBIEFlag")]
        public bool InsertUncheckedCubie { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether a CUBIE pocket programmed to be inserted
        /// for one dispensing device may be inserted into another
        /// </summary>
        [Column("CUBIEAnotherDestinationFlag")]
        public bool CubieAnotherDestination { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CriteriaBasedFillFlag")]
        public bool CriteriaBasedFill { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether the facility uses CardinalASSIST
        /// </summary>
        [Column("CardinalASSISTFlag")]
        public bool CardinalAssist { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether a facility is a long term care (LTC) facility
        /// </summary>
        [Column("LongTermCareFlag")]
        public bool LongTermCare { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a profile MedStation always opend the "All Orders"
        /// tab when a user indicates the need to remove from a profile list; when not true, the MedStation
        /// may open the "Due Now", "PRN", or "All Orders" tab based on the user's selected order type from
        /// the patient summary view.
        /// </summary>
        [Column("DispensingDeviceAllOrdersFlag")]
        public bool DispensingDeviceAllOrders { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether queuing mode is profile for areas that have a mix of both
        /// profile and non-profile devices, including temporary non-profile
        /// </summary>
        [Column("MixedDeviceTypeQueuingModeProfileFlag")]
        public bool MixedDeviceTypeQueuingModeProfile { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether the request-a-dose feature is allowed for a facility
        /// </summary>
        [Column("RequestPharmacyOrderDoseFlag")]
        public bool RequestPharmacyOrderDose { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) that request for a pharmacy-order dose is displayed
        /// </summary>
        [Column("RequestPharmacyOrderDoseDurationAmount")]
        public short RequestPharmacyOrderDoseDuration { get; set; }

        [Column("AttentionNoticeCriticalThresholdDurationAmount")]
        public short AttentionNoticeCriticalThresholdDuration { get; set; }

        [Column("StatusBoardDoseRequestDisplayDurationAmount")]
        public short StatusBoardDoseRequestDisplayDuration { get; set; }

        [Column("StatusBoardNewDoseRequestDisplayDurationAmount")]
        public short StatusBoardNewDoseRequestDisplayDuration { get; set; }

        [Column("UnknownAdmissionStatusRetentionDurationAmount")]
        public short UnknownAdmissionStatusRetentionDuration { get; set; }

        /// <summary>
        /// Gets or sets the binary value that is unique within a database and that is assigned a new
        /// value on insert and on update.
        /// </summary>
        [Column("LastModifiedBinaryValue")]
        public byte[] LastModified { get; set; }

        /// <summary>
        /// Get or Set the flag that indicates whether new PIS items are automatically approved for a facility
        /// </summary>
        [Column("AutoApprovePISItemFlag")]
        public bool AutoApprove { get; set; }

        /// <summary>
        /// Gets or sets the retention period (in hours) that is used for reversal of the discharge 
        /// for a patient visit against a patient that is already discharged from the ADT system can occur
        /// </summary>
        [Column("ReverseDischargeDurationAmount")]
        public short ReverseDischargeDuration { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether teh discharge of a previously discharged patient visit
        /// can be reversed
        /// </summary>
        [Column("ReverseDischargeFlag")]
        public bool ReverseDischarge { get; set; }

        /// <summary>
        /// Gets or sets the primary med contact.
        /// </summary>
        public FacilityContact PrimaryMedContact { get; set; }

        /// <summary>
        /// Gets or sets the secondary med contact.
        /// </summary>
        public FacilityContact SecondaryMedContact { get; set; }

        /// <summary>
        /// Gets or sets the primary supply contact.
        /// </summary>
        public FacilityContact PrimarySupplyContact { get; set; }

        /// <summary>
        /// Gets or sets the secondary supply contact.
        /// </summary>
        public FacilityContact SecondarySupplyContact { get; set; }

        /// <summary>
        /// Gets or sets the notice types for a facility.
        /// </summary>
        public FacilityNoticeType[] NoticeTypes { get; set; }

        /// <summary>
        /// Gets or sets the no recent message received associated with a facility.
        /// </summary>
        public NoRecentMessageReceivedConfiguration[] NoRecentMessageReceivedConfigurations { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether the System will support an inventory worklist from an external system
        /// </summary>
        [Column("ExternalInventoryCountRequestFlag")]
        public bool ExternalInventoryCountRequest { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether the System will support a refill worklist from an external system
        /// </summary>
        [Column("ExternalRefillRequestFlag")]
        public bool ExternalRefillRequest { get; set; }

        /// <summary>
        /// Gets or sets the duration (in hours) before an external refill request expires and is no longer displayed to the user
        /// </summary>
        [Column("ExternalRefillRequestExpirationDurationAmount")]
        public short ExternalRefillRequestExpirationDuration { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether the System will support Independent Facility User Management
        /// </summary>
        [Column("IndependentFacilityUserManagementFlag")]
        public bool IndependentFacilityUserManagement { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether the System will support Pharmacy Facility
        /// </summary>
        [Column("PharmacyFacilityFlag")]
        public bool PharmacyFacility { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether the System will require ProductID Lot expiration flag
        /// </summary>
        [Column("RequireProductIDLotIDExpirationDateFlag")]
        public bool RequireProductIDLotIDExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether the System will require medication search flag
        /// </summary>
        [Column("MedSearchStringFlag")]
        public bool MedSearchString { get; set; }


        /// <summary>
        /// Gets or sets the number of leading characters for medication search.
        /// </summary>
        [Column("MedSearchStringLengthQuantity")]
        public byte MedSearchStringLength { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether the System will disable pyxis barcode scan on load refill
        /// </summary>
        [Column("DisablePyxisBarcodeScanOnLoadRefillFlag")]
        public bool DisablePyxisBarcodeScanOnLoadRefill { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether the System will Require an Purchase Order ID on Receive
        /// </summary>
        [Column("GCSMReceivePurchaseOrderRequiredFlag")]
        public bool GCSMReceivePurchaseOrderRequired { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether the System will interface with Logistics for Ordering
        /// </summary>
        [Column("GCSMLogisticsOrderingInterfaceSupportFlag")]
        public bool GCSMLogisticsOrderingInterfaceSupport { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether the System will interface with Logistics for Receive
        /// </summary>
        [Column("GCSMLogisticsReceiveInterfaceSupportFlag")]
        public bool GCSMLogisticsReceiveInterfaceSupport { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether the System will interface with Cardinal Assist
        /// </summary>
        [Column("GCSMCardinalAssistInterfaceSupportFlag")]
        public bool GCSMCardinalAssistInterfaceSupport { get; set; }

        [Column("GCSMDefaultOnReceiveDistributorKey")]
        public Guid? GCSMDefaultOnReceiveDistributorKey { get; set; }

        [Column("GCSMShowInvoiceTypeFlag")]
        public bool GCSMShowInvoiceType { get; set; }

        [Column("GCSMChangeEmptyDestructionBinQuantityFlag")]
        public bool GCSMChangeEmptyDestructionBinQuantityFlag { get; set; }

        [Column("GCSMAddItemFromCountDestructionBinFlag")]
        public bool GCSMAddItemFromCountDestructionBinFlag { get; set; }

        [Column("GCSMAddItemFromEmptyDestructionBinFlag")]
        public bool GCSMAddItemFromEmptyDestructionBinFlag { get; set; }

        [Column("GCSMDispenseMultiMedSheetReconciliationFlag")]
        public bool GCSMDispenseMultiMedSheetReconciliation { get; set; }

        [Column("GCSMAllDeviceEventsReviewSignaturesFlag")]
        public bool GCSMAllDeviceEventsReviewSignatures { get; set; }

        [Column("GCSMPrintADMLabelFlag")]
        public bool GCSMPrintADMLabel { get; set; }

        [Column("GCSMPrintLabelByDispenseOrderFlag")]
        public bool GCSMPrintLabelByDispenseOrder { get; set; }

        [Column("FirstSnapshotFlag")]
        public bool FirstSnapshot { get; set; }

        public SheetConfiguration[] SheetConfigurations { get; set; }

        [Column("MedSelectionOrderFlag")]
        public bool MedSelectionOrder { get; set; }

        [Column("AllowPatientsToBeSearchedByLocationFlag")]
        public bool AllowPatientsToBeSearchedByLocation { get; set; }

        [Column("DefaultSortPatientsByLocationFlag")]
        public bool DefaultSortPatientsByLocation { get; set; }

        [Column("UserAuthenticationRequestDuration2Amount")]
        public short UserAuthenticationRequestDuration2 { get; set; }

        [Column("DisplayPatientPreferredNameOnRemoveLabelFlag")]
        public bool DisplayPatientPreferredNameOnRemoveLabel { get; set; }
        #endregion

        #region Public Members

        public bool IsTransient()
        {
            return Key == default(Guid);
        }

        #endregion
    }
}
