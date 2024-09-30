using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data.Models;
using Pyxis.Core.Data.InternalCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using CoreDALModel = Pyxis.Core.Data.Schema.Core.Models;
using StrgDAL = Pyxis.Core.Data.Schema.Strg;

namespace CareFusion.Dispensing.Data
{
    internal static class ModelExtensions
    {
        public static AuthUserAccount ToAuthUserContract(this CoreDALModel.UserAccount model)
        {
            return new AuthUserAccount
            {
                AccountExpirationUtcDate = model.AccountExpirationUtcDateTime,
                ActiveDirectoryDomainKey = model.ActiveDirectoryDomainKey,
                CardSerialId = model.CardSerialId,
                RFIDCardSerialID = model.RFIDCardSerialID,
                DirectoryChangePasswordUtcDateTime = model.DirectoryChangePasswordUtcDateTime,
                FirstName = model.FirstName,
                InitialsText = model.InitialsText,
                IsActive = model.ActiveFlag,
                IsCardPinExempt = model.CardPinExemptFlag,
                IsFingerprintExempt = model.FingerprintExemptFlag,
                IsLocked = model.LockedFlag,
                IsSuperUser = model.SuperUserFlag,
                IsSupportUser = model.SupportUserFlag,
                IsTemporary = model.TemporaryFlag,
                Key = model.UserAccountKey,
                LastName = model.LastName,
                LastPasswordExpirationNoticeUtcDateTime = model.LastPasswordExpirationNoticeUtcDateTime,
                LastSuccessfulPasswordAuthenticationUtcDateTime = model.LastSuccessfulPasswordAuthenticationUtcDateTime,
                MiddleName = model.MiddleName,
                ScanCode = model.ScanCodeValue,
                SnapshotKey = model.UserAccountSnapshotKey,
                UserId = model.UserId,
                UserTypeKey = model.UserTypeKey
            };
        }

        public static PasswordCredential ToContract(this PasswordCredentialResult model)
        {
            return new PasswordCredential
            {
                CreatedUtcDateTime = model.StartUTCDateTime,
                EncryptionAlgorithm = model.EncryptionAlgorithmInternalCode.FromInternalCode<EncryptionAlgorithmInternalCode>(),
                Key = model.PasswordCredentialKey,
                LastModified = model.LastModifiedBinaryValue,
                PasswordHash = model.PasswordHashValue,
                Salt = model.PasswordSaltValue,
                UserChangedOwnPasswordDate = model.UserChangedOwnPasswordLocalDateTime,
                UserChangedOwnPasswordUtcDate = model.UserChangedOwnPasswordUtcDateTime
            };
        }

        public static ActiveDirectoryDomain ToContract(this CoreDALModel.ActiveDirectoryDomain model)
        {
            return new ActiveDirectoryDomain(model.ActiveDirectoryDomainKey)
            {
                ControllerAddress = model.DomainControllerAddressValue,
                EncryptionAlgorithmInternalCode = model.EncryptionAlgorithmInternalCode,
                FullyQualifiedName = model.FullyQualifiedDomainName,
                HighestCommittedUsn = model.HighestCommittedUsnValue,
                InvocationId = model.InvocationId,
                IsActive = model.ActiveFlag,
                IsSupportDomain = model.SupportDomainFlag,
                LastModified = model.LastModifiedBinaryValue,
                LastPollStatusInternalCode = model.LastPollStatusInternalCode,
                LastSuccessfulPollDateTime = model.LastSuccessfulPollLocalDateTime,
                LastSuccessfulPollUtcDateTime = model.LastSuccessfulPollUtcDateTime,
                LDAPCertificateFileName = model.LDAPCertificateFileName,
                Name = model.ShortDomainName,
                PollingDuration = model.PollingDurationAmount,
                PortNumber = model.PortNumber,
                ScheduledPasswordSignInInterval = model.ScheduledPasswordSignInIntervalAmount,
                SecuredCommunication = model.SecuredCommunicationFlag,
                SystemAccountId = model.SystemAccountId,
                SystemAccountPasswordEncrypted = model.SystemAccountPasswordEncryptedValue,
                UserDirectoryTypeInternalCode = model.UserDirectoryTypeInternalCode,
                Workgroup = model.WorkgroupFlag
            };
        }

        public static UserRole ToContract(this CoreDALModel.UserRoleMember model, IEnumerable<CoreDALModel.UserRoleMemberArea> areas)
        {
            return new UserRole(model.UserRoleMemberKey)
            {
                Areas = areas?.Where(a => a.UserRoleMemberKey == model.UserRoleMemberKey).Select(a => a.AreaKey).ToArray(),
                MedicationTemporaryAccess = model.MedTemporaryAccessFlag,
                RoleKey = model.UserRoleKey
            };
        }

