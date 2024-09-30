using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using Dapper;
using Pyxis.Core.Data;
using Pyxis.Core.Data.InternalCodes;
using Pyxis.Core.Data.Schema.TableTypes;
using LocationDAL = Pyxis.Core.Data.Schema.Location;
using NoticeDAL = Pyxis.Core.Data.Schema.Notice;
using SheetConfigDAL = Pyxis.Core.Data.Schema.Rx;

namespace CareFusion.Dispensing.Data.Repositories
{
    #region IFacilityRepository Interface

    public interface IFacilityRepository
    {
        /// <summary>
        /// Retrieves a collection of <see cref="Facility"/> by key.
        /// </summary>
        /// <param name="facilityFilter">The collection of facility keys or NULL for all.</param>
        /// <param name="facilityCodeFilter"></param>
        /// <param name="pharmacyInformationSystemFilter"></param>
        /// <param name="activeFilter"></param>
        /// <returns>An IReadOnlyCollection(T) object, where the generic parameter T is <see cref="Facility"/>.</returns>
        IReadOnlyCollection<Facility> GetFacilities(
            Filter<IEnumerable<Guid>> facilityFilter = default(Filter<IEnumerable<Guid>>), 
            Filter<string> facilityCodeFilter = default(Filter<string>), 
            Filter<Guid?> pharmacyInformationSystemFilter = default(Filter<Guid?>),
            Filter<bool> activeFilter = default(Filter<bool>));

        /// <summary>
        /// Gets a list of surrogate keys that matches the specified pharmacy information system key.
        /// </summary>
        /// <param name="pharmacyInformationSystemKey">The pharmacy information system key.</param>
        /// <returns>A IEnumerable(T) object, where the generic parameter T is <see cref="Facility"/>.</returns>
        IReadOnlyCollection<Guid> GetFacilityKeys(Guid pharmacyInformationSystemKey);

        /// <summary>
        /// Gets the facility that matches the specified key.
        /// </summary>
        /// <param name="facilityKey">The facility key.</param>
        /// <returns>A <see cref="Facility"/> object.</returns>
        Facility GetFacility(Guid facilityKey);

        /// <summary>
        /// Gets the facility that matches the specified code.
        /// </summary>
        /// <param name="facilityCode">The facility key.</param>
        /// <returns>A <see cref="Facility"/> object.</returns>
        Facility GetFacility(string facilityCode);

        bool FacilityNameExists(string name, Filter<Guid> ignoreFacilityKey = default(Filter<Guid>));

        bool FacilityCodeExists(string code, Filter<Guid> ignoreFacilityKey = default(Filter<Guid>));

        bool HasFacilityItemsAssociated(Guid facilityKey, Guid externalSystemKey);

        /// <summary>
        /// Persists the new facility to the database.
        /// </summary>
        /// <param name="context">The application context.</param>
        /// <param name="facility">The facility to persist.</param>
        /// <returns>A facility key, which uniquely identifies the facility in the database.</returns>
        Guid InsertFacility(Context context, Facility facility);

        /// <summary>
        /// Updates an existing facility in the database.
        /// </summary>
        /// <param name="context">The application context.</param>
        /// <param name="facility">The facility to update.</param>
        void UpdateFacility(Context context, Facility facility);
    }

    #endregion

    public class FacilityRepository : IFacilityRepository
    {
        static FacilityRepository()
        {
            // Dapper custom mappings
            SqlMapper.SetTypeMap(
                typeof(Facility),
                new ColumnAttributeTypeMapper<Facility>());
            SqlMapper.SetTypeMap(
                typeof(FacilityContact),
                new ColumnAttributeTypeMapper<FacilityContact>());
            SqlMapper.SetTypeMap(
                typeof(FacilityNoticeType),
                new ColumnAttributeTypeMapper<FacilityNoticeType>());
            SqlMapper.SetTypeMap(
                typeof(NoRecentMessageReceivedConfiguration),
                new ColumnAttributeTypeMapper<NoRecentMessageReceivedConfiguration>());
            SqlMapper.SetTypeMap(
               typeof(SheetConfiguration),
               new ColumnAttributeTypeMapper<SheetConfiguration>());
            SqlMapper.SetTypeMap(
              typeof(SheetConfigurationMedClassGroup),
              new ColumnAttributeTypeMapper<SheetConfigurationMedClassGroup>());
        }

