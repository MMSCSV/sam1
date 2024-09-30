namespace CareFusion.Dispensing
{
    public static class ValidationConstants
    {
        public const string EmailAddressPattern = @"\w+('*?)([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
        public const string MultipleEmailAddressPattern = @"((\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)*([,])*)*";

        #region Common Constants

        public const int DisplayCodeUpperBound10 = 10;
        public const int DescriptionUpperBound50 = 50;

        #endregion

        #region ActiveDirectoryDomain

        public const int ActiveDirectoryDomainScheduledPasswordSignInIntervalDefaultValue = 14;
        public const int ActiveDirectoryDomainScheduledPasswordSignInIntervalLowerBound = 7;
        public const int ActiveDirectoryDomainScheduledPasswordSignInIntervalUpperBound = 60;

        #endregion

        #region User

        public const int UserIdUpperBound = 200;
        public const int UserLastNameUpperBound = 50;
        public const int UserFirstNameUpperBound = 50;
        public const int UserMiddleNameUpperBound = 50;
        public const int UserInitialsUpperBound = 10;
        public const int UserEmailAddressUpperBound = 256;
        public const int UserPasswordUpperBound = 127;
        public const int UserSuffixUpperBound = 20;
        public const int UserJobTitleUpperBound = 64;
        public const string UserIdRestrictedCharacters = @"(^[^@\\]+$)";

        public const int RoleNameUpperBound = 50;
        public const int RoleDescriptionUpperBound = 50;

        #endregion

        #region Location

        public const int FacilityContactNameUpperBound = 50;
        public const int FacilityContactDescriptionUpperBound = 50;
        public const int FacilityContactPhoneNumberUpperBound = 50;
        public const int FacilityContactFaxNumberUpperBound = 50;
        public const int FacilityContactEmailAddressUpperBound = 256;

        public const int FacilityNameUpperBound = 50;
        public const int FacilityNameLegacyUpperBound = 50;
        public const int FacilityDescriptionUpperBound = 100;
        public const int FacilitySiteIdUpperBound = 50;
        public const int FacilityStreetAddressUpperBound = 120;
        public const int FacilityCityUpperBound = 50;
        public const int FacilityStateUpperBound = 50;
        public const int FacilityPostalCodeUpperBound = 20;
        public const int FacilityCountryUpperBound = 50;
        public const int FacilityNotesUpperBound = 4000;
        public const int FacilityNoticeDiscrepancyDelayLowerBound = 0;
        public const int FacilityNoticeDiscrepancyDelayUpperBound = 48;
        public const int FacilityNoticeDeviceNotCommunicatingDelayDefaultValue = 5;
        public const int FacilityNoticeDeviceNotCommunicatingDelayLowerBound = 5;
        public const int FacilityNoticeDeviceNotCommunicatingDelayUpperBound = 60;
        public const int FacilityCodeUpperBound = 20;
        public const int FacilityTimeZoneIdUpperBound = 64;
        public const short FacilityTemporaryPatientDurationLowerBound = 1;
        public const short FacilityTemporaryPatientDurationUpperBound = 168;
        public const short FacilityTemporaryPatientDurationDefaultValue = 2;
        public const short FacilityMedicationTemporaryUserAccessDurationDefaultValue = 14;
        public const short FacilityMedicationTemporaryUserAccessDurationLowerBound = 1;
        public const short FacilityMedicationTemporaryUserAccessDurationUpperBound = 72;
        public const short FacilityTooCloseRemoveDurationLowerBound = 1;
        public const short FacilityTooCloseRemoveDurationUpperBound = 2880;
        public const short FacilityTooCloseRemoveDurationDefaultValue = 150;
        public const short FacilityLateRemoveDurationLowerBound = 0;
        public const short FacilityLateRemoveDurationUpperBound = 1440;
        public const short FacilityLateRemoveDurationDefaultValue = 60;
        public const short FacilityMedicationExpirationCheckDurationLowerBound = 0;
        public const short FacilityMedicationExpirationCheckDurationUpperBound = 180;
        public const short FacilityMedicationExpirationCheckDurationDefaultValue = 3;
        public const short FacilityNotReturnableItemMessageUpperBound = 150;
        public const short FacilityExternalRefillRequestDurationLowerBound = 1;
        public const short FacilityExternalRefillRequestDurationUpperBound = 96;
        public const short FacilityExternalRefillRequestDurationDefaultValue = 24;
        public const short FacilityReverseDischargeDurationDefaultValue = 2;
        public const short FacilityReverseDischargeDurationLowerBound = 0;
        public const short FacilityReverseDischargeDurationUpperBound = 336;
        public const byte MedSearchStringLengthDefaultValue = 5;
        public const bool MedSearchStringDefaultValue = true;
        public const short FacilityMedicationQueueDurationLowerBound = 15;
        public const short FacilityMedicationQueueDurationUpperBound = 1440;
        public const short FacilityMedicationQueueDurationDefaultValue = 120;
        public const short FacilityMedicationQueueBeforeDueDurationLowerBound = 1;
        public const short FacilityMedicationQueueBeforeDueDurationUpperBound = 1440;
        public const short FacilityMedicationQueueBeforeDueDurationDefaultValue = 60;
        public const short FacilityMedicationQueueBeforeOrderStartDurationLowerBound = 0;
        public const short FacilityMedicationQueueBeforeOrderStartDurationUpperBound = 7200;
        public const short FacilityMedicationQueueBeforeOrderStartDurationDefaultValue = 120;
        public const short FacilityMedicationQueueAfterOrderExpiredDurationLowerBound = 0;
        public const short FacilityMedicationQueueAfterOrderExpiredDurationUpperBound = 7200;
        public const short FacilityMedicationQueueAfterOrderExpiredDurationDefaultValue = 120;
        public const short FacilityMedicationQueueBeforeDueNowDurationLowerBound = 10;
        public const short FacilityMedicationQueueBeforeDueNowDurationUpperBound = 7200;
        public const short FacilityMedicationQueueBeforeDueNowDurationDefaultValue = 240;
        public const short FacilityMedicationQueueAfterDueNowDurationLowerBound = 10;
        public const short FacilityMedicationQueueAfterDueNowDurationUpperBound = 7200;
        public const short FacilityMedicationQueueAfterDueNowDurationDefaultValue = 120;
        public const short FacilityPreadmissionLeadDurationLowerBound = 0;
        public const short FacilityPreadmissionLeadDurationUpperBound = 168;
        public const short FacilityPreadmissionLeadDurationDefaultValue = 72;
        public const short FacilityPreadmissionProlongedInactivityDurationLowerBound = 1;
        public const short FacilityPreadmissionProlongedInactivityDurationUpperBound = 336;
        public const short FacilityPreadmissionProlongedInactivityDurationDefaultValue = 72;
        public const short FacilityAdmissionProlongedInactivityDurationLowerBound = 1;
        public const short FacilityAdmissionProlongedInactivityDurationUpperBound = 365;
        public const short FacilityAdmissionProlongedInactivityDurationDefaultValue = 60;
        public const short FacilityDischargeDelayDurationLowerBound = 0;
        public const short FacilityDischargeDelayDurationUpperBound = 168;
        public const short FacilityDischargeDelayDurationDefaultValue = 2;
        public const short FacilityTransferDelayDurationLowerBound = 0;
        public const short FacilityTransferDelayDurationUpperBound = 168;
        public const short FacilityTransferDelayDurationDefaultValue = 2;
        public const short FacilityResendAfterTemporaryRemainsUnmergedDurationLowerBound = 1;
        public const short FacilityResendAfterTemporaryRemainsUnmergedDurationUpperBound = 168;
        public const short FacilityResendAfterTemporaryRemainsUnmergedDurationDefaultValue = 168;
        public const short FacilityDeliveryStatusDisplayDurationLowerBound = 1;
        public const short FacilityDeliveryStatusDisplayDurationUpperBound = 12;
        public const short FacilityDeliveryStatusDisplayDurationDefaultValue = 2;
        public const short FacilityRxCheckDelayDurationLowerBound = 0;
        public const short FacilityRxCheckDelayDurationUpperBound = 1440;
        public const short FacilityRxCheckExpirationDurationLowerBound = 24;
        public const short FacilityRxCheckExpirationDurationUpperBound = 168;
        public const short FacilityReportETLFailureDelayPeriodLowerBound = 15;
        public const short FacilityReportETLFailureDelayPeriodUpperBound = 1440;
        public const short FacilityReportETLFailureDelayPeriodDefaultValue = 60;
        public const short FacilityDiscontinueOrdersOnReadmitDaysUpperBound = 14;
        public const short FacilityLeaveOfAbsenceDelayDurationAmountLowerBound = 0;
        public const short FacilityLeaveOfAbsenceDelayDurationAmountUpperBound = 168;
        public const short FacilityLeaveOfAbsenceDelayDurationAmountDefaultValue = 2;

        //stored in minutes
        public const short FacilityRepickWaitDurationDefaultValue = 240; // 4 hrs
        public const short FacilityRepickWaitDurationMinValue = 30;
        public const short FacilityRepickWaitDurationMaxValue = 1440;
        public const short FacilityRepickWaitDurationMaxHours = 24;

        //stored in minutes
        public const short FacilityMyItemsNotificationDurationDefaultValue = 30;
        public const short FacilityMyItemsNotificationDurationMinValue = 0;
        public const short FacilityMyItemsNotificationDurationMaxValue = 1440;
        public const short FacilityMyItemsNotificationDurationMaxHours = 24;

        //scan code
        public const short FacilityOrderIdScanCodeSuffixLengthUpperBound = 50;
        public const short FacilityOrderIdScanCodeSuffixLengthLowerBound = 1;
        public const short FacilityOrderIdScanCodePrefixLengthUpperBound = 50;
        public const short FacilityOrderIdScanCodePrefixLengthLowerBound = 1;
        public const short FacilityOrderIdScanCodeSuffixDelimiterValueUpperBound = 10;
        public const short FacilityOrderIdScanCodeSuffixCustomExpressionUpperBound = 150;
        public const short FacilityScanCodePrefixLowerBound = 0;
        public const short FacilityScanCodeSuffixLowerBound = 0;
        public const short FacilityScanCodePrefixUpperBound = 20;
        public const short FacilityScanCodeSuffixUpperBound = 20;

        public const bool FacilityCriticalOverrideSchedulingDefaultValue = false;
        public const bool FacilitySelectAllForRemoveListDefaultValue = true;
        public const bool FacilityAllowFreeFormReasonDefaultValue = true;
        public const bool FacilityIsActiveDefaultValue = false;
        public const bool RemovePatientIdBarcode = true;
        public const bool RemoveMedLabelItemIdBarcode = true;
        public const bool GCSMDispenseMultiMedSheetReconciliation = true;

        public const short FacilityUserAuthenticationRequestDuration2AmountLowerBound = 10;
        public const short FacilityUserAuthenticationRequestDuration2AmountUpperBound = 180;
        public const short FacilityUserAuthenticationRequestDuration2AmountDefaultValue = 10;

        /*Note RDS: Used for validation on facility dropdownlists universally throughout the Web UI*/
        public const string FacilityKeyDefault = "00000000-0000-0000-0000-000000000000";

        // Spec 483794: Regex pattern to disallow \/:*"?<>|
        public const string FacilityCodePattern = "^[^\\/\\:*\\\"\\?\\<\\>|\\\\]+$";
        #endregion

        #region Legacy

        public const int ConsoleIdUpperBound = 50;
        public const int ConsoleAliasIdUpperBound = 50;
        public const int ConsoleNameUpperBound = 50;
        public const int ConsoleVersionUpperBound = 50;
        public const int ConsoleRealTimeZoneIdUpperBound = 64;
        public const int ConsoleConfiguredTimeZoneIdUpperBound = 64;

        #endregion

        #region DispensingSystem

        public const int DispensingSystemIdMaxLength = 50;
        public const int DispensingSystemCustomerIdMaxLength = 50;
        public const int DispensingSystemCustomerNameMaxLength = 100;
        public const int DispensingSystemCustomerStreetAddressMaxLength = 120;
        public const int DispensingSystemCustomerCityMaxLength = 50;
        public const int DispensingSystemCustomerStateMaxLength = 50;
        public const int DispensingSystemCustomerPostalCodeMaxLength = 20;
        public const int DispensingSystemCustomerCountryMaxLength = 50;
        public const int DispensingSystemCustomerNotesMaxLength = 4000;
        public const int DispensingSystemServiceCenterPhoneNumberMaxLength = 50;
        public const int DispensingSystemServerStreetAddressMaxLength = 120;
        public const int DispensingSystemServerCityMaxLength = 50;
        public const int DispensingSystemServerStateMaxLength = 50;
        public const int DispensingSystemServerPostalCodeMaxLength = 20;
        public const int DispensingSystemServerCountryMaxLength = 50;
        public const int DispensingSystemServerLocationIdMaxLength = 50;
        public const int DispensingSystemServerNotesMaxLength = 4000;
        public const int DispensingSystemWarningBannerHeaderMaxLength = 150;
        public const int DispensingSystemWarningBannerTitleMaxLength = 100;
        public const int DispensingSystemWarningBannerDescriptionMaxLength = 3000;
        public const int SensitiveDataBannerMaxLength = 60;
        public const int DispensingSystemOrderIDPrefixMaxLength = 10;
        public const int DispensingSystemCardOnlineCertificateStatusUrlMaxLength = 200;

        public const byte DispensingSystemMinimumPasswordLengthDefaultValue = 6;
        public const byte DispensingSystemMinimumPasswordLengthLowerBound = 1;
        public const byte DispensingSystemMinimumPasswordLengthUpperBound = 16;

        public const short DispensingSystemPasswordExpirationDefaultValue = 90;
        public const short DispensingSystemPasswordExpirationLowerBound = 30;
        public const short DispensingSystemPasswordExpirationUpperBound = 365;

        public const short DispensingSystemMinimumPasswordAgeDefaultValue = 1;
        public const short DispensingSystemMinimumPasswordAgeLowerBound = 0;
        public const short DispensingSystemMinimumPasswordAgeUpperBound = 15;

        public const int DispensingSystemFailedAuthenticationLockoutIntervalDefaultValue = 30;
        public const int DispensingSystemFailedAuthenticationLockoutIntervalLowerBound = 1;
        public const int DispensingSystemFailedAuthenticationLockoutIntervalUpperBound = 9999;

        public const int DispensingSystemSessionInactivityTimeoutDefaultValue = 15;
        public const int DispensingSystemSessionInactivityTimeoutLowerBound = 2;
        public const int DispensingSystemSessionInactivityTimeoutUpperBound = 60;

        public const short DispensingSystemPasswordHistoryDefaultValue = 8;
        public const short DispensingSystemPasswordHistoryLowerBound = 1;
        public const short DispensingSystemPasswordHistoryUpperBound = 16;

        public const byte DispensingSystemFailedAuthenticationAttemptsAllowedDefaultValue = 3;
        public const byte DispensingSystemFailedAuthenticationAttemptsAllowedLowerBound = 1;
        public const byte DispensingSystemFailedAuthenticationAttemptsAllowedUpperBound = 20;

        public const short DispensingSystemTemporaryPasswordDurationDefaultValue = 24;
        public const short DispensingSystemTemporaryPasswordDurationLowerBound = 1;
        public const short DispensingSystemTemporaryPasswordDurationUpperBound = 168;

        public const int DispensingSystemPasswordMinimumDigitQuantityLowerBound = 0;
        public const int DispensingSystemPasswordMinimumDigitQuantityUpperBound = 16;

        public const int DispensingSystemPasswordMinimumLowercaseQuantityLowerBound = 0;
        public const int DispensingSystemPasswordMinimumLowercaseQuantityUpperBound = 16;

        public const int DispensingSystemPasswordMinimumUppercaseQuantityLowerBound = 0;
        public const int DispensingSystemPasswordMinimumUppercaseQuantityUpperBound = 16;

        public const int DispensingSystemPasswordMinimumSpecialCharacterQuantityLowerBound = 0;
        public const int DispensingSystemPasswordMinimumSpecialCharacterQuantityUpperBound = 16;

        public const bool DispensingSystemAllowLocalUserCreationDefaultValue = true;

        public const int DispensingSystemStandardRetentionLowerBound = 14;
        public const int DispensingSystemStandardRetentionUpperBound = 31;

        public const int DispensingSystemGCSMRetentionLowerBound = 731;
        public const int DispensingSystemGCSMRetentionUpperBound = 1100;

        public const int PurgeableRetentionDurationLowerBound = 31;
        public const int PurgeableRetentionDurationUpperBound = 365;

        public const int LockedNoAuthenticationDurationLowerBound = 1;
        public const int LockedNoAuthenticationDurationUpperBound = 365;

        public const int DispensingSystemPatientRetentionLowerBound = 14;
        public const int DispensingSystemPatientRetentionUpperBound = 1000;

        public const int DispensingSystemSyncRetentionLowerBound = 3;
        public const int DispensingSystemSyncRetentionUpperBound = 31;

        public const int DispensingSystemInboundMessageNonErrorRetentionLowerBound = 7;
        public const int DispensingSystemInboundMessageNonErrorRetentionUpperBound = 31;

        public const int DispensingSystemInboundMessageErrorRetentionLowerBound = 7;
        public const int DispensingSystemInboundMessageErrorRetentionUpperBound = 99;

        public const int DispensingSystemOutboundMessageRetentionLowerBound = 7;
        public const int DispensingSystemOutboundMessageRetentionUpperBound = 183;

        public const int DispensingSystemPassordExpirationNoticeDefaultValue = 15;
        public const int DispensingSystemPassordExpirationNoticeLowerBound = 0;
        public const int DispensingSystemPassordExpirationNoticeUpperBound = 30;

        public const bool DispensingSystemDefaultDeployedSystem = false;

        public const string DispensingSystemDefaultDeliveryTransactionPrefix = "(ES)";

        public const string DispensingSystemDefaultManualPharmacyOrderIdPrefix = "ES";

        public const bool DispensingSystemSupportUserCardPINExemptFlagDefaultValue = true;

        #endregion

        #region DispensingSystemContact

        public const int DispensingSystemContactNameMaxLength = 50;
        public const int DispensingSystemContactDescriptionMaxLength = 50;
        public const int DispensingSystemContactPhoneNumberMaxLength = 50;
        public const int DispensingSystemContactFaxNumberMaxLength = 50;
        public const int DispensingSystemContactEmailAddressMaxLength = 256;

        #endregion

        #region Encounter

        public const int EncounterIdUpperBound = 50;
        public const int EncounterIdCheckValueUpperBound = 2;
        public const int AlternateEncounterIdUpperBound = 50;
        public const int AlternateEncounterIdCheckValueUpperBound = 2;
        public const int PreAdmitIdUpperBound = 50;
        public const int PreAdmitIdCheckValueUpperBound = 2;
        public const int AccountIdUpperBound = 50;
        public const int AccountIdCheckValueUpperBound = 2;
        #endregion

        #region EncounterPatientLocation

        public const int AssignedBedIdUpperBound = 50;
        public const int TemporaryBedIdUpperBound = 50;

        #endregion

        #region ItemScanCode

        public const int ItemScanCodeUpperBound = 100;

        #endregion

        #region Item

        public const int ItemAltItemIdUpperBound = 100;
        public const int ItemIdUpperBound = 100;
        public const int ItemDisplayNameUpperBound = 500;
        public const int ItemCustomFieldUpperBound = 100;
        public const byte CriticalLowPercentage_ToInitiateBulletin_MinimumValue = 0;
        public const byte CriticalLowPercentage_ToInitiateBulletin_MaximumValue = 100;

        #endregion

        #region MedItem

        public const int MedItemGenericNameUpperBound = 150;
        public const int MedItemBrandNameUpperBound = 100;
        public const int MedItemSearchGenericNameUpperBound = 150;
        public const int MedItemSearchBrandNameUpperBound = 100;
        public const int MedItemPureGenericNameUpperBound = 150;
        public const int MedItemStrengthAmountLowerBound = 1;
        public const int MedItemConcentrationVolumeAmountLowerBound = 1;
        public const int MedItemTotalVolumeAmountLowerBound = 1;
        public const int MedItemTotalDeviceParDurationLowerBound = 1;
        public const int MedItemTotalDeviceParDurationUpperBound = 9999;

        #endregion

        #region FacilityItem

        public const short FacilityItemTooCloseRemoveDurationLowerBound = 1;
        public const short FacilityItemTooCloseRemoveDurationUpperBound = 2880;
        public const decimal FacilityItemUnitsOfIssuePerUnitOfRefillLowerBound = 1;
        public const byte FacilityItemCriticalLowPercentageLowerBound = 0;
        public const byte FacilityItemCriticalLowPercentageUpperBound = 100;

        #endregion

        #region FacilityKit

        public const int FacilityKitNameUpperBound = 50;
        public const int FacilityKitDescriptionUpperBound = 100;
        public const short FacilityKitItemQuantityLowerBound = 1;
        public const short FacilityKitItemQuantityUpperBound = 99;
        public const int FacilityKitItemCountLowerBound = 1;
        public const int FacilityKitItemCountUpperBound = 100;

        #endregion

        #region Compound Template Item

        public const int CompoundItemNameUpperBound = 100;
        public const int CompoundItemDescriptionUpperBound = 100;
        public const int CompoundItemMedIdUpperBound = 100;
        public const short CompoundItemIngredientsQuantityLowerBound = 1;
        public const short CompoundItemIngredientsUpperBound = 99;
        public const int NonStndCompoundNameUpperBound = 100;
        public const int NonStndCompoundIdUpperBound = 100;

        #endregion

        #region SecurityGroup

        public const int SecurityGroupDisplayCodeUpperBound = 20;
        public const int SecurityGroupDescriptionUpperBound = 100;

        #endregion

        #region OverrideGroup

        public const int OverrideGroupDisplayCodeUpperBound = 20;
        public const int OverrideGroupDescriptionUpperBound = 100;

        #endregion

        #region MedicationClass

        public const int MedicationClassCodeUpperBound = 20;
        public const int MedicationClassDescriptionUpperBound = 100;

        #endregion

        #region TherapeuticClass

        public const int TherapeuticClassCodeUpperBound = 20;
        public const int TherapeuticClassDescriptionUpperBound = 100;

        #endregion

        #region DosageForm

        public const int DosageFormCodeUpperBound = 20;
        public const int DosageFormDescriptionUpperBound = 100;

        #endregion

        #region EquivalencyDosageFormGroup

        public const int EquivalencyDosageFormGroupDisplayCodeUpperBound = 20;
        public const int EquivalencyDosageFormGroupDescriptionUpperBound = 100;

        #endregion

        #region HazardousWasteClass

        public const int HazardousWasteClassDisplayCodeUpperBound = 20;
        public const int HazardousWasteClassDescriptionUpperBound = 100;
        public const int HazardousWasteClassDisposalInstructionsUpperBound = 250;

        #endregion

        #region ExternalSystem

        public const int ExternalSystemInboundDelayDurationLowerBound = 1;
        public const int ExternalSystemInboundDelayDurationUpperBound = 60;
        public const int ExternalSystemInboundDownDurationLowerBound = 1;
        public const int ExternalSystemInboundDownDurationUpperBound = 60;

        #endregion

        #region DiscrepancyResolution

        public const int DiscrepancyResolutionUpperBound = 250;

        #endregion

        #region OverrideReason

        public const int OverrideReasonDescriptionUpperBound = 250;

        #endregion

        #region TooCloseReason

        public const int TooCloseReasonDescriptionUpperBound = 250;

        #endregion

        #region ClinicalDataCategory

        public const int ClinicalDataCategoryDescriptionUpperBound = 100;

        #endregion

        #region UnitOfMeasure

        public const decimal UnitOfMeasureConversionLowerBound = 0.00000000000001m;
        public const decimal UnitOfMeasureConversionUpperBound = 9999999999.99999999999999m;
        public const int UnitOfMeasureDisplayCodeUpperBound = 50;
        public const int UnitOfMeasureDescriptionUpperBound = 250;

        #endregion

        #region ExternalUnitOfMeasure

        public const int ExternalUnitOfMeasureCodeUpperBound = 75;

        #endregion

        #region DispensingDevice

        private const int NinetyNineMinutesAndFiftyNineSeconds = (99 * 60) + 59;

        public const int DispensingDeviceNameUpperBound = 50;
        public const int DispensingDeviceComputerNameUpperBound = 50;
        public const int DispensingDeviceDescriptionUpperBound = 100;
        public const short DispensingDeviceAutoCriticalOverrideDurationLowerBound = 1;
        public const short DispensingDeviceUpgradeTimeOfDayLowerBound = 0;
        public const short DispensingDeviceUpgradeTimeOfDayUpperBound = 1439;

        // time out stores in seconds
        public const short DispensingDeviceMenuTimeoutDefaultValue = 90;
        public const short DispensingDeviceMenuTimeoutMinValue = 30;
        public const short DispensingDeviceMenuTimeoutMaxValue = NinetyNineMinutesAndFiftyNineSeconds;

        public const short DispensingDeviceOpenDrawerTimeoutDefaultValue = 90;
        public const short DispensingDeviceOpenDrawerTimeoutMinValue = 30;
        public const short DispensingDeviceOpenDrawerTimeoutMaxValue = NinetyNineMinutesAndFiftyNineSeconds;

        public const short DispensingDeviceEmptyReturnBinTimeoutDefaultValue = 300;
        public const short DispensingDeviceEmptyReturnBinTimeoutMinValue = 60;
        public const short DispensingDeviceEmptyReturnBinTimeoutMaxValue = NinetyNineMinutesAndFiftyNineSeconds;

        // the following stores in minutes
        public const short DispensingDeviceExtendedTimeOutMinValue = 0;
        public const short DispensingDeviceExtendedTimeOutMaxValue = 32767;

        public const short DispensingDeviceRemoveBeforeOrderStartDefaultValue = 60;
        public const short DispensingDeviceRemoveBeforeOrderStartMinValue = 0;
        public const short DispensingDeviceRemoveBeforeOrderStartMaxValue = 7200;

        public const short DispensingDeviceRemoveAfterOrderStartDefaultValue = 120;
        public const short DispensingDeviceRemoveAfterOrderStartMinValue = 0;
        public const short DispensingDeviceRemoveAfterOrderStartMaxValue = 7200;

        public const short DispensingDeviceAutoEnableDownTimeDefaultValue = 120;
        public const short DispensingDeviceAutoEnableDownTimeMinValue = 1;
        public const short DispensingDeviceAutoEnableDownTimeMaxValue = 32767;

        public const short DispensingDeviceAssociatedAreaMaxValue = 100;
        public const short DispensingDeviceAssociatedRoomsMaxValue = 60;
        public const short DispensingDeviceAssociatedCDCListMaxValue = 200;

        public const short MedDueNowAfterDefaultValue = 60;
        public const short MedDueNowAfterMinValue = 10;
        public const short MedDueNowAfterMaxValue = 7200;

        public const short MedDueNowBeforeDefaultValue = 240;
        public const short MedDueNowBeforeMinValue = 10;
        public const short MedDueNowBeforeMaxValue = 7200;

        public const short DispensingDeviceDefaultUpgradeTimeDefaultValue = 180;

        public const short DispensingDeviceSyncUploadRetryIntervalDefaultValue = 60;
        public const short DispensingDeviceSyncUploadRetryIntervalMinValue = 10;
        public const short DispensingDeviceSyncUploadRetryIntervalMaxValue = 3600;

        public const short DispensingDeviceSyncUploadMaximumRetryQuantityDefaultValue = 75;
        public const short DispensingDeviceSyncUploadMaximumRetryQuantityMinValue = 1;
        public const short DispensingDeviceSyncUploadMaximumRetryQuantityMaxValue = 1000;

        public const short DispensingDeviceFutureTaskWarningDefaultValue = 4;
        public const short DispensingDeviceFutureTaskWarningMinValue = 1;
        public const short DispensingDeviceFutureTaskWarningMaxValue = 99;

        public const short DispensingDeviceOtpTimeoutDurationDefaultValue = 60;
        public const short LeaveOfAbsenceDelayDurationAmountDefaultValue = 2;
        public const short DispensingDeviceDestructionBinTimeoutDefaultValue = 300;
        public const short DispensingDeviceDestructionBinTimeoutMinutesMinValue = 1;
        public const short DispensingDeviceDestructionBinTimeoutMinutesMaxValue = 99;
        public const short DispensingDeviceDestructionBinTimeoutMaxValue = 5999;
        public const short DispensingDeviceDestructionBinTimeoutMinValue = 60;
        #endregion

        #region CriticalOverridePeriod

        public const int CriticalOverridePeriodNameUpperBound = 50;
        public const short CriticalOverridePeriodStartTimeOfDayLowerBound = 0;
        public const short CriticalOverridePeriodStartTimeOfDayUpperBound = 1440;
        public const short CriticalOverridePeriodEndTimeOfDayLowerBound = 0;
        public const short CriticalOverridePeriodEndTimeOfDayUpperBound = 1440;

        #endregion

        #region ConfigurationGroup

        public const int ConfigurationGroupNameUpperBound = 100;

        #endregion

        #region Zone

        public const int ZoneNameUpperBound = 100;

        #endregion

        #region Area

        public const int AreaNameUpperBound = 100;
        public const int AreaDescriptionUpperBound = 100;

        #endregion

        #region Unit

        public const int UnitNameUpperBound = 50;
        public const int UnitDescriptionUpperBound = 100;
        public const short UnitPreadmissionLeadDurationMinValue = 0;
        public const short UnitPreadmissionLeadDurationMaxValue = 168;
        public const short UnitDischargeDelayDurationMinValue = 0;
        public const short UnitDischargeDelayDurationMaxValue = 168;
        public const short UnitTransferDelayDurationMinValue = 0;
        public const short UnitTransferDelayDurationMaxValue = 168;
        public const short UnitPreadmissionProlongedInactivityDurationMinValue = 1;
        public const short UnitPreadmissionProlongedInactivityDurationMaxValue = 336;
        public const short UnitAdmissionProlongedInactivityDurationMinValue = 1;
        public const short UnitAdmissionProlongedInactivityDurationMaxValue = 365;

        // spec 375644
        public const short UnitAutoDischargeDurationMinValue = 0;
        public const short UnitAutoDischargeDurationMaxValue = 365;
        // spec 375646
        public const short UnitAltAutoDischargeDurationMinValue = 0;
        public const short UnitAltAutoDischargeDurationMaxValue = 365;

        public const int UnitMinimumNumberOfAreasAssociated = 1;

        #endregion

        #region PickArea

        public const int PickAreaNameUpperBound = 100;

        #endregion

        #region Patient

        public const int PatientFirstNameUpperBound = 50;
        public const int PatientMiddleNameUpperBound = 50;
        public const int PatientLastNameUpperBound = 50;
        public const int PatientPreferredNameUpperBound = 50;
        public const int PatientPrefixUpperBound = 20;
        public const int PatientSuffixUpperBound = 50;

        #endregion

        #region PatientSilo

        public const int PatientSiloNameUpperBound = 50;

        #endregion

        #region PatientIdentification

        public const int PatientIdentificationUpperBound = 50;
        public const int PatientIdentificationCheckValueUpperBound = 2;

        #endregion

        #region PatientIdentificationType

        public const int PatientIdentificationTypeDisplayCodeUpperBound = 10;
        public const int PatientIdentificationTypeDescriptionUpperBound = 50;

        #endregion

        #region ExternalPatientIdentificationType

        public const int ExternalPatientIdentificationTypeCodeUpperBound = 20;

        #endregion

        #region Gender

        public const int GenderDisplayCodeUpperBound = 10;
        public const int GenderDescriptionUpperBound = 50;

        #endregion

        #region ExternalGender

        public const int ExternalGenderCodeUpperBound = 20;

        #endregion

        #region RepeatPattern

        public const int RepeatPatternCodeUpperBound = 75;
        public const int RepeatPatternDescriptionUpperBound = 250;
        public const short RepeatPatternExplicitTimeOfDayLowerBound = 0;
        public const short RepeatPatternExplicitTimeOfDayUpperBound = 1439; //23:59

        #endregion

        #region TimingRecordPriority

        public const int TimingRecordPriorityCodeUpperBound = 20;
        public const int TimingRecordPriorityDescriptionUpperBound = 250;

        #endregion

        #region AllergyType

        public const int AllergyTypeCodeUpperBound = 20;
        public const int AllergyTypeDescriptionUpperBound = 100;

        #endregion

        #region UserDirectories

        public const int SynchronizationIntervalLowerBound = 1;
        public const int SynchronizationIntervalUpperBound = 500;

        #endregion

        #region OutOfTemperatureRangeResolution

        public const int OutOfTemperatureRangeResolutionDescriptionUpperBound = 250;

        #endregion

        #region RxCheckReason

        public const int RxCheckReasonUpperBound = 250;

        #endregion

        #region Device

        public const int ExternalSystemDevicePortNumber = 22;
        public const int ExternalSystemDeviceCommandTimeoutDurationAmount = 10;
        public const int DeviceAreasListUpperBound = 100;
        public const int BarcodeReceiverDevicePortNumber = 48427;
        public const bool ReceiveBarcodeInventoryDecrementExternalFlag = false;
        public const bool IsBlindCount = true;
        public const bool GCSMBlindCount = true;

        #endregion

        #region FacilitySheetConfiguration

        public const bool GCSMReconcileSingleMedSheet = true;
        public const bool GCSMReconcileMultiMedSheet = true;
        public const bool GCSMReconcileDripSheet = true;
        public const bool GCSMReconcileProceduralSheet = true;
        public const bool GCSMReconcileDeliverySignatureSheet = true;
        public const short GCSMDeliverySignatureReceiptsToPrint = 1;
        public const bool GCSMShowInvoiceType = true;

        #endregion

        #region CSLicense

        public const int CSLicenseNameUpperBound = 100;

        public const int LicenseBeforeExpireNotifyDays = 30;

        #endregion

        #region RestockGroupName

        public const int RestockGroupNameUpperBound = 100;
        public const int RestockGroupDescriptionUpperBound = 100;

        #endregion

        #region DestructionBin

        public const bool GCSMAddItemFromCountDestructionBinFlag = true;
        public const bool GCSMAddItemFromEmptyDestructionBinFlag = false;
        public const bool GCSMChangeEmptyDestructionBinQuantityFlag = false;

        #endregion
    }
}