        public static UserAccount ToContract(this CoreDALModel.UserAccount model,
                                             CoreDALModel.ActiveDirectoryDomain domain,
                                             PasswordCredentialResult passwordCredential,
                                             IEnumerable<CoreDALModel.UserRoleMember> roles,
                                             IEnumerable<CoreDALModel.UserRoleMemberArea> areas,
                                             IEnumerable<Guid> facilities)
        {
            return new UserAccount(model.UserAccountKey, model.UserAccountSnapshotKey)
            {
                AccountExpirationDate = model.AccountExpirationLocalDateTime,
                AccountExpirationUtcDate = model.AccountExpirationUtcDateTime,
                ActiveDirectoryObjectGuid = model.ActiveDirectoryObjectGloballyUniqueId,
                ActiveDirectoryDomain = domain?.ToContract(),
                CardSerialId = model.CardSerialId,
                RFIDCardSerialID = model.RFIDCardSerialID,
                ContactInformation = model.ContactInformation,
                DirectoryChangePasswordDateTime = model.DirectoryChangePasswordLocalDateTime,
                DirectoryChangePasswordUtcDateTime = model.DirectoryChangePasswordUtcDateTime,
                EmailAddress = model.EmailAddressValue,
                FacilityKeys = facilities?.ToArray(),
                FirstName = model.FirstName,
                FullName = model.FullName,
                Initials = model.InitialsText,
                IsActive = model.ActiveFlag,
                IsCardPinExempt = model.CardPinExemptFlag,
                IsFingerprintExempt = model.FingerprintExemptFlag,
                IsLocked = model.LockedFlag,
                IsSuperUser = model.SuperUserFlag,
                IsSupportUser = model.SupportUserFlag,
                IsTemporary = model.TemporaryFlag,
                JobTitle = model.JobTitleText,
                LastAreaKey = model.LastAreaKey,
                LastModified = model.LastModifiedBinaryValue,
                LastName = model.LastName,
                LastPasswordExpirationNoticeUtcDateTime = model.LastPasswordExpirationNoticeUtcDateTime,
                LastPasswordExpirationNoticeDateTime = model.LastPasswordExpirationNoticeLocalDateTime,
                LastSuccessfulPasswordAuthenticationUtcDateTime = model.LastSuccessfulPasswordAuthenticationUtcDateTime,
                LastSuccessfulPasswordAuthenticationDateTime = model.LastSuccessfulPasswordAuthenticationLocalDateTime,
                MiddleName = model.MiddleName,
                PasswordCredential = passwordCredential?.ToContract(),
                UserRoles = roles?.Select(x => x?.ToContract(areas)).ToArray(),
                ScanCode = model.ScanCodeValue,
                Suffix = model.SuffixText,
                UserId = model.UserId,
                UserTypeKey = model.UserTypeKey
            };
        }

        public static CoreDALModel.UserAccount ToModel(this UserAccount userAccount)
        {
            return new CoreDALModel.UserAccount
            {
                AccountExpirationLocalDateTime = userAccount.AccountExpirationDate,
                AccountExpirationUtcDateTime = userAccount.AccountExpirationUtcDate,
                ActiveDirectoryDomainKey = userAccount.ActiveDirectoryDomain != null ? userAccount.ActiveDirectoryDomain.Key : default(Guid?),
                ActiveDirectoryObjectGloballyUniqueId = userAccount.ActiveDirectoryObjectGuid,
                ActiveFlag = userAccount.IsActive,
                CardPinExemptFlag = userAccount.IsCardPinExempt,
                CardSerialId = userAccount.CardSerialId,
                RFIDCardSerialID = userAccount.RFIDCardSerialID,
                ContactInformation = userAccount.ContactInformation,
                DirectoryChangePasswordLocalDateTime = userAccount.DirectoryChangePasswordDateTime,
                DirectoryChangePasswordUtcDateTime = userAccount.DirectoryChangePasswordUtcDateTime,
                EmailAddressValue = userAccount.EmailAddress,
                FingerprintExemptFlag = userAccount.IsFingerprintExempt,
                FirstName = userAccount.FirstName,
                FullName = userAccount.FullName,
                InitialsText = userAccount.Initials,
                JobTitleText = userAccount.JobTitle,
                LastModifiedBinaryValue = userAccount.LastModified,
                LastName = userAccount.LastName,
                LockedFlag = userAccount.IsLocked,
                MiddleName = userAccount.MiddleName,
                ScanCodeValue = userAccount.ScanCode,
                SuffixText = userAccount.Suffix,
                SuperUserFlag = userAccount.IsSuperUser,
                SupportUserFlag = userAccount.IsSupportUser,
                TemporaryFlag = userAccount.IsTemporary,
                UserAccountKey = userAccount.Key,
                UserAccountSnapshotKey = userAccount.SnapshotKey,
                UserId = userAccount.UserId,
                UserTypeKey = userAccount.UserTypeKey
            };
        }