        IReadOnlyCollection<Facility> IFacilityRepository.GetFacilities(
            Filter<IEnumerable<Guid>> facilityFilter,
            Filter<string> facilityCodeFilter,
            Filter<Guid?> pharmacyInformationSystemFilter,
            Filter<bool> activeFilter)
        {
            List<Facility> facilities = new List<Facility>();

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (facilityFilter.HasValue)
                {
                    if (facilityFilter.Value == null || !facilityFilter.Value.Any())
                        return facilities; // Empty results

                    if (facilityFilter.Value != null)
                        selectedKeys = new GuidKeyTable(facilityFilter.Value);
                }

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    using (var multi = connectionScope.QueryMultiple(
                        "Location.bsp_GetFacilities",
                        new
                        {
                            SelectedKeys = selectedKeys.AsTableValuedParameter(),
                            FacilityCode = facilityCodeFilter.HasValue ? facilityCodeFilter.Value : default(string),
                            PharmacyInformationSystemKey =
                                pharmacyInformationSystemFilter.HasValue
                                    ? pharmacyInformationSystemFilter.Value
                                    : default(Guid?),
                            ActiveFlag = activeFilter.HasValue ? activeFilter.Value : default(bool?)
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure))
                    {
                        facilities = multi.Read<Facility>()
                            .ToList();
                        var contactResults = multi.Read<FacilityContact>()
                            .ToArray();
                        var noticeTypeResults = multi.Read<FacilityNoticeType>()
                            .ToLookup(fnt => fnt.FacilityKey);
                        var noRecentMessagesResults = multi.Read<NoRecentMessageReceivedConfiguration>()
                            .ToLookup(fnt => fnt.FacilityKey);
                        var sheetConfigResults = multi.Read<SheetConfiguration>()
                           .ToLookup(fnt => fnt.FacilityKey);
                        var sheetConfigMedClassGroupResults = multi.Read<SheetConfigurationMedClassGroup>()
                           .ToLookup(fnt => fnt.SheetConfigKey);

                        foreach (var facility in facilities)
                        {
                            facility.PrimaryMedContact = contactResults.FirstOrDefault(fc =>
                                fc.FacilityKey == facility.Key &&
                                fc.BusinessDomainInternalCode == BusinessDomainInternalCode.MED.ToInternalCode() &&
                                fc.Rank == FacilityContact.PrimaryContactRankValue);

                            facility.SecondaryMedContact = contactResults.FirstOrDefault(fc =>
                                fc.FacilityKey == facility.Key &&
                                fc.BusinessDomainInternalCode == BusinessDomainInternalCode.MED.ToInternalCode() &&
                                fc.Rank == FacilityContact.SecondaryContactRankValue);

                            facility.PrimarySupplyContact = contactResults.FirstOrDefault(fc =>
                                fc.FacilityKey == facility.Key &&
                                fc.BusinessDomainInternalCode == BusinessDomainInternalCode.SUP.ToInternalCode() &&
                                fc.Rank == FacilityContact.PrimaryContactRankValue);

                            facility.SecondarySupplyContact = contactResults.FirstOrDefault(fc =>
                                fc.FacilityKey == facility.Key &&
                                             fc.BusinessDomainInternalCode == BusinessDomainInternalCode.SUP.ToInternalCode() &&
                                             fc.Rank == FacilityContact.SecondaryContactRankValue);
                           
                            facility.GCSMDestructionBinEmptyWithDiscrepancyModeInternalCode = !string.IsNullOrEmpty(facility.GCSMDestructionBinEmptyWithDiscrepancyModeInternalCode) ?
                                                                              facility.GCSMDestructionBinEmptyWithDiscrepancyModeInternalCode.FromInternalCode<DestructionBinEmptyWithDiscrepancyModeInternalCode>().ToInternalCode() :
                                                                               Facility.Default.GCSMDestructionBinEmptyWithDiscrepancyModeInternalCode;

                            if (noticeTypeResults.Contains(facility.Key))
                            {
                                facility.NoticeTypes = noticeTypeResults[facility.Key].ToArray();
                            }

                            if (noRecentMessagesResults.Contains(facility.Key))
                            {
                                facility.NoRecentMessageReceivedConfigurations = noRecentMessagesResults[facility.Key].ToArray();
                            }

                            if (sheetConfigResults.Contains(facility.Key))
                            {
                                facility.SheetConfigurations = sheetConfigResults[facility.Key].ToArray();
                                if (sheetConfigMedClassGroupResults.Count > 0)
                                {
                                    var sheetConfigs = new List<SheetConfiguration>();
                                    foreach (var sheetConfig in facility.SheetConfigurations)
                                    {
                                        sheetConfig.MedClassGroups = sheetConfigMedClassGroupResults[sheetConfig.Key].ToArray();
                                        sheetConfigs.Add(sheetConfig);
                                    }
                                    facility.SheetConfigurations = sheetConfigs.ToArray();
                                }
                            }

                            
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return facilities;
        }

        IReadOnlyCollection<Guid> IFacilityRepository.GetFacilityKeys(Guid pharmacyInformationSystemKey)
        {
            List<Guid> facilityKeys = null;

            try
            {
                SqlBuilder query = new SqlBuilder();
                query.SELECT("FacilityKey")
                    .FROM("Location.FacilitySnapshot")
                    .WHERE("EndUTCDateTime IS NULL")
                    .WHERE("PharmacyInformationSystemKey = @PharmacyInformationSystemKey");

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    facilityKeys = connectionScope.Query(
                        query.ToString(),
                        new { PharmacyInformationSystemKey = pharmacyInformationSystemKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Select(x => (Guid)x.FacilityKey)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return facilityKeys;
        }

        Facility IFacilityRepository.GetFacility(Guid facilityKey)
        {
            var facilities =
                ((IFacilityRepository)this).GetFacilities(new[] { facilityKey });

            return facilities.FirstOrDefault();
        }

        Facility IFacilityRepository.GetFacility(string facilityCode)
        {
            var facilities =
                ((IFacilityRepository)this).GetFacilities(null, facilityCode);

            return facilities.FirstOrDefault();
        }

        bool IFacilityRepository.FacilityNameExists(string name, Filter<Guid> ignoreFacilityKey)
        {
            bool exists = false;

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    SqlBuilder query = new SqlBuilder();
                    query.SELECT()
                        ._("FacilityKey")
                        .FROM("Location.FacilitySnapshot")
                        .WHERE("EndUTCDateTime IS NULL")
                        .WHERE("FacilityName = @FacilityName");

                    if (ignoreFacilityKey.HasValue)
                    {
                        query.WHERE("FacilityKey <> @IgnoreFacilityKey");
                    }

                    exists = connectionScope.Query(
                        query.ToString(),
                        new
                        {
                            FacilityName = name,
                            IgnoreFacilityKey = ignoreFacilityKey.HasValue ? ignoreFacilityKey.GetValueOrDefault() : default(Guid?)
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Any();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return exists;
        }

        bool IFacilityRepository.FacilityCodeExists(string code, Filter<Guid> ignoreFacilityKey)
        {
            bool exists = false;

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    SqlBuilder query = new SqlBuilder();
                    query.SELECT()
                        ._("FacilityKey")
                        .FROM("Location.FacilitySnapshot")
                        .WHERE("EndUTCDateTime IS NULL")
                        .WHERE("FacilityCode = @FacilityCode");

                    if (ignoreFacilityKey.HasValue)
                    {
                        query.WHERE("FacilityKey <> @IgnoreFacilityKey");
                    }

                    exists = connectionScope.Query(
                        query.ToString(),
                        new
                        {
                            FacilityCode = code,
                            IgnoreFacilityKey = ignoreFacilityKey.HasValue ? ignoreFacilityKey.GetValueOrDefault() : default(Guid?)
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Any();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return exists;
        }

        public bool HasFacilityItemsAssociated(Guid facilityKey, Guid externalSystemKey)
        {
            bool hasAssociations = false;

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    SqlBuilder query = new SqlBuilder();
                    query.SELECT()
                        ._("fi.FacilityItemKey")
                        .FROM("Item.FacilityItem fi")
                        .INNER_JOIN("Item.ItemSnapshot itm ON itm.ItemKey = fi.ItemKey")
                        .WHERE("fi.DisassociationUTCDateTime IS NULL")
                        .WHERE("fi.FacilityKey = @FacilityKey")
                        .WHERE("itm.EndUTCDateTime IS NULL")
                        .WHERE("itm.ExternalSystemKey = @ExternalSystemKey");

                    hasAssociations = connectionScope.Query(
                        query.ToString(),
                        new
                        {
                            FacilityKey = facilityKey,
                            ExternalSystemKey = externalSystemKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Any();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return hasAssociations;
        }

        Guid IFacilityRepository.InsertFacility(Context context, Facility facility)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(facility, "facility");
            Guid facilityKey = Guid.Empty;

            try
            {
                LocationDAL.IFacilityRepository facilityRepository = new LocationDAL.FacilityRepository();

                using (ConnectionScopeFactory.Create())
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                {
                    facilityKey = facilityRepository.InsertFacility(context.ToActionContext(),
                        new LocationDAL.Models.Facility
                        {
                            FacilityKey = facility.Key,
                            FacilityCode = facility.Code,
                            FacilityName = facility.Name,
                            DescriptionText = facility.Description,
                            ActiveFlag = facility.IsActive,
                            StreetAddressText = facility.StreetAddress,
                            CityName = facility.City,
                            SubdivisionName = facility.State,
                            PostalCode = facility.PostalCode,
                            CountryName = facility.Country,
                            NotesText = facility.Notes,
                            SiteId = facility.SiteId,
                            TimeZoneId = facility.TimeZoneId,
                            PatientReconciliationDefaultFilterDurationAmount = facility.PatientReconciliationDefaultFilterDuration,
                            TemporaryPatientDurationAmount = facility.TemporaryPatientDuration,
                            ResendAfterTemporaryRemainsUnmergedDurationAmount = facility.ResendAfterTemporaryRemainsUnmergedDuration,
                            MedTemporaryUserAccessDurationAmount = facility.MedicationTemporaryUserAccessDuration,
                            UseVisitorRolesInMedTemporaryUserAccessFlag = facility.UseVisitorRolesInMedTemporaryUserAccess,
                            TooCloseRemoveDurationAmount = facility.TooCloseRemoveDuration,
                            PisMessageProcessingBehindDurationAmount = facility.PISMessageProcessingBehindDuration,
                            AdtMessageProcessingBehindDurationAmount = facility.ADTMessageProcessingBehindDuration,
                            NoAdtMessageReceivedDurationAmount = facility.NoADTMessageReceivedDuration,
                            NoPisMessageReceivedDurationAmount = facility.NoPISMessageReceivedDuration,
                            PharmacyInformationSystemKey = facility.PharmacyInformationSystemKey,
                            BioIdFailoverInternalCode = facility.BioIdFailoverInternalCode,
                            RFIDFailoverInternalCode = facility.RfidFailoverInternalCode,
                            UserScanModeInternalCode = facility.UserScanModeInternalCode,
                            FreeFormReasonFlag = facility.AllowFreeFormReason,
                            UseUnverifiedScanCodeFlag = facility.UseUnverifiedScanCode,
                            LateRemoveDurationAmount = facility.LateRemoveDuration,
                            MedExpirationCheckDurationAmount = facility.MedicationExpirationCheckDuration,
                            CriticalOverrideSchedulingFlag = facility.CriticalOverrideScheduling,
                            LeastRemovedDurationAmount = facility.LeastRemovedDuration,
                            SelectAllForRemoveListFlag = facility.SelectAllForRemoveList,
                            NoticeDiscrepancyDelayDurationAmount = facility.NoticeDiscrepancyDelayDuration,
                            NoticeDeviceNotCommunicatingDelayDurationAmount = facility.NoticeDeviceNotCommunicatingDelayDuration,
                            NoticeIncludeScheduledCriticalOverrideFlag = facility.NoticeIncludeScheduledCriticalOverride,
                            NoticeEtlDelayDurationAmount = facility.NoticeEtlDelayDuration,
                            DecreaseOrderedDoseFlag = facility.DecreaseOrderedDose,
                            OmnlToPrintEquivalenciesFlag = facility.OMNLToPrintEquivalencies,
                            TemporaryPatientNoIdText = facility.TemporaryPatientNoIdText,
                            DisplayEncounterIdFlag = facility.DisplayEncounterId,
                            DisplayAlternateEncounterIdFlag = facility.DisplayAlternateEncounterId,
                            DisplayAccountIdFlag = facility.DisplayAccountId,
                            DisplayPatientIdTypeKey = facility.DisplayPatientIdentificationTypeKey,
                            OptionalTemporaryEncounterIdFlag = facility.OptionalTemporaryEncounterId,
                            OptionalTemporaryAlternateEncounterIdFlag = facility.OptionalTemporaryAlternateEncounterId,
                            OptionalTemporaryAccountIdFlag = facility.OptionalTemporaryAccountId,
                            OptionalTemporaryPatientIdTypeKey = facility.OptionalTemporaryPatientIdentificationTypeKey,
                            RemoveMedLabelDisplayIdFlag = facility.RemoveMedicationLabelDisplayId,
                            RemoveMedLabelEncounterIdFlag = facility.RemoveMedicationLabelEncounterId,
                            RemoveMedLabelAlternateEncounterIdFlag = facility.RemoveMedicationLabelAlternateEncounterId,
                            RemoveMedLabelAccountIdFlag = facility.RemoveMedicationLabelAccountId,
                            RemoveMedLabelPatientIdTypeKey = facility.RemoveMedicationLabelPatientIdentificationTypeKey,
                            RemoveMedLabelProductIdFlag = facility.RemoveMedicationLabelProductId,
                            RemoveMedLabelItemIdBarcodeFlag = facility.RemoveMedLabelItemIdBarcode,
                            NotReturnableMedMessageText = facility.NotReturnableMedicationMessage,
                            AllowSingleMultiDoseDispenseCancelFlag = facility.AllowSingleMultiDoseDispenseCancel,
                            RemoveOrderedItemWithoutDoseFlag = facility.RemoveOrderedItemWithoutDose,
                            RemoteDispensingFlag = facility.RemoteDispensing,
                            DeliveryFlag = facility.Delivery,
                            MedQueueDurationAmount = facility.MedicationQueueDuration,
                            MedQueueBeforeDueDurationAmount = facility.MedicationQueueBeforeDueDuration,
                            MedQueueBeforeOrderStartDurationAmount = facility.MedicationQueueBeforeOrderStartDuration,
                            MedQueueAfterOrderExpiredDurationAmount = facility.MedicationQueueAfterOrderExpiredDuration,
                            MedQueueBeforeDueNowDurationAmount = facility.MedicationQueueBeforeDueNowDuration,
                            MedQueueAfterDueNowDurationAmount = facility.MedicationQueueAfterDueNowDuration,
                            PreadmissionLeadDurationAmount = facility.PreadmissionLeadDuration,
                            PreadmissionProlongedInactivityDurationAmount = facility.PreadmissionProlongedInactivityDuration,
                            AdmissionProlongedInactivityDurationAmount = facility.AdmissionProlongedInactivityDuration,
                            DischargeDelayDurationAmount = facility.DischargeDelayDuration,
                            DischargeDelayCancelFlag = facility.DischargeDelayCancel,
                            TransferDelayDurationAmount = facility.TransferDelayDuration,
                            LeaveOfAbsenceDelayDurationAmount = facility.LeaveOfAbsenceDelayDuration,
                            MyItemsNotificationDurationAmount = facility.MyItemsNotificationDuration,
                            DeliveryStatusDisplayDurationAmount = facility.DeliveryStatusDisplayDuration,
                            DiscontinueOrdersOnReadmitDurationAmount = facility.DiscontinueOrdersOnReadmitDuration,
                            PartialMultiComponentOrderRemoveFlag = facility.PartialMultiComponentOrderRemove,
                            RemovePatientIdBarcodeFlag = facility.RemovePatientIdBarcode,
                            IncreaseOrderedDoseFlag = facility.IncreaseOrderedDose,
                            RxCheckExpirationDurationAmount = facility.RxCheckExpirationDuration,
                            ExcludeRxCheckCubieOrSingleMultiDoseMiniFlag = facility.ExcludeRxCheckCubieOrSingleMultiDoseMini,
                            ExcludeRxCheckScanOnLoadRefillFlag = facility.ExcludeRxCheckScanOnLoadRefill,
                            RxCheckDelayDurationAmount = facility.RxCheckDelayDuration,
                            JitMgmtUrlId = facility.JitManagementUrlId,
                            PharmogisticsUrlId = facility.PharmogisticsUrlId,
                            AddFacilityNonMedItemOnlyFlag = facility.AddFacilityNonMedItemOnly,
                            DisablePendAssignOutdateTrackingFlag = facility.DisablePendAssignOutdateTracking,
                            TruncatedAllergyCommentText = facility.TruncatedAllergyComment,
                            TruncatedOrderCommentText = facility.TruncatedOrderComment,
                            DispensingDeviceUserScanFlag = facility.DispensingDeviceUserScan,
                            MatchByUserScanCodeFlag = facility.MatchByUserScanCode,
                            UserIdScanCodePrefixText = facility.UserIdScanCodePrefix,
                            UserIdScanCodeSuffixText = facility.UserIdScanCodeSuffix,
                            OrderIdScanCodePrefixText = facility.OrderIdScanCodePrefix,
                            OrderIdScanCodeSuffixText = facility.OrderIdScanCodeSuffix,
                            OrderIdScanCodePrefixLengthQuantity = facility.OrderIdScanCodePrefixLength,
                            OrderIdScanCodeSuffixLengthQuantity = facility.OrderIdScanCodeSuffixLength,
                            OrderIdScanCodeSuffixDelimiterValue = facility.OrderIdScanCodeSuffixDelimiterValue,
                            OrderIdScanCodeSuffixCustomExpressionText = facility.OrderIdScanCodeSuffixCustomExpressionText,
                            RepickWaitDurationAmount = facility.RepickWaitDuration,
                            CubieEsFlag = facility.CubieES,
                            HealthSightInventoryTasksFlag = facility.HealthSightInventoryTasks,
                            InsertUncheckedCubieFlag = facility.InsertUncheckedCubie,
                            CubieAnotherDestinationFlag = facility.CubieAnotherDestination,
                            CriteriaBasedFillFlag = facility.CriteriaBasedFill,
                            CardinalAssistFlag = facility.CardinalAssist,
                            LongTermCareFlag = facility.LongTermCare,
                            DispensingDeviceAllOrdersFlag = facility.DispensingDeviceAllOrders,
                            MixedDeviceTypeQueuingModeProfileFlag = facility.MixedDeviceTypeQueuingModeProfile,
                            RequestPharmacyOrderDoseFlag = facility.RequestPharmacyOrderDose,
                            RequestPharmacyOrderDoseDurationAmount = facility.RequestPharmacyOrderDoseDuration,
                            AttentionNoticeCriticalThresholdDurationAmount = facility.AttentionNoticeCriticalThresholdDuration,
                            StatusBoardDoseRequestDisplayDurationAmount = facility.StatusBoardDoseRequestDisplayDuration,
                            StatusBoardNewDoseRequestDisplayDurationAmount = facility.StatusBoardNewDoseRequestDisplayDuration,
                            UnknownAdmissionStatusRetentionDurationAmount = facility.UnknownAdmissionStatusRetentionDuration,
                            SequentialDrainModeInternalCode = facility.SequentialDrainModeInternalCode,
                            ExternalInventoryCountRequestFlag = facility.ExternalInventoryCountRequest,
                            ExternalRefillRequestFlag = facility.ExternalRefillRequest,
                            ExternalRefillRequestExpirationDurationAmount = facility.ExternalRefillRequestExpirationDuration,
                            UseEquivalenciesFlag = facility.UseEquivalencies,
                            FacilitySpecificEquivalenciesFlag = facility.AllowFacilitySpecificEquivalencies,
                            AutoApprovePisItemFlag = facility.AutoApprove,
                            ReverseDischargeDurationAmount = facility.ReverseDischargeDuration,
                            DisablePyxisBarcodeScanOnLoadRefillFlag = facility.DisablePyxisBarcodeScanOnLoadRefill,
                            ReverseDischargeFlag = facility.ReverseDischarge,
                            MedSearchStringFlag = facility.MedSearchString,
                            MedSearchStringLengthQuantity = facility.MedSearchStringLength,
                            MedSelectionOrderFlag = facility.MedSelectionOrder,
                            AllowPatientsToBeSearchedByLocationFlag = facility.AllowPatientsToBeSearchedByLocation,
                            DefaultSortPatientsByLocationFlag = facility.DefaultSortPatientsByLocation,
                            UserAuthenticationRequestDuration2Amount = facility.UserAuthenticationRequestDuration2,
                            DisplayPatientPreferredNameOnRemoveLabelFlag = facility.DisplayPatientPreferredNameOnRemoveLabel,

                            GCSMReceivePurchaseOrderRequiredFlag = facility.GCSMReceivePurchaseOrderRequired,
                            GCSMLogisticsOrderingInterfaceSupportFlag = facility.GCSMLogisticsOrderingInterfaceSupport,
                            GCSMLogisticsReceiveInterfaceSupportFlag = facility.GCSMLogisticsReceiveInterfaceSupport,
                            GCSMCardinalAssistInterfaceSupportFlag = facility.GCSMCardinalAssistInterfaceSupport,
                            GCSMDefaultOnReceiveDistributorKey = facility.GCSMDefaultOnReceiveDistributorKey,
                            GCSMAllDeviceEventsReviewSignaturesFlag = facility.GCSMAllDeviceEventsReviewSignatures,
                            GCSMPrintADMLabelFlag = facility.GCSMPrintADMLabel,
                            GCSMPrintLabelByDispenseOrderFlag = facility.GCSMPrintLabelByDispenseOrder,
                            GCSMShowInvoiceTypeFlag = facility.GCSMShowInvoiceType,
                            GCSMDispenseMultiMedSheetReconciliationFlag = facility.GCSMDispenseMultiMedSheetReconciliation,
                            GCSMAddItemFromCountDestructionBinFlag = facility.GCSMAddItemFromCountDestructionBinFlag,
                            GCSMAddItemFromEmptyDestructionBinFlag = facility.GCSMAddItemFromEmptyDestructionBinFlag,
                            GCSMChangeEmptyDestructionBinQuantityFlag = facility.GCSMChangeEmptyDestructionBinQuantityFlag,
                            GCSMDestructionBinEmptyWithDiscrepancyModeInternalCode = facility.GCSMDestructionBinEmptyWithDiscrepancyModeInternalCode,
                            LastModifiedBinaryValue = facility.LastModified
                        }); 

                    if (facility.PrimaryMedContact != null)
                    {
                        facility.PrimaryMedContact.FacilityKey = facilityKey;
                        facility.PrimaryMedContact.BusinessDomainInternalCode = BusinessDomainInternalCode.MED.ToInternalCode();
                        facility.PrimaryMedContact.Rank = FacilityContact.PrimaryContactRankValue;

                        // Insert primary med contact
                        InsertFacilityContact(context, facility.PrimaryMedContact);
                    }

                    if (facility.SecondaryMedContact != null)
                    {
                        facility.SecondaryMedContact.FacilityKey = facilityKey;
                        facility.SecondaryMedContact.BusinessDomainInternalCode = BusinessDomainInternalCode.MED.ToInternalCode();
                        facility.SecondaryMedContact.Rank = FacilityContact.SecondaryContactRankValue;

                        // Insert secondary med contact
                        InsertFacilityContact(context, facility.SecondaryMedContact);
                    }

                    if (facility.PrimarySupplyContact != null)
                    {
                        facility.PrimarySupplyContact.FacilityKey = facilityKey;
                        facility.PrimarySupplyContact.BusinessDomainInternalCode = BusinessDomainInternalCode.SUP.ToInternalCode();
                        facility.PrimarySupplyContact.Rank = FacilityContact.PrimaryContactRankValue;

                        // Insert primary supply contact
                        InsertFacilityContact(context, facility.PrimarySupplyContact);
                    }

                    if (facility.SecondarySupplyContact != null)
                    {
                        facility.SecondarySupplyContact.FacilityKey = facilityKey;
                        facility.SecondarySupplyContact.BusinessDomainInternalCode = BusinessDomainInternalCode.SUP.ToInternalCode();
                        facility.SecondarySupplyContact.Rank = FacilityContact.SecondaryContactRankValue;

                        // Insert secondary supply contact
                        InsertFacilityContact(context, facility.SecondarySupplyContact);
                    }

                    if (facility.NoticeTypes != null)
                    {
                        foreach (var facilityNoticeType in facility.NoticeTypes)
                        {
                            facilityNoticeType.FacilityKey = facilityKey;
                        }

                        InsertFacilityNoticeTypes(context, facility.NoticeTypes);
                    }

                    if (facility.NoRecentMessageReceivedConfigurations != null)
                    {
                        InsertNoRecentMessageReceivedConfigurations(
                            context,
                            facilityKey,
                            facility.NoRecentMessageReceivedConfigurations);
                    }

                    if (facility.SheetConfigurations != null)
                    {
                        foreach (var sheetConfiguration in facility.SheetConfigurations)
                        {
                            sheetConfiguration.FacilityKey = facilityKey;
                        }

                        InsertFacilitySheetConfigs(context, facility.SheetConfigurations);
                    }

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return facilityKey;
        }

        void IFacilityRepository.UpdateFacility(Context context, Facility facility)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(facility, "facility");

            try
            {
                LocationDAL.IFacilityRepository facilityRepository = new LocationDAL.FacilityRepository();

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                {
                    facilityRepository.UpdateFacility(context.ToActionContext(),
                        new LocationDAL.Models.Facility
                        {
                            FacilityKey = facility.Key,
                            FacilityCode = facility.Code,
                            FacilityName = facility.Name,
                            DescriptionText = facility.Description,
                            ActiveFlag = facility.IsActive,
                            PatientReconciliationDefaultFilterDurationAmount = facility.PatientReconciliationDefaultFilterDuration,
                            TemporaryPatientDurationAmount = facility.TemporaryPatientDuration,
                            ResendAfterTemporaryRemainsUnmergedDurationAmount = facility.ResendAfterTemporaryRemainsUnmergedDuration,
                            StreetAddressText = facility.StreetAddress,
                            CityName = facility.City,
                            SubdivisionName = facility.State,
                            PostalCode = facility.PostalCode,
                            CountryName = facility.Country,
                            NotesText = facility.Notes,
                            SiteId = facility.SiteId,
                            TimeZoneId = facility.TimeZoneId,
                            PharmacyInformationSystemKey = facility.PharmacyInformationSystemKey,
                            MedTemporaryUserAccessDurationAmount = facility.MedicationTemporaryUserAccessDuration,
                            UseVisitorRolesInMedTemporaryUserAccessFlag = facility.UseVisitorRolesInMedTemporaryUserAccess,
                            TooCloseRemoveDurationAmount = facility.TooCloseRemoveDuration,
                            PisMessageProcessingBehindDurationAmount = facility.PISMessageProcessingBehindDuration,
                            AdtMessageProcessingBehindDurationAmount = facility.ADTMessageProcessingBehindDuration,
                            NoAdtMessageReceivedDurationAmount = facility.NoADTMessageReceivedDuration,
                            NoPisMessageReceivedDurationAmount = facility.NoPISMessageReceivedDuration,
                            BioIdFailoverInternalCode = facility.BioIdFailoverInternalCode,
                            RFIDFailoverInternalCode = facility.RfidFailoverInternalCode,
                            UserScanModeInternalCode = facility.UserScanModeInternalCode,
                            FreeFormReasonFlag = facility.AllowFreeFormReason,
                            UseUnverifiedScanCodeFlag = facility.UseUnverifiedScanCode,
                            LateRemoveDurationAmount = facility.LateRemoveDuration,
                            MedExpirationCheckDurationAmount = facility.MedicationExpirationCheckDuration,
                            CriticalOverrideSchedulingFlag = facility.CriticalOverrideScheduling,
                            LeastRemovedDurationAmount = facility.LeastRemovedDuration,
                            SelectAllForRemoveListFlag = facility.SelectAllForRemoveList,
                            NoticeDiscrepancyDelayDurationAmount = facility.NoticeDiscrepancyDelayDuration,
                            NoticeDeviceNotCommunicatingDelayDurationAmount = facility.NoticeDeviceNotCommunicatingDelayDuration,
                            NoticeIncludeScheduledCriticalOverrideFlag = facility.NoticeIncludeScheduledCriticalOverride,
                            NoticeEtlDelayDurationAmount = facility.NoticeEtlDelayDuration,
                            DecreaseOrderedDoseFlag = facility.DecreaseOrderedDose,
                            OmnlToPrintEquivalenciesFlag = facility.OMNLToPrintEquivalencies,
                            TemporaryPatientNoIdText = facility.TemporaryPatientNoIdText,
                            DisplayEncounterIdFlag = facility.DisplayEncounterId,
                            DisplayAlternateEncounterIdFlag = facility.DisplayAlternateEncounterId,
                            DisplayAccountIdFlag = facility.DisplayAccountId,
                            DisplayPatientIdTypeKey = facility.DisplayPatientIdentificationTypeKey,
                            OptionalTemporaryEncounterIdFlag = facility.OptionalTemporaryEncounterId,
                            OptionalTemporaryAlternateEncounterIdFlag = facility.OptionalTemporaryAlternateEncounterId,
                            OptionalTemporaryAccountIdFlag = facility.OptionalTemporaryAccountId,
                            OptionalTemporaryPatientIdTypeKey = facility.OptionalTemporaryPatientIdentificationTypeKey,
                            RemoveMedLabelDisplayIdFlag = facility.RemoveMedicationLabelDisplayId,
                            RemoveMedLabelEncounterIdFlag = facility.RemoveMedicationLabelEncounterId,
                            RemoveMedLabelAlternateEncounterIdFlag = facility.RemoveMedicationLabelAlternateEncounterId,
                            RemoveMedLabelAccountIdFlag = facility.RemoveMedicationLabelAccountId,
                            RemoveMedLabelPatientIdTypeKey = facility.RemoveMedicationLabelPatientIdentificationTypeKey,
                            RemoveMedLabelProductIdFlag = facility.RemoveMedicationLabelProductId,
                            RemoveMedLabelItemIdBarcodeFlag = facility.RemoveMedLabelItemIdBarcode,
                            RemovePatientIdBarcodeFlag = facility.RemovePatientIdBarcode,
                            NotReturnableMedMessageText = facility.NotReturnableMedicationMessage,
                            AllowSingleMultiDoseDispenseCancelFlag = facility.AllowSingleMultiDoseDispenseCancel,
                            RemoveOrderedItemWithoutDoseFlag = facility.RemoveOrderedItemWithoutDose,
                            RemoteDispensingFlag = facility.RemoteDispensing,
                            DeliveryFlag = facility.Delivery,
                            MedQueueDurationAmount = facility.MedicationQueueDuration,
                            MedQueueBeforeDueDurationAmount = facility.MedicationQueueBeforeDueDuration,
                            MedQueueBeforeOrderStartDurationAmount = facility.MedicationQueueBeforeOrderStartDuration,
                            MedQueueAfterOrderExpiredDurationAmount = facility.MedicationQueueAfterOrderExpiredDuration,
                            MedQueueBeforeDueNowDurationAmount = facility.MedicationQueueBeforeDueNowDuration,
                            MedQueueAfterDueNowDurationAmount = facility.MedicationQueueAfterDueNowDuration,
                            PreadmissionLeadDurationAmount = facility.PreadmissionLeadDuration,
                            PreadmissionProlongedInactivityDurationAmount = facility.PreadmissionProlongedInactivityDuration,
                            AdmissionProlongedInactivityDurationAmount = facility.AdmissionProlongedInactivityDuration,
                            DischargeDelayDurationAmount = facility.DischargeDelayDuration,
                            DischargeDelayCancelFlag = facility.DischargeDelayCancel,
                            TransferDelayDurationAmount = facility.TransferDelayDuration,
                            LeaveOfAbsenceDelayDurationAmount = facility.LeaveOfAbsenceDelayDuration,
                            MyItemsNotificationDurationAmount = facility.MyItemsNotificationDuration,
                            DeliveryStatusDisplayDurationAmount = facility.DeliveryStatusDisplayDuration,
                            DiscontinueOrdersOnReadmitDurationAmount = facility.DiscontinueOrdersOnReadmitDuration,
                            PartialMultiComponentOrderRemoveFlag = facility.PartialMultiComponentOrderRemove,
                            IncreaseOrderedDoseFlag = facility.IncreaseOrderedDose,
                            RxCheckExpirationDurationAmount = facility.RxCheckExpirationDuration,
                            ExcludeRxCheckCubieOrSingleMultiDoseMiniFlag = facility.ExcludeRxCheckCubieOrSingleMultiDoseMini,
                            ExcludeRxCheckScanOnLoadRefillFlag = facility.ExcludeRxCheckScanOnLoadRefill,
                            RxCheckDelayDurationAmount = facility.RxCheckDelayDuration,
                            JitMgmtUrlId = facility.JitManagementUrlId,
                            PharmogisticsUrlId = facility.PharmogisticsUrlId,
                            AddFacilityNonMedItemOnlyFlag = facility.AddFacilityNonMedItemOnly,
                            DisablePendAssignOutdateTrackingFlag = facility.DisablePendAssignOutdateTracking,
                            TruncatedAllergyCommentText = facility.TruncatedAllergyComment,
                            TruncatedOrderCommentText = facility.TruncatedOrderComment,
                            DispensingDeviceUserScanFlag = facility.DispensingDeviceUserScan,
                            MatchByUserScanCodeFlag = facility.MatchByUserScanCode,
                            UserIdScanCodePrefixText = facility.UserIdScanCodePrefix,
                            UserIdScanCodeSuffixText = facility.UserIdScanCodeSuffix,
                            OrderIdScanCodePrefixText = facility.OrderIdScanCodePrefix,
                            OrderIdScanCodeSuffixText = facility.OrderIdScanCodeSuffix,
                            OrderIdScanCodePrefixLengthQuantity = facility.OrderIdScanCodePrefixLength,
                            OrderIdScanCodeSuffixLengthQuantity = facility.OrderIdScanCodeSuffixLength,
                            OrderIdScanCodeSuffixDelimiterValue = facility.OrderIdScanCodeSuffixDelimiterValue,
                            OrderIdScanCodeSuffixCustomExpressionText = facility.OrderIdScanCodeSuffixCustomExpressionText,
                            RepickWaitDurationAmount = facility.RepickWaitDuration,
                            CubieEsFlag = facility.CubieES,
                            HealthSightInventoryTasksFlag = facility.HealthSightInventoryTasks,
                            InsertUncheckedCubieFlag = facility.InsertUncheckedCubie,
                            CubieAnotherDestinationFlag = facility.CubieAnotherDestination,
                            CriteriaBasedFillFlag = facility.CriteriaBasedFill,
                            CardinalAssistFlag = facility.CardinalAssist,
                            LongTermCareFlag = facility.LongTermCare,
                            DispensingDeviceAllOrdersFlag = facility.DispensingDeviceAllOrders,
                            MixedDeviceTypeQueuingModeProfileFlag = facility.MixedDeviceTypeQueuingModeProfile,
                            RequestPharmacyOrderDoseFlag = facility.RequestPharmacyOrderDose,
                            RequestPharmacyOrderDoseDurationAmount = facility.RequestPharmacyOrderDoseDuration,
                            AttentionNoticeCriticalThresholdDurationAmount = facility.AttentionNoticeCriticalThresholdDuration,
                            StatusBoardDoseRequestDisplayDurationAmount = facility.StatusBoardDoseRequestDisplayDuration,
                            StatusBoardNewDoseRequestDisplayDurationAmount = facility.StatusBoardNewDoseRequestDisplayDuration,
                            UnknownAdmissionStatusRetentionDurationAmount = facility.UnknownAdmissionStatusRetentionDuration,
                            SequentialDrainModeInternalCode = facility.SequentialDrainModeInternalCode,
                            ExternalInventoryCountRequestFlag = facility.ExternalInventoryCountRequest,
                            ExternalRefillRequestFlag = facility.ExternalRefillRequest,
                            ExternalRefillRequestExpirationDurationAmount = facility.ExternalRefillRequestExpirationDuration,
                            UseEquivalenciesFlag = facility.UseEquivalencies,
                            FacilitySpecificEquivalenciesFlag = facility.AllowFacilitySpecificEquivalencies,
                            AutoApprovePisItemFlag = facility.AutoApprove,
                            ReverseDischargeDurationAmount = facility.ReverseDischargeDuration,
                            DisablePyxisBarcodeScanOnLoadRefillFlag = facility.DisablePyxisBarcodeScanOnLoadRefill,
                            ReverseDischargeFlag = facility.ReverseDischarge,
                            MedSearchStringFlag = facility.MedSearchString,
                            MedSearchStringLengthQuantity = facility.MedSearchStringLength,
                            MedSelectionOrderFlag = facility.MedSelectionOrder,
                            AllowPatientsToBeSearchedByLocationFlag = facility.AllowPatientsToBeSearchedByLocation,
                            DefaultSortPatientsByLocationFlag = facility.DefaultSortPatientsByLocation,
                            UserAuthenticationRequestDuration2Amount = facility.UserAuthenticationRequestDuration2,
                            DisplayPatientPreferredNameOnRemoveLabelFlag = facility.DisplayPatientPreferredNameOnRemoveLabel,
                            GCSMReceivePurchaseOrderRequiredFlag = facility.GCSMReceivePurchaseOrderRequired,
                            GCSMLogisticsOrderingInterfaceSupportFlag = facility.GCSMLogisticsOrderingInterfaceSupport,
                            GCSMLogisticsReceiveInterfaceSupportFlag = facility.GCSMLogisticsReceiveInterfaceSupport,
                            GCSMCardinalAssistInterfaceSupportFlag = facility.GCSMCardinalAssistInterfaceSupport,
                            GCSMDefaultOnReceiveDistributorKey = facility.GCSMDefaultOnReceiveDistributorKey,
                            GCSMAllDeviceEventsReviewSignaturesFlag = facility.GCSMAllDeviceEventsReviewSignatures,
                            GCSMPrintADMLabelFlag = facility.GCSMPrintADMLabel,
                            GCSMPrintLabelByDispenseOrderFlag = facility.GCSMPrintLabelByDispenseOrder,
                            GCSMShowInvoiceTypeFlag = facility.GCSMShowInvoiceType,
                            GCSMDispenseMultiMedSheetReconciliationFlag = facility.GCSMDispenseMultiMedSheetReconciliation,
                            GCSMAddItemFromCountDestructionBinFlag = facility.GCSMAddItemFromCountDestructionBinFlag,
                            GCSMAddItemFromEmptyDestructionBinFlag = facility.GCSMAddItemFromEmptyDestructionBinFlag,
                            GCSMChangeEmptyDestructionBinQuantityFlag = facility.GCSMChangeEmptyDestructionBinQuantityFlag,
                            GCSMDestructionBinEmptyWithDiscrepancyModeInternalCode = facility.GCSMDestructionBinEmptyWithDiscrepancyModeInternalCode,
                            LastModifiedBinaryValue = facility.LastModified
                        });

                    if (facility.PrimaryMedContact != null)
                    {
                        facility.PrimaryMedContact.FacilityKey = facility.Key;
                        facility.PrimaryMedContact.BusinessDomainInternalCode = BusinessDomainInternalCode.MED.ToInternalCode();
                        facility.PrimaryMedContact.Rank = FacilityContact.PrimaryContactRankValue;

                        // Update primary med contact
                        UpdateFacilityContact(context, facility.PrimaryMedContact);
                    }

                    if (facility.SecondaryMedContact != null)
                    {
                        facility.SecondaryMedContact.FacilityKey = facility.Key;
                        facility.SecondaryMedContact.BusinessDomainInternalCode = BusinessDomainInternalCode.MED.ToInternalCode();
                        facility.SecondaryMedContact.Rank = FacilityContact.SecondaryContactRankValue;

                        // Update secondary med contact
                        UpdateFacilityContact(context, facility.SecondaryMedContact);
                    }

                    if (facility.PrimarySupplyContact != null)
                    {
                        facility.PrimarySupplyContact.FacilityKey = facility.Key;
                        facility.PrimarySupplyContact.BusinessDomainInternalCode = BusinessDomainInternalCode.SUP.ToInternalCode();
                        facility.PrimarySupplyContact.Rank = FacilityContact.PrimaryContactRankValue;

                        // Update primary supply contact
                        UpdateFacilityContact(context, facility.PrimarySupplyContact);
                    }

                    if (facility.SecondarySupplyContact != null)
                    {
                        facility.SecondarySupplyContact.FacilityKey = facility.Key;
                        facility.SecondarySupplyContact.BusinessDomainInternalCode = BusinessDomainInternalCode.SUP.ToInternalCode();
                        facility.SecondarySupplyContact.Rank = FacilityContact.SecondaryContactRankValue;

                        // Update secondary supply contact
                        UpdateFacilityContact(context, facility.SecondarySupplyContact);
                    }

                    // Update facility notice types
                    UpdateFacilityNoticeTypes(context, facility.NoticeTypes ?? new FacilityNoticeType[0]);

                    // Update no recent message received configurations
                    UpdateNoRecentMessageReceivedConfigurations(
                        connectionScope,
                        context,
                        facility.Key,
                        facility.NoRecentMessageReceivedConfigurations ?? new NoRecentMessageReceivedConfiguration[0]);

                    // Update facility sheet configurations
                    UpdateFacilitySheetConfigs(context, facility.SheetConfigurations ?? new SheetConfiguration[0]);

                    // Commit transaction
                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #region Private Members

        private static void InsertFacilityContact(Context context, FacilityContact facilityContact)
        {
            Guard.ArgumentNotNull(context, "context");

            if (facilityContact == null)
                return;

            LocationDAL.IFacilityRepository facilityRepository = new LocationDAL.FacilityRepository();

            facilityRepository.InsertFacilityContact(context.ToActionContext(),
                new LocationDAL.Models.FacilityContact
                {
                    FacilityContactKey = facilityContact.Key,
                    FacilityKey = facilityContact.FacilityKey,
                    BusinessDomainInternalCode = facilityContact.BusinessDomainInternalCode,
                    RankValue = facilityContact.Rank,
                    CustomerContactName = facilityContact.Name,
                    CustomerContactDescriptionText = facilityContact.Description,
                    CustomerContactPhoneNumberText = facilityContact.PhoneNumber,
                    CustomerContactFaxNumberText = facilityContact.FaxNumber,
                    CustomerContactEmailAddressValue = facilityContact.EmailAddress,
                    CustomerContactPreferredMethodInternalCode = facilityContact.CustomerContactPreferredMethodInternalCode,
                    LastModifiedBinaryValue = facilityContact.LastModified
                });
        }

        private static void UpdateFacilityContact(Context context, FacilityContact facilityContact)
        {
            Guard.ArgumentNotNull(context, "context");

            if (facilityContact == null)
                return;

            if (!facilityContact.IsTransient())
            {
                LocationDAL.IFacilityRepository facilityRepository = new LocationDAL.FacilityRepository();

                facilityRepository.UpdateFacilityContact(context.ToActionContext(),
                    new LocationDAL.Models.FacilityContact
                    {
                        FacilityContactKey = facilityContact.Key,
                        FacilityKey = facilityContact.FacilityKey,
                        BusinessDomainInternalCode = facilityContact.BusinessDomainInternalCode,
                        RankValue = facilityContact.Rank,
                        CustomerContactName = facilityContact.Name,
                        CustomerContactDescriptionText = facilityContact.Description,
                        CustomerContactPhoneNumberText = facilityContact.PhoneNumber,
                        CustomerContactFaxNumberText = facilityContact.FaxNumber,
                        CustomerContactEmailAddressValue = facilityContact.EmailAddress,
                        CustomerContactPreferredMethodInternalCode =
                            facilityContact.CustomerContactPreferredMethodInternalCode,
                        LastModifiedBinaryValue = facilityContact.LastModified
                    });
            }
            else
            {
                InsertFacilityContact(context, facilityContact);
            }
        }

        private void InsertFacilityNoticeTypes(Context context, IEnumerable<FacilityNoticeType> noticeTypes)
        {
            Guard.ArgumentNotNull(context, "context");

            if (noticeTypes == null)
                return;

            NoticeDAL.INoticeTypeFacilityRepository noticeTypeFacilityRepository = new NoticeDAL.NoticeTypeFacilityRepository();

            foreach (var noticeType in noticeTypes)
            {
                noticeTypeFacilityRepository.InsertNoticeTypeFacility(context.ToActionContext(),
                    new NoticeDAL.Models.NoticeTypeFacility
                    {
                        NoticeTypeFacilityKey = noticeType.Key,
                        NoticeTypeInternalCode = noticeType.NoticeTypeInternalCode,
                        FacilityKey = noticeType.FacilityKey,
                        NoticeSeverityInternalCode = noticeType.NoticeSeverityInternalCode,
                        DisplayNoticeTypeFlag = noticeType.DisplayNoticeType,
                        LastModifiedBinaryValue = noticeType.LastModified
                    });
            }
        }

        private void UpdateFacilityNoticeTypes(Context context, IEnumerable<FacilityNoticeType> noticeTypes)
        {
            Guard.ArgumentNotNull(context, "context");

            if (noticeTypes == null)
                return;

            NoticeDAL.INoticeTypeFacilityRepository noticeTypeFacilityRepository = new NoticeDAL.NoticeTypeFacilityRepository();

            foreach (var noticeType in noticeTypes)
            {
                noticeTypeFacilityRepository.UpdateNoticeTypeFacility(context.ToActionContext(),
                    new NoticeDAL.Models.NoticeTypeFacility
                    {
                        NoticeTypeFacilityKey = noticeType.Key,
                        NoticeTypeInternalCode = noticeType.NoticeTypeInternalCode,
                        FacilityKey = noticeType.FacilityKey,
                        NoticeSeverityInternalCode = noticeType.NoticeSeverityInternalCode,
                        DisplayNoticeTypeFlag = noticeType.DisplayNoticeType,
                        LastModifiedBinaryValue = noticeType.LastModified
                    });
            }
        }

        private void InsertNoRecentMessageReceivedConfigurations(Context context, Guid facilityKey, IEnumerable<NoRecentMessageReceivedConfiguration> noRecentMessageReceivedConfigurations)
        {
            Guard.ArgumentNotNull(context, "context");

            if (noRecentMessageReceivedConfigurations == null)
                return;

            LocationDAL.IFacilityRepository facilityRepository = new LocationDAL.FacilityRepository();
            foreach (var noRecentMessageReceivedConfiguration in noRecentMessageReceivedConfigurations)
            {
                facilityRepository.InsertNoRecentMessageReceivedConfig(context.ToActionContext(),
                    new LocationDAL.Models.NoRecentMessageReceivedConfig
                    {
                        NoRecentMessageReceivedConfigKey = noRecentMessageReceivedConfiguration.Key,
                        FacilityKey = facilityKey,
                        NoRecentMessageReceivedTypeInternalCode = noRecentMessageReceivedConfiguration.NoRecentMessageReceivedTypeInternalCode,
                        ConfigName = noRecentMessageReceivedConfiguration.Name,
                        StartTimeOfDayValue = noRecentMessageReceivedConfiguration.StartTimeOfDay,
                        EndTimeOfDayValue = noRecentMessageReceivedConfiguration.EndTimeOfDay,
                        MondayFlag = noRecentMessageReceivedConfiguration.Monday,
                        TuesdayFlag = noRecentMessageReceivedConfiguration.Tuesday,
                        WednesdayFlag = noRecentMessageReceivedConfiguration.Wednesday,
                        ThursdayFlag = noRecentMessageReceivedConfiguration.Thursday,
                        FridayFlag = noRecentMessageReceivedConfiguration.Friday,
                        SaturdayFlag = noRecentMessageReceivedConfiguration.Saturday,
                        SundayFlag = noRecentMessageReceivedConfiguration.Sunday,
                        NoMessageReceivedDurationAmount = noRecentMessageReceivedConfiguration.NoMessageReceivedDuration,
                        LastModifiedBinaryValue = noRecentMessageReceivedConfiguration.LastModified
                    });
            }
        }

        private void UpdateNoRecentMessageReceivedConfigurations(IConnectionScope connectionScope, Context context, Guid facilityKey, IEnumerable<NoRecentMessageReceivedConfiguration> noRecentMessageReceivedConfigurations)
        {
            Guard.ArgumentNotNull(context, "context");
            LocationDAL.IFacilityRepository facilityRepository = new LocationDAL.FacilityRepository();

            SqlBuilder query = new SqlBuilder();
            query.SELECT("NoRecentMessageReceivedConfigKey")
                .FROM("Location.NoRecentMessageReceivedConfigSnapshot")
                .WHERE("EndUTCDateTime IS NULL")
                .WHERE("DeleteFlag = 0")
                .WHERE("FacilityKey = @FacilityKey");

            // Get the list of configurations associated with this facility.
            IReadOnlyCollection<Guid> currentConfigurations = connectionScope.Query(
                        query.ToString(),
                        new { FacilityKey = facilityKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Select(x => (Guid)x.NoRecentMessageReceivedConfigKey)
                        .ToList();

            // Find the configuration keys that were removed.
            IEnumerable<Guid> removedConfigurations =
                currentConfigurations.Except(noRecentMessageReceivedConfigurations.Select(nrmrc => nrmrc.Key));

            // Remove configurations that are no longer associated with this facility.
            foreach (Guid configurationKey in removedConfigurations)
            {
                facilityRepository.DeleteNoRecentMessageReceivedConfig(context.ToActionContext(), configurationKey);
            }

            // Find the configurations that were added and update existing ones.
            List<NoRecentMessageReceivedConfiguration> addedConfigurations = new List<NoRecentMessageReceivedConfiguration>();
            foreach (NoRecentMessageReceivedConfiguration configuration in noRecentMessageReceivedConfigurations)
            {
                if (configuration.IsTransient())
                {
                    addedConfigurations.Add(configuration);
                    continue;
                }

                facilityRepository.UpdateNoRecentMessageReceivedConfig(context.ToActionContext(),
                    new LocationDAL.Models.NoRecentMessageReceivedConfig
                    {
                        NoRecentMessageReceivedConfigKey = configuration.Key,
                        FacilityKey = facilityKey,
                        NoRecentMessageReceivedTypeInternalCode = configuration.NoRecentMessageReceivedTypeInternalCode,
                        ConfigName = configuration.Name,
                        StartTimeOfDayValue = configuration.StartTimeOfDay,
                        EndTimeOfDayValue = configuration.EndTimeOfDay,
                        MondayFlag = configuration.Monday,
                        TuesdayFlag = configuration.Tuesday,
                        WednesdayFlag = configuration.Wednesday,
                        ThursdayFlag = configuration.Thursday,
                        FridayFlag = configuration.Friday,
                        SaturdayFlag = configuration.Saturday,
                        SundayFlag = configuration.Sunday,
                        NoMessageReceivedDurationAmount = configuration.NoMessageReceivedDuration,
                        LastModifiedBinaryValue = configuration.LastModified
                    });
            }

            // Add the configurations.
            InsertNoRecentMessageReceivedConfigurations(
                context,
                facilityKey,
                addedConfigurations);
        }

        private void InsertFacilitySheetConfigs(Context context, IEnumerable<SheetConfiguration> sheetConfigs)
        {
            Guard.ArgumentNotNull(context, "context");

            if (sheetConfigs == null)
                return;

            SheetConfigDAL.ISheetConfigurationRepository sheetConfigFacilityRepository = new SheetConfigDAL.SheetConfigurationRepository();

            foreach (var sheetConfig in sheetConfigs)
            {
                var sheetConfigKey = sheetConfigFacilityRepository.InserSheetConfiguration(context.ToActionContext(),
                     new SheetConfigDAL.Models.SheetConfiguration
                     {
                         SheetConfigKey = sheetConfig.Key,
                         SheetStyleKey = sheetConfig.SheetStyleKey,
                         FacilityKey = sheetConfig.FacilityKey,
                         SheetTypeInternalCode = sheetConfig.SheetTypeInternalCode,
                         ReconcileFlag = sheetConfig.Reconcile,
                         DeliverySignatureReceiptPrintQuantity = sheetConfig.DeliverySignatureReceiptPrintQuantity,
                         PageBreakByMedClassFlag = sheetConfig.PageBreakByMedClass,
                         PageBreakByInjectableFlag = sheetConfig.PageBreakByInjectable,
                         InPageBreakByMedClassFlag = sheetConfig.InPageBreakByMedClass,
                         InPageBreakByInjectableFlag = sheetConfig.InPageBreakByInjectable,
                         BlankSpaceQuantity = sheetConfig.BlankSpaceQuantity,
                         EmptyPerSheetTransactionFlag = sheetConfig.EmptyPerSheetTransaction,
                         LastModifiedBinaryValue = sheetConfig.LastModified
                     });
                InsertFacilitySheetConfigMedClassGroups(context, sheetConfigKey, sheetConfig.MedClassGroups);
            }
        }

        private void UpdateFacilitySheetConfigs(Context context, IEnumerable<SheetConfiguration> sheetConfigs)
        {
            Guard.ArgumentNotNull(context, "context");

            if (sheetConfigs == null)
                return;

            SheetConfigDAL.ISheetConfigurationRepository sheetConfigFacilityRepository = new SheetConfigDAL.SheetConfigurationRepository();

            foreach (var sheetConfig in sheetConfigs)
            {
                var sheetConfiguration = new SheetConfigDAL.Models.SheetConfiguration
                {
                    SheetConfigKey = sheetConfig.Key,
                    SheetStyleKey = sheetConfig.SheetStyleKey,
                    FacilityKey = sheetConfig.FacilityKey,
                    SheetTypeInternalCode = sheetConfig.SheetTypeInternalCode,
                    ReconcileFlag = sheetConfig.Reconcile,
                    DeliverySignatureReceiptPrintQuantity = sheetConfig.DeliverySignatureReceiptPrintQuantity,
                    PageBreakByMedClassFlag = sheetConfig.PageBreakByMedClass,
                    PageBreakByInjectableFlag = sheetConfig.PageBreakByInjectable,
                    InPageBreakByMedClassFlag = sheetConfig.InPageBreakByMedClass,
                    InPageBreakByInjectableFlag = sheetConfig.InPageBreakByInjectable,
                    BlankSpaceQuantity = sheetConfig.BlankSpaceQuantity,
                    EmptyPerSheetTransactionFlag = sheetConfig.EmptyPerSheetTransaction,
                    LastModifiedBinaryValue = sheetConfig.LastModified
                };
                if (sheetConfig.Key != null && sheetConfig.LastModified != null)
                {
                    sheetConfigFacilityRepository.UpdateSheetConfiguration(context.ToActionContext(), sheetConfiguration);
                    var disassociateSheetConfigMedClassGroups = GetFacilitySheetConfigMedClassGroups(sheetConfig.Key);
                    DeleteFacilitySheetConfigMedClassGroup(context, disassociateSheetConfigMedClassGroups);
                    InsertFacilitySheetConfigMedClassGroups(context, sheetConfig.Key, sheetConfig.MedClassGroups);
                }
                else
                {
                    var sheetConfigKey = sheetConfigFacilityRepository.InserSheetConfiguration(context.ToActionContext(), sheetConfiguration);
                    InsertFacilitySheetConfigMedClassGroups(context, sheetConfigKey, sheetConfig.MedClassGroups);
                }
            }
        }

        #region Faciliy_SheetConfigMedClassGroup
        private IEnumerable<SheetConfigDAL.Models.SheetConfigurationMedClassGroup> GetFacilitySheetConfigMedClassGroups(Guid sheetConfig)
        {
            SheetConfigDAL.ISheetConfigurationMedClassGroupRepository sheetConfigFacilityRepository = new SheetConfigDAL.SheetConfigurationMedClassGroupRepository();
            return sheetConfigFacilityRepository.GetSheetConfigurationMedClassGroups(sheetConfig);
        }
        private void InsertFacilitySheetConfigMedClassGroups(Context context,Guid sheetConfigKey, IEnumerable<SheetConfigurationMedClassGroup> sheetConfigMedClassGroups)
        {
            Guard.ArgumentNotNull(context, "context");

            if (sheetConfigMedClassGroups == null)
                return;

            SheetConfigDAL.ISheetConfigurationMedClassGroupRepository sheetConfigMedClassGroupRepository = new SheetConfigDAL.SheetConfigurationMedClassGroupRepository();
            short rankValue = 0;
            foreach (var sheetConfigMedClassGroup in sheetConfigMedClassGroups)
            {
                sheetConfigMedClassGroupRepository.InserSheetConfigurationMedClassGroup(context.ToActionContext(),
                    new SheetConfigDAL.Models.SheetConfigurationMedClassGroup
                    {
                        SheetConfigMedClassGroupKey = sheetConfigMedClassGroup.Key,
                        SheetConfigKey = sheetConfigKey,
                        MedClassGroupKey = sheetConfigMedClassGroup.MedClassGroupKey,
                        RankValue = rankValue,
                        LastModifiedBinaryValue = sheetConfigMedClassGroup.LastModified
                    });
                rankValue++;
            }
        }

        private void DeleteFacilitySheetConfigMedClassGroup(Context context, IEnumerable<SheetConfigDAL.Models.SheetConfigurationMedClassGroup> sheetConfigMedClassGroups)
        {
            Guard.ArgumentNotNull(context, "context");

            if (sheetConfigMedClassGroups == null)
                return;

            foreach (var sheetConfigmedClassGroup in sheetConfigMedClassGroups)
            {
                SheetConfigDAL.ISheetConfigurationMedClassGroupRepository sheetConfigMedClassGroupRepository = new SheetConfigDAL.SheetConfigurationMedClassGroupRepository();
                sheetConfigMedClassGroupRepository.DeleteSheetConfigurationMedClassGroup(context.ToActionContext(), sheetConfigmedClassGroup.SheetConfigMedClassGroupKey);
            }
        }        

        #endregion

        #endregion
    }
}