        public static UserFingerprint ToContract(this CoreDALModel.UserFingerprint model)
        {
            return new UserFingerprint
            {
                Value1Length = model.Fingerprint1LengthQuantity,
                Value1 = model.Fingerprint1Value,
                Value2Length = model.Fingerprint2LengthQuantity,
                Value2 = model.Fingerprint2Value,
                Value3Length = model.Fingerprint3LengthQuantity,
                Value3 = model.Fingerprint3Value,
                LastModified = model.LastModifiedBinaryValue,
                UserAccountKey = model.UserAccountKey,
                Key = model.UserFingerprintKey
            };
        }

        public static SystemBusDevice ToContract(this SystemBusDeviceResult model)
        {
            return new SystemBusDevice(model.SystemBusDeviceKey, model.LastModifiedBinaryValue)
            {
                CommunicationCubeType = model.CommunicationCubeTypeInternalCode.FromNullableInternalCode<CommunicationCubeTypeInternalCode>(),
                Depth = model.DepthQuantity,
                DispensingDeviceKey = model.DispensingDeviceKey,
                Height = model.HeightQuantity,
                Offset = model.OffsetQuantity,
                ParentSystemBusKey = model.ControllingSystemBusDeviceKey,
                Position = model.PositionNumber,
                SerialNumber = model.DeviceNumber,
                SystemBusDeviceType = model.SystemBusDeviceTypeInternalCode.FromInternalCode<SystemBusDeviceTypeInternalCode>(),
                Width = model.WidthQuantity
            };
        }

        public static StorageSpace ToContract(this StorageSpaceResult model)
        {
            var space = new StorageSpace(model.StorageSpaceKey)
            {
                AbbreviatedName = model.StorageSpaceAbbreviatedName,
                AnchorStrorageSpaceKey = model.AnchorStorageSpaceKey,
                DefrostInHierarchy = model.DefrostInHierarchyFlag,
                DispensingDeviceKey = model.DispensingDeviceKey,
                FacilityKey = model.FacilityKey,
                FailureInHierarchy = model.FailureInHierarchyFlag,
                LastModified = model.LastModifiedBinaryValue,
                Level = model.LevelNumber,
                MiniDrawerTrayMode = model.MiniDrawerTrayModeInternalCode?.FromNullableInternalCode<MiniDrawerTrayModeInternalCode>(),
                MoreThanOneItem = model.MoreThanOneItemFlag,
                ParentMiniDrawerTrayModeInternalCode = model.ParentMiniDrawerTrayModeInternalCode?.FromNullableInternalCode<MiniDrawerTrayModeInternalCode>(),
                ParentPendingMiniDrawerTrayModeInternalCode = model.ParentPendingMiniDrawerTrayModeInternalCode?.FromNullableInternalCode<MiniDrawerTrayModeInternalCode>(),
                ParentStorageSpaceKey = model.ParentStorageSpaceKey,
                PendingMiniDrawerTrayMode = model.PendingMiniDrawerTrayModeInternalCode?.FromNullableInternalCode<MiniDrawerTrayModeInternalCode>(),
                PositionID = model.PositionID,
                ProductName = model.ProductName,
                RestrictedAccess = model.RestrictedAccessFlag,
                SecondSystemBusDeviceKey = model.SecondSystemBusDeviceKey,
                SerialID = model.SerialID,
                StackedSerialID = model.StackedSerialID,
                RegistrySerialID = model.RegistrySerialID,
                ShortName = model.StorageSpaceShortName,
                SnapshotKey = model.StorageSpaceSnapshotKey,
                SortValue = model.SortValue,
                StorageSpaceFormDescriptionText = model.StorageSpaceFormDescription,
                StorageSpaceFormInternalCode = model.StorageSpaceFormInternalCode?.FromNullableInternalCode<StorageSpaceFormInternalCode>(),
                StorageSpaceFormKey = model.StorageSpaceFormKey,
                StorageSpaceName = model.StorageSpaceName,
                StorageSpaceTypeInternalCode = model.StorageSpaceTypeInternalCode.FromInternalCode<StorageSpaceTypeInternalCode>(),
                SystemBusDeviceKey = model.SystemBusDeviceKey,
                UnavailableForInventory = model.UnavailableForInventoryFlag,
                InteriorCabinetLightIntensityModeInternalCode = model.InteriorCabinetLightIntensityModeInternalCode.FromNullableInternalCode<InteriorCabinetLightIntensityModeInternalCode>()
            };

            if (model.StorageSpaceStateKey.HasValue)
            {
                space.StorageSpaceState = new StorageSpaceState(model.StorageSpaceStateKey.Value)
                {
                    Closed = model.ClosedFlag,
                    Defrost = model.DefrostFlag,
                    Failed = model.FailedFlag,
                    OnBattery = model.OnBatteryFlag,
                    FailureRequiresMaintenance = model.FailureRequiresMaintenanceFlag,
                    StorageSpaceAbbreviatedName = model.StorageSpaceAbbreviatedName,
                    StorageSpaceShortName = model.StorageSpaceShortName,
                    StorageSpaceKey = model.StorageSpaceKey,
                    Locked = model.LockedFlag,
                    LastModified = model.StorageSpaceLastModified,
                    FailureReason = model.StorageSpaceFailureReasonInternalCode == null
                        ? null
                        : new StorageSpaceFailureReason
                        {
                            Description = model.StorageSpaceFailureReasonDescription,
                            InternalCode = model.StorageSpaceFailureReasonInternalCode.FromInternalCode<StorageSpaceFailureReasonInternalCode>()
                        }
                };
            }

            return space;
        }

        public static StorageSpaceState ToContract(this StorageSpaceStateResult model)
        {
            return new StorageSpaceState(model.StorageSpaceStateKey)
            {
                Closed = model.ClosedFlag,
                Defrost = model.DefrostFlag,
                Failed = model.FailedFlag,
                OnBattery = model.OnBatteryFlag,
                FailureRequiresMaintenance = model.FailureRequiresMaintenanceFlag,
                StorageSpaceAbbreviatedName = model.StorageSpaceAbbreviatedName,
                StorageSpaceShortName = model.StorageSpaceShortName,
                StorageSpaceKey = model.StorageSpaceKey,
                Locked = model.LockedFlag,
                LastModified = model.LastModifiedBinaryValue,
                FailureReason = model.StorageSpaceFailureReasonInternalCode == null
                    ? null
                    : new StorageSpaceFailureReason
                      {
                        Description = model.DescriptionText,
                        InternalCode = model.StorageSpaceFailureReasonInternalCode == null
                            ? StorageSpaceFailureReasonInternalCode.UnknownInternalCode
                            : model.StorageSpaceFailureReasonInternalCode.FromInternalCode<StorageSpaceFailureReasonInternalCode>(),
                        SortValue = model.SortValue
                      }
            };
        }

        public static StrgDAL.Models.DispensingDevice ToModel(this DispensingDevice contract)
        {
            return new StrgDAL.Models.DispensingDevice
            {
                AccessInventoryFeatureOffFlag = contract.IsAccessInventoryFeatureOff,
                AdmissionProlongedInactivityDurationAmount = contract.AdmissionProlongedInactivityDuration.GetValueOrDefault(),
                AfterMedDueNowDurationAmount = contract.AfterMedicationDueNowDuration,
                AnesthesiaSwitchUserDurationAmount = contract.AnesthesiaSwitchUserDuration.GetValueOrDefault(),
                AnesthesiaSwitchUserFlag = contract.AnesthesiaSwitchUser,
                AnesthesiaTimeOutDurationAmount = contract.AnesthesiaTimeOutDuration,
                AuthenticationMethodInternalCode = contract.AuthenticationMethod.ToInternalCode(),
                AutoCriticalOverrideDurationAmount = contract.AutoCriticalOverrideDuration.GetValueOrDefault(),
                AutoMedLabelFlag = contract.AutoMedLabel,
                BarcodeReceiverDevicePortNumber = contract.BarcodeReceiverDevicePortNumber,
                BeforeMedDueNowDurationAmount = contract.BeforeMedicationDueNowDuration,
                BioIdExemptInternalCode = contract.BioIdExempt.ToInternalCode(),
                BlindCountFlag = contract.IsBlindCount,
                CardExemptInternalCode = contract.CardExempt?.ToInternalCode(),
                ComputerName = contract.ComputerName,
                ControlledSubstanceLicenseKey = contract.ControlledSubstanceLicenseKey,
                CriticalLowNoticePrinterName = contract.CriticalLowNoticePrinterName,
                CriticalOverrideFlag = contract.CriticalOverride,
                DefaultGlobalPatientSearchFlag = contract.DefaultGlobalPatientSearch,
                DischargeDelayDurationAmount = contract.DischargeDelayDuration.GetValueOrDefault(),
                DispensingDeviceKey = contract.Key,
                DispensingDeviceName = contract.Name,
                DispensingDeviceTypeInternalCode = contract.DispensingDeviceType.ToInternalCode(),
                DisplayPatientPreferredNameFlag = contract.DisplayPatientPreferredNameFlag,
                EmptyReturnBinTimeOutDurationAmount = contract.EmptyReturnBinTimeOutDuration,
                ExternalSystemDeviceAdminUserPasswordValue = contract.ExternalSystemDeviceAdminUserPasswordValue,
                ExternalSystemDeviceCommandTimeoutDurationAmount = contract.ExternalSystemDeviceCommandTimeoutDurationAmount,
                OpenBinTimeoutDurationAmount = contract.OpenBinTimeoutDurationAmount.GetValueOrDefault(),
                ExternalSystemDeviceHostValue = contract.ExternalSystemDeviceHostValue,
                ExternalSystemDevicePortNumber = contract.ExternalSystemDevicePortNumber,
                FacilityKey = contract.FacilityKey,
                FutureTaskWarningDurationAmount = contract.FutureTaskWarningDurationAmount,
                GCSMBlindCountFlag = contract.GCSMBlindCountFlag,
                GCSMCompareReportStandardRangeInternalCode = contract.GCSMCompareReportStandardRange.ToInternalCode(),
                GCSMCompareReportViewFilterFocusedFlag = contract.GCSMCompareReportViewFilterFocusedFlag,
                GCSMDestructionBinTimeOutDurationAmount = contract.GCSMDestructionBinTimeOutDurationAmount,
                GCSMHideAreaFilterFlag = contract.GCSMHideAreaFilterFlag,
                GCSMHideZoneFilterFlag = contract.GCSMHideZoneFilterFlag,
                GCSMLabelPrinterName = contract.GCSMLabelPrinterName,
                GCSMOriginDispensingDeviceKey = contract.GCSMOriginDispensingDeviceKey,
                GCSMDestinationDispensingDeviceKey = contract.GCSMDestinationDispensingDeviceKey,
                GCSMPrintLabelOnAccessDestructionBinFlag = contract.GCSMPrintLabelOnAccessDestructionBinFlag,
                GCSMPrintLabelOnFillPrescriptionFlag = contract.GCSMPrintLabelOnFillPrescriptionFlag,
                GCSMPrintLabelOnNonStandardCompoundIngredientFlag = contract.GCSMPrintLabelOnNonStandardCompoundIngredientFlag,
                GCSMPrintLabelOnNonStandardCompoundMedFlag = contract.GCSMPrintLabelOnNonStandardCompoundMedFlag,
                GCSMPrintLabelOnOutdateFlag = contract.GCSMPrintLabelOnOutdateFlag,
                GCSMPrintLabelOnRecallFlag = contract.GCSMPrintLabelOnRecallFlag,
                GCSMPrintLabelOnReturnFlag = contract.GCSMPrintLabelOnReturnFlag,
                GCSMPrintLabelOnSellFlag = contract.GCSMPrintLabelOnSellFlag,
                GCSMPrintLabelOnStandardCompoundIngredientFlag = contract.GCSMPrintLabelOnStandardCompoundIngredientFlag,
                GCSMPrintLabelOnStandardCompoundMedFlag = contract.GCSMPrintLabelOnStandardCompoundMedFlag,
                GCSMPrintLabelOnWasteFlag = contract.GCSMPrintLabelOnWasteFlag,
                GCSMPrintPullListOnAutorestockFlag = contract.GCSMPrintPullListOnAutorestockFlag,
                GCSMPrintPullListOnDispenseToLocationFlag = contract.GCSMPrintPullListOnDispenseToLocationFlag,
                GCSMPrintPullListOnKitComponentFlag = contract.GCSMPrintPullListOnKitComponentFlag,
                GCSMPrintPullListOnKitFlag = contract.GCSMPrintPullListOnKitFlag,
                GCSMPrintPullListOnManageExcessStockFlag = contract.GCSMPrintPullListOnManageExcessStockFlag,
                GCSMPrintPullListOnNonStandardCompoundFlag = contract.GCSMPrintPullListOnNonStandardCompoundFlag,
                GCSMPrintPullListOnNonStandardCompoundIngredientFlag = contract.GCSMPrintPullListOnNonStandardCompoundIngredientFlag,
                GCSMPrintPullListOnStandardCompoundFlag = contract.GCSMPrintPullListOnStandardCompoundFlag,
                GCSMPrintPullListOnStandardCompoundIngredientFlag = contract.GCSMPrintPullListOnStandardCompoundIngredientFlag,
                GCSMPrintReceiptOnAccessDestructionBinFlag = contract.GCSMPrintReceiptOnAccessDestructionBinFlag,
                GCSMPrintReceiptOnDiscrepancyResolutionFlag = contract.GCSMPrintReceiptOnDiscrepancyResolutionFlag,
                GCSMPrintReceiptOnEmptyDestructionBinFlag = contract.GCSMPrintReceiptOnEmptyDestructionBinFlag,
                GCSMPrintReceiptOnFillPrescriptionFlag = contract.GCSMPrintReceiptOnFillPrescriptionFlag,
                GCSMPrintReceiptOnManageStockFlag = contract.GCSMPrintReceiptOnManageStockFlag,
                GCSMPrintReceiptOnNonStandardCompoundRecordFlag = contract.GCSMPrintReceiptOnNonStandardCompoundRecordFlag,
                GCSMPrintReceiptOnOutdateFlag = contract.GCSMPrintReceiptOnOutdateFlag,
                GCSMPrintReceiptOnPendingStandardCompoundRecordFlag = contract.GCSMPrintReceiptOnPendingStandardCompoundRecordFlag,
                GCSMPrintReceiptOnRecallFlag = contract.GCSMPrintReceiptOnRecallFlag,
                GCSMPrintReceiptOnReceiveFlag = contract.GCSMPrintReceiptOnReceiveFlag,
                GCSMPrintReceiptOnReturnFlag = contract.GCSMPrintReceiptOnReturnFlag,
                GCSMPrintReceiptOnReverseNonStandardCompoundFlag = contract.GCSMPrintReceiptOnReverseNonStandardCompoundFlag,
                GCSMPrintReceiptOnSellFlag = contract.GCSMPrintReceiptOnSellFlag,
                GCSMPrintReceiptOnStandardCompoundDispositionSummaryFlag = contract.GCSMPrintReceiptOnStandardCompoundDispositionSummaryFlag,
                GCSMPrintReceiptOnUnloadFlag = contract.GCSMPrintReceiptOnUnloadFlag,
                GCSMPrintReceiptOnWasteFlag = contract.GCSMPrintReceiptOnWasteFlag,
                GCSMScanOnCompoundingFlag = contract.GCSMScanOnCompoundingFlag,
                GCSMScanOnIssueFlag = contract.GCSMScanOnIssueFlag,
                GCSMScanOnPrescriptionFlag = contract.GCSMScanOnPrescriptionFlag,
                GCSMScanOnReceiveFlag = contract.GCSMScanOnReceiveFlag,
                GCSMScanOnRestockADMFlag = contract.GCSMScanOnRestockADMFlag,
                GCSMScanOnReturnFlag = contract.GCSMScanOnReturnFlag,
                GCSMScanOnSellFlag = contract.GCSMScanOnSellFlag,
                GCSMScanOnStockTransferFlag = contract.GCSMScanOnStockTransferFlag,
                GCSMSheetPrinterName = contract.GCSMSheetPrinterName,
                GCSMWitnessOnAccessToDestructionBinFlag = contract.GCSMWitnessOnAccessToDestructionBinFlag,
                GCSMWitnessOnAddToDestructionBinFlag = contract.GCSMWitnessOnAddToDestructionBinFlag,
                GCSMWitnessOnceInventoryFlag = contract.GCSMWitnessOnceInventoryFlag,
                GCSMWitnessOnCompoundingFlag = contract.GCSMWitnessOnCompoundingFlag,
                GCSMWitnessOnEmptyDestructionBinFlag = contract.GCSMWitnessOnEmptyDestructionBinFlag,
                GCSMWitnessOnInventoryFlag = contract.GCSMWitnessOnInventoryFlag,
                GCSMWitnessOnIssueFlag = contract.GCSMWitnessOnIssueFlag,
                GCSMWitnessOnOutdateFlag = contract.GCSMWitnessOnOutdateFlag,
                GCSMWitnessOnPrescriptionFlag = contract.GCSMWitnessOnPrescriptionFlag,
                GCSMWitnessOnRecallFlag = contract.GCSMWitnessOnRecallFlag,
                GCSMWitnessOnReceiveFlag = contract.GCSMWitnessOnReceiveFlag,
                GCSMWitnessOnRecoverFlag = contract.GCSMWitnessOnRecoverFlag,
                GCSMWitnessOnRestockADMFlag = contract.GCSMWitnessOnRestockADMFlag,
                GCSMWitnessOnReturnFlag = contract.GCSMWitnessOnReturnFlag,
                GCSMWitnessOnReverseCompoundingFlag = contract.GCSMWitnessOnReverseCompoundingFlag,
                GCSMWitnessOnSellFlag = contract.GCSMWitnessOnSellFlag,
                GCSMWitnessOnStockTransferFlag = contract.GCSMWitnessOnStockTransferFlag,
                GCSMWitnessOnUnloadFlag = contract.GCSMWitnessOnUnloadFlag,
                GCSMWitnessOnWasteFlag = contract.GCSMWitnessOnWasteFlag,
                GrabScanFlag = contract.IsGrabScan,
                IdentityServerClientID = contract.IdentityServerClientID,
                IdentityServerClientSecretValue = contract.IdentityServerClientSecretValue,
                InventoryDrawerMapTimeOutDurationAmount = contract.InventoryDrawerMapTimeOutDuration,                
                LastModifiedBinaryValue = contract.LastModified,
                LeaveOfAbsenceDelayDurationAmount = contract.LeaveOfAbsenceDelayDurationAmount.GetValueOrDefault(),
                ManualUpgradeFlag = contract.ManualUpgrade,
                MenuTimeOutDurationAmount = contract.MenuTimeOutDuration,
                OneTimePasswordSecretKeyValue = contract.OneTimePasswordSecretKeyValue,
                OneTimePasswordTimeoutDurationAmount = contract.OneTimePasswordTimeoutDurationAmount,
                OpenDrawerTimeOutDurationAmount = contract.OpenDrawerTimeOutDuration,
                OutdateTrackingFlag = contract.OutdateTracking,
                OutOfServiceFlag = contract.IsOutOfService,
                OverrideReasonRequiredFlag = contract.IsOverrideReasonRequired,
                PatientCaseTransactionHoldDurationAmount = contract.PatientCaseTransactionHoldDuration,
                PatientSpecificFunctionalityFlag = contract.IsPatientSpecificFunctionalityEnabled,
                PharmacyOrderDispenseQuantityFlag = contract.PharmacyOrderDispenseQuantity,
                PreadmissionLeadDurationAmount = contract.PreadmissionLeadDuration.GetValueOrDefault(),
                PreadmissionProlongedInactivityDurationAmount = contract.PreadmissionProlongedInactivityDuration.GetValueOrDefault(),
                PrintMedDestockFlag = contract.PrintMedicationDestock,
                PrintMedDisposeFlag = contract.PrintMedicationDispose,
                PrintMedEmptyReturnBinFlag = contract.PrintMedicationEmptyReturnBinTransaction,
                PrintMedLoadRefillFlag = contract.PrintMedicationLoadRefill,
                PrintMedOutdateFlag = contract.PrintMedicationOutdateTransaction,
                PrintMedRemoveFlag = contract.PrintMedicationRemoveTransaction,
                PrintMedReturnFlag = contract.PrintMedicationReturnTransaction,
                PrintMedRxCheckFlag = contract.PrintMedicationRxCheck,
                PrintMedUnloadFlag = contract.PrintMedicationUnloadTransaction,
                PrintPatientLabelRemoveFlag = contract.PrintPatientLabelRemove,
                ProfileModeFlag = contract.IsProfileMode,
                ReceiveBarcodeInventoryDecrementExternalFlag = contract.ReceiveBarcodeInventoryDecrementExternalFlag,
                RemoveAfterOrderExpiredDurationAmount = contract.RemoveAfterOrderExpiredDuration.GetValueOrDefault(),
                RemoveBeforeOrderStartDurationAmount = contract.RemoveBeforeOrderStartDuration.GetValueOrDefault(),
                ReplenishmentAreaKey = contract.ReplenishmentAreaKey,
                RequireExpirationDateOnRemoveFlag = contract.RequireExpirationDateOnRemove,
                RequireExpirationDateOnReturnFlag = contract.RequireExpirationDateOnReturn,
                RequireLotIdOnRemoveFlag = contract.RequireLotIdOnRemove,
                RequireLotIdOnReturnFlag = contract.RequireLotIdOnReturn,
                RequireSerialIdOnRemoveFlag = contract.RequireSerialIdOnRemove,
                RequireSerialIdOnReturnFlag = contract.RequireSerialIdOnReturn,
                ReturnToStockFlag = contract.ReturnToStock,
                ReverificationTimeOutDurationAmount = contract.ReverificationTimeOutDuration,
                RxCheckFlag = contract.RxCheck,
                ScanOnLoadRefillFlag = contract.ScanOnLoadRefill,
                ScanOnRemoveFlag = contract.ScanOnRemove,
                ScanOnReturnFlag = contract.ScanOnReturn,
                ServerAddressValue = contract.ServerAddress,
                ShowPreadmissionFlag = contract.ShowPreadmission,
                ShowRecurringAdmissionFlag = contract.ShowRecurringAdmission,
                StockOutNoticePrinterName = contract.StockOutNoticePrinterName,
                SyncAllowDownloadOnUploadFailureFlag = contract.SyncAllowDownloadOnUploadFailure,
                SyncServerKey = contract.SyncServerKey,
                SyncUploadAllowSkipFlag = contract.SyncUploadAllowSkip,
                SyncUploadMaximumRetryQuantity = contract.SyncUploadMaximumRetryQuantity,
                SyncUploadRetryIntervalAmount = contract.SyncUploadRetryInterval,
                TemporaryNonProfileModeFlag = contract.IsTemporarilyNonProfileMode,
                TooCloseWarningFlag = contract.IsTooCloseWarningRequired,
                TransferDelayDurationAmount = contract.TransferDelayDuration.GetValueOrDefault(),
                UpgradeTimeOfDayValue = contract.UpgradeTimeOfDay,
                UseEquivalenciesFlag = contract.UseEquivalencies,
                UserScanModeInternalCode = contract.UserScanMode.ToInternalCode(),
                VirtualStockLocationKey = contract.VirtualStockLocationKey,
                WasteModeInternalCode = contract.WasteMode.ToInternalCode(),
                WitnessOnCubieEjectFlag = contract.WitnessOnCubieEject,
                WitnessOnDestockFlag = contract.WitnessOnDestock,
                WitnessOnDispenseFlag = contract.WitnessOnDispense,
                WitnessOnDisposeFlag = contract.WitnessOnDispose,
                WitnessOnEmptyReturnBinFlag = contract.WitnessOnEmptyReturnBin,
                WitnessOnInventoryFlag = contract.WitnessOnInventory,
                WitnessOnLoadRefillFlag = contract.WitnessOnLoadRefill,
                WitnessOnOutdateFlag = contract.WitnessOnOutdate,
                WitnessOnOverrideFlag = contract.WitnessOnOverride,
                WitnessOnReturnFlag = contract.WitnessOnReturn,
                WitnessOnRxCheckFlag = contract.WitnessOnRxCheck,
                WitnessOnUnloadFlag = contract.WitnessOnUnload,
                ZoneKey = contract.ZoneKey
            };
        }

        public static StrgDAL.Models.StorageSpaceItem ToModel(this StorageSpaceItem contract)
        {
            return new StrgDAL.Models.StorageSpaceItem
            {
                CriticalLowQuantity = contract.CriticalLowQuantity,
                DispenseFractionFlag = contract.DispenseFraction,
                DispensingDeviceEjectWithInventoryFlag = contract.DispensingDeviceEjectWithInventory,
                FromExternalSystemFlag = contract.FromExternalSystem,
                IssueUOMKey = contract.IssueUnitOfMeasureKey,
                ItemKey = contract.ItemKey,
                LastDispenseLocalDateTime = contract.LastDispenseDateTime,
                LastDispenseUtcDateTime = contract.LastDispenseUtcDateTime,
                LastInventoryLocalDateTime = contract.LastInventoryDateTime,
                LastInventoryUtcDateTime = contract.LastInventoryUtcDateTime,
                LastKnownLocalDateTime = contract.LastKnownDateTime,
                LastKnownUtcDateTime = contract.LastKnownUtcDateTime,
                LastLoadRefillActorKey = contract.LastLoadRefillActorKey,
                LastLoadRefillLocalDateTime = contract.LastLoadRefillDateTime,
                LastLoadRefillSuccessfulScanFlag = contract.LastLoadRefillSuccessfulScan,
                LastModifiedBinaryValue = contract.LastModified,                 
                LastLoadRefillUtcDateTime = contract.LastLoadRefillUtcDateTime,
                LastRxCheckLocalDateTime = contract.LastRxCheckDateTime,
                LastRxCheckUtcDateTime = contract.LastRxCheckUTCDateTime,
                LoadLocalDateTime = contract.LoadDateTime,
                LoadUtcDateTime = contract.LoadUtcDateTime,
                OutdateTrackingFlag = contract.OutdateTracking,
                ParQuantity = contract.ParQuantity,
                PendedAtServerFlag = contract.PendedAtServer,
                PhysicalMaximumQuantity = contract.PhysicalMaximumQuantity,
                RefillPointQuantity = contract.RefillPointQuantity.ToNullableNumeric14_4(),
                StandardStockWithinDispensingDeviceFlag = contract.IsStandardStockWithinDispensingDevice,
                StorageSpaceItemKey = contract.Key,
                StorageSpaceItemStatusInternalCode = contract.StorageSpaceItemStatus.ToInternalCode(),
                StorageSpaceKey = contract.StorageSpaceKey,
                SystemBusDeviceKey = contract.SystemBusDeviceKey
            };
        }
    }
}
