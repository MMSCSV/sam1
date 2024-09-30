using System;
using System.Data;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Models;
using Dapper;
using Pyxis.Core.Data;
using CoreDAL = Pyxis.Core.Data.Schema.Core;

namespace CareFusion.Dispensing.Data.Repositories
{
    #region IDispensingSystemRepository Interface

    public interface IDispensingSystemRepository
    {
        /// <summary>
        /// Retreive the dispensing system record
        /// </summary>
        /// <returns></returns>
        DispensingSystem GetDispensingSystem();

        /// <summary>
        /// Retreive the dispensing system template record
        /// </summary>
        /// <returns></returns>
        DispensingSystemTemplate GetDispensingSystemTemplate();

        /// <summary>
        /// Inserts a dispensing system record
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dispensingSystem"></param>
        Guid InsertDispensingSystem(Context context, DispensingSystem dispensingSystem);

        /// <summary>
        /// Updates the dispensing system record
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dispensingSystem"></param>
        void UpdateDispensingSystem(Context context, DispensingSystem dispensingSystem);
    }

    #endregion

    public class DispensingSystemRepository : IDispensingSystemRepository
    {
        static DispensingSystemRepository()
        {
            // Dapper custom mappings
            SqlMapper.SetTypeMap(
                typeof(DispensingSystem),
                new ColumnAttributeTypeMapper<DispensingSystem>());
            SqlMapper.SetTypeMap(
                typeof(DispensingSystemContact),
                new ColumnAttributeTypeMapper<DispensingSystemContact>());
            SqlMapper.SetTypeMap(
                typeof(DispensingSystemTemplate),
                new ColumnAttributeTypeMapper<DispensingSystemTemplate>());
        }


        DispensingSystem IDispensingSystemRepository.GetDispensingSystem()
        {
            DispensingSystem dispensingSystem = null;

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    using (var multi = connectionScope.QueryMultiple(
                        "Core.bsp_GetDispensingSystem",
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure))
                    {
                        dispensingSystem = multi.Read<DispensingSystem>()
                            .FirstOrDefault();

                        if (dispensingSystem != null)
                        {
                            var contactResults = multi.Read<DispensingSystemContact>()
                                .ToArray();

                            dispensingSystem.PrimaryCustomerContact = contactResults.FirstOrDefault(dsc => 
                                dsc.DispensingSystemKey == dispensingSystem.Key &&
                                dsc.Rank == DispensingSystemContact.PrimaryContactRankValue);

                            dispensingSystem.SecondaryCustomerContact = contactResults.FirstOrDefault(dsc =>
                                dsc.DispensingSystemKey == dispensingSystem.Key &&
                                dsc.Rank == DispensingSystemContact.SecondaryContactRankValue);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return dispensingSystem;
        }

        DispensingSystemTemplate IDispensingSystemRepository.GetDispensingSystemTemplate()
        {
            DispensingSystemTemplate dispensingSystemTemplate = null;

            try
            {
                SqlBuilder query = new SqlBuilder();
                query.SELECT("TOP 1 *")
                     .FROM("Core.DispensingSystemTemplate");

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    dispensingSystemTemplate = connectionScope.Query<DispensingSystemTemplate>(
                        query.ToString(),
                        null,
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return dispensingSystemTemplate;
        }

        Guid IDispensingSystemRepository.InsertDispensingSystem(Context context, DispensingSystem dispensingSystem)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(dispensingSystem, "dispensingSystem");

            Guid? dispensingSystemKey = null;

            try
            {
                CoreDAL.IDispensingSystemRepository dispensingSystemRepository = new CoreDAL.DispensingSystemRepository();

                using (ConnectionScopeFactory.Create())
                using (var tx = TransactionScopeFactory.Create())
                {
                    dispensingSystemKey = dispensingSystemRepository.InsertDispensingSystem(context.ToActionContext(),
                        new CoreDAL.Models.DispensingSystem
                        {
                            DispensingSystemKey = dispensingSystem.Key,
                            ServerId = dispensingSystem.ServerId,
                            DispensingSystemId = dispensingSystem.DispensingSystemId,
                            CustomerName = dispensingSystem.CustomerName,
                            CustomerMainStreetAddressText = dispensingSystem.CustomerStreetAddress,
                            CustomerMainCityName = dispensingSystem.CustomerCity,
                            CustomerMainSubdivisionName = dispensingSystem.CustomerState,
                            CustomerMainPostalCode = dispensingSystem.CustomerPostalCode,
                            CustomerMainCountryName = dispensingSystem.CustomerCountry,
                            CustomerNotesText = dispensingSystem.CustomerNotes,
                            ServiceCenterPhoneNumberText = dispensingSystem.ServiceCenterPhoneNumber,
                            ServerStreetAddressText = dispensingSystem.ServerStreetAddress,
                            ServerCityName = dispensingSystem.ServerCity,
                            ServerSubdivisionName = dispensingSystem.ServerState,
                            ServerPostalCode = dispensingSystem.ServerPostalCode,
                            ServerCountryName = dispensingSystem.ServerCountry,
                            ServerLocationId = dispensingSystem.ServerLocationId,
                            DispensingSystemNotesText = dispensingSystem.ServerNotes,
                            EncryptionAlgorithmInternalCode = dispensingSystem.EncryptionAlgorithmInternalCode,
                            PasswordMaximumDurationAmount = dispensingSystem.PasswordExpiration,
                            PasswordMinimumDurationAmount = dispensingSystem.MinimumPasswordAge,
                            DifferentPreviousPasswordQuantity = dispensingSystem.PasswordHistory,
                            MinimumPasswordLengthQuantity = dispensingSystem.MinimumPasswordLength,
                            PasswordMinimumLowercaseQuantity = dispensingSystem.PasswordMinimumLowercaseQuantity,
                            PasswordMinimumUppercaseQuantity = dispensingSystem.PasswordMinimumUppercaseQuantity,
                            PasswordMinimumDigitQuantity = dispensingSystem.PasswordMinimumDigitQuantity,
                            PasswordMinimumSpecialCharacterQuantity = dispensingSystem.PasswordMinimumSpecialCharacterQuantity,
                            PasswordChallengeQuestionQuantity = dispensingSystem.PasswordChallengeQuestionQuantity,
                            RequirePasswordChallengeSetupFlag = dispensingSystem.RequirePasswordChallengeSetup,
                            InactivitySignOutDurationAmount = dispensingSystem.SessionInactivityTimeout,
                            LockedFailedAuthenticationQuantity = dispensingSystem.FailedAuthenticationAttemptsAllowed,
                            LockedFailedAuthenticationDurationAmount = dispensingSystem.FailedAuthenticationLockoutInterval,
                            LockedNoAuthenticationDurationAmount = dispensingSystem.LockedNoAuthenticationDuration,
                            PasswordContentCheckFlag = dispensingSystem.PasswordContentCheck,
                            WebBrowserAuthenticationMethodInternalCode = dispensingSystem.WebBrowserAuthenticationMethodInternalCode,
                            TemporaryPasswordDurationAmount = dispensingSystem.TemporaryPasswordDuration,
                            TemporaryPasswordDurationForNewLocalUserFlag = dispensingSystem.UseTemporaryPasswordDurationForNewLocalUser,
                            UsDoDFlag = dispensingSystem.IsDoD,
                            RecentAuthenticationInformationFlag = dispensingSystem.RecentAuthenticationInformation,
                            AllowLocalUserCreationFlag = dispensingSystem.AllowLocalUserCreation,
                            DisconnectedModeBioIdFlag = dispensingSystem.AllowBioIdInDisconnectedMode,
                            DisconnectedModeCardPinFlag=dispensingSystem.AllowCardPinInDisconnectedMode,
                            DisconnectedModePasswordFlag = dispensingSystem.AllowPasswordInDisconnectedMode,
                            EmailEnabledFlag = dispensingSystem.IsEmailEnabled,
                            SenderEmailAddressValue = dispensingSystem.SenderEmailAddress,
                            SenderUrlId = dispensingSystem.SenderUrlId,
                            SenderPortNumber = dispensingSystem.SenderPortNumber,
                            OltpDataRetentionDurationAmount = dispensingSystem.OltpDataRetentionDuration,
                            StandardRetentionDurationAmount = dispensingSystem.StandardRetentionDuration,
                            PatientRetentionDurationAmount = dispensingSystem.PatientRetentionDuration,
                            InboundMessageErrorRetentionDurationAmount = dispensingSystem.InboundMessageErrorRetentionDuration,
                            InboundMessageNonErrorRetentionDurationAmount = dispensingSystem.InboundMessageNonErrorRetentionDuration,
                            OutboundMessageRetentionDurationAmount = dispensingSystem.OutboundMessageRetentionDuration,
                            InboundContentErrorRetentionDurationAmount = dispensingSystem.InboundContentErrorRetentionDuration,
                            InboundContentNonErrorRetentionDurationAmount = dispensingSystem.InboundContentNonErrorRetentionDuration,
                            OutboundContentRetentionDurationAmount = dispensingSystem.OutboundContentRetentionDuration,
                            SyncRetentionDurationAmount = dispensingSystem.SyncRetentionDuration,
                            GCSMRetentionDurationAmount = dispensingSystem.GCSMRetentionDuration,
                            GCSMCompareReportRetentionDurationAmount = dispensingSystem.GCSMCompareReportRetentionDuration,
                            PurgeableRetentionDurationAmount = dispensingSystem.PurgeableRetentionDuration,
                            PurgeStatisticsRetentionDurationAmount = dispensingSystem.PurgeStatisticsRetentionDuration,
                            SystemOperationRetentionDurationAmount = dispensingSystem.SystemOperationRetentionDuration,
                            ArchiveRetentionDurationAmount = dispensingSystem.ArchiveRetentionDuration,
                            FacilityInboundProblemRetentionDurationAmount = dispensingSystem.FacilityInboundProblemRetentionDuration,
                            ReceivedMessageStatisticsRetentionDurationAmount = dispensingSystem.ReceivedMessageStatisticsRetentionDuration,
                            StorageSpaceSnapshotRetentionDurationAmount = dispensingSystem.StorageSpaceSnapshotRetentionDuration,
                            WorkflowStepEventRetentionDurationAmount = dispensingSystem.WorkflowStepEventRetentionDuration,
                            AbnormalEndSessionRetentionDurationAmount = dispensingSystem.AbnormalEndSessionRetentionDuration,
                            PasswordExpirationNoticeIntervalAmount = dispensingSystem.PasswordExpirationNoticeInterval,
                            SupportPasswordMgmtSystemText = dispensingSystem.SupportPasswordManagementSystem,
                            WarningBannerFlag = dispensingSystem.WarningBanner,
                            WarningBannerHeaderText = dispensingSystem.WarningBannerHeader,
                            WarningBannerTitleText = dispensingSystem.WarningBannerTitle,
                            WarningBannerDescriptionText = dispensingSystem.WarningBannerDescription,
                            SensitiveDataText = dispensingSystem.SensitiveDataBanner,
                            EsSystemFlag = dispensingSystem.ESSystem,
                            PharmogisticsFlag = dispensingSystem.Pharmogistics,
                            ItemDeliveryTransactionPrefixText = dispensingSystem.DeliveryTransactionPrefix,
                            ImageStorageLocationId = dispensingSystem.ImageStorageLocationId,
                            ImageStorageMaximumAmount = dispensingSystem.ImageStorageMaximumAmount,
                            ImageStorageThresholdPercentage = dispensingSystem.ImageStorageThresholdPercentage,
                            ReceivedMessageExportTextFileFlag = dispensingSystem.ReceivedMessageExportTextFile,
                            HandheldSessionInactivityDurationAmount = dispensingSystem.HandheldSessionInactivityDuration,
                            PinFlag = dispensingSystem.Pin,
                            PinMaximumDurationAmount = dispensingSystem.PinMaximumDuration,
                            DifferentPreviousPinQuantity = dispensingSystem.DifferentPreviousPinQuantity,
                            MinimumPinLengthQuantity = dispensingSystem.MinimumPinLength,
                            ParallelizeInboundMessageProcessingFlag = dispensingSystem.ParallelizeInboundMessageProcessing,
                            AlternateSearchFlag = dispensingSystem.AlternateSearch,
                            ItemTransactionRecordQuantityDifferentDurationAmount = dispensingSystem.ItemTransactionRecordQuantityDifferentDuration,
                            ManualPharmacyOrderIdPrefixText = dispensingSystem.ManualPharmacyOrderIdPrefix,
                            SyncChangeTrackingFlag = dispensingSystem.SyncChangeTracking,
                            CardOnlineCertificateStatusUrlId = dispensingSystem.CardOnlineCertificateStatusUrlId,
                            UserDirectorySelfRegistrationFlag = dispensingSystem.UserDirectorySelfRegistrationFlag,
                            UserDirectorySelfRegistrationStartUtcDateTime = dispensingSystem.UserDirectorySelfRegistrationStartUtcDateTime,
                            UserDirectorySelfRegistrationStartLocalDateTime = dispensingSystem.UserDirectorySelfRegistrationStartLocalDateTime,
                            UserDirectorySelfRegistrationEndUtcDateTime = dispensingSystem.UserDirectorySelfRegistrationEndUtcDateTime,
                            UserDirectorySelfRegistrationEndLocalDateTime = dispensingSystem.UserDirectorySelfRegistrationEndLocalDateTime,
                            GCSMFlag = dispensingSystem.GCSMFlag,
                            SupportUserCardPINExemptFlag = dispensingSystem.SupportUserCardPINExempt,
                            LastModifiedBinaryValue = dispensingSystem.LastModified
                        });

                    if (dispensingSystemKey.HasValue)
                    {
                        // Insert primary customer contact.
                        if (dispensingSystem.PrimaryCustomerContact != null)
                        {
                            dispensingSystem.PrimaryCustomerContact.DispensingSystemKey = dispensingSystemKey.Value;
                            dispensingSystem.PrimaryCustomerContact.Rank = DispensingSystemContact.PrimaryContactRankValue;

                            InsertDispensingSystemContact(dispensingSystemRepository, context, dispensingSystem.PrimaryCustomerContact);
                        }


                        // Insert secondary customer contact.
                        if (dispensingSystem.SecondaryCustomerContact != null)
                        {
                            dispensingSystem.SecondaryCustomerContact.DispensingSystemKey = dispensingSystemKey.Value;
                            dispensingSystem.SecondaryCustomerContact.Rank = DispensingSystemContact.SecondaryContactRankValue;

                            InsertDispensingSystemContact(dispensingSystemRepository, context, dispensingSystem.SecondaryCustomerContact);
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

            return dispensingSystemKey.GetValueOrDefault();
        }

        void IDispensingSystemRepository.UpdateDispensingSystem(Context context, DispensingSystem dispensingSystem)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(dispensingSystem, "dispensingSystem");

            try
            {
                CoreDAL.IDispensingSystemRepository dispensingSystemRepository = new CoreDAL.DispensingSystemRepository();

                using (ConnectionScopeFactory.Create())
                using (var tx = TransactionScopeFactory.Create())
                {
                    dispensingSystemRepository.UpdateDispensingSystem(context.ToActionContext(),
                        new CoreDAL.Models.DispensingSystem
                        {
                            DispensingSystemKey = dispensingSystem.Key,
                            ServerId = dispensingSystem.ServerId,
                            DispensingSystemId = dispensingSystem.DispensingSystemId,
                            CustomerName = dispensingSystem.CustomerName,
                            CustomerMainStreetAddressText = dispensingSystem.CustomerStreetAddress,
                            CustomerMainCityName = dispensingSystem.CustomerCity,
                            CustomerMainSubdivisionName = dispensingSystem.CustomerState,
                            CustomerMainPostalCode = dispensingSystem.CustomerPostalCode,
                            CustomerMainCountryName = dispensingSystem.CustomerCountry,
                            CustomerNotesText = dispensingSystem.CustomerNotes,
                            ServiceCenterPhoneNumberText = dispensingSystem.ServiceCenterPhoneNumber,
                            ServerStreetAddressText = dispensingSystem.ServerStreetAddress,
                            ServerCityName = dispensingSystem.ServerCity,
                            ServerSubdivisionName = dispensingSystem.ServerState,
                            ServerPostalCode = dispensingSystem.ServerPostalCode,
                            ServerCountryName = dispensingSystem.ServerCountry,
                            ServerLocationId = dispensingSystem.ServerLocationId,
                            DispensingSystemNotesText = dispensingSystem.ServerNotes,
                            EncryptionAlgorithmInternalCode = dispensingSystem.EncryptionAlgorithmInternalCode,
                            PasswordMaximumDurationAmount = dispensingSystem.PasswordExpiration,
                            PasswordMinimumDurationAmount = dispensingSystem.MinimumPasswordAge,
                            DifferentPreviousPasswordQuantity = dispensingSystem.PasswordHistory,
                            MinimumPasswordLengthQuantity = dispensingSystem.MinimumPasswordLength,
                            PasswordMinimumLowercaseQuantity = dispensingSystem.PasswordMinimumLowercaseQuantity,
                            PasswordMinimumUppercaseQuantity = dispensingSystem.PasswordMinimumUppercaseQuantity,
                            PasswordMinimumDigitQuantity = dispensingSystem.PasswordMinimumDigitQuantity,
                            PasswordMinimumSpecialCharacterQuantity = dispensingSystem.PasswordMinimumSpecialCharacterQuantity,
                            PasswordChallengeQuestionQuantity = dispensingSystem.PasswordChallengeQuestionQuantity,
                            RequirePasswordChallengeSetupFlag = dispensingSystem.RequirePasswordChallengeSetup,
                            InactivitySignOutDurationAmount = dispensingSystem.SessionInactivityTimeout,
                            LockedFailedAuthenticationQuantity = dispensingSystem.FailedAuthenticationAttemptsAllowed,
                            LockedFailedAuthenticationDurationAmount = dispensingSystem.FailedAuthenticationLockoutInterval,
                            LockedNoAuthenticationDurationAmount = dispensingSystem.LockedNoAuthenticationDuration,
                            PasswordContentCheckFlag = dispensingSystem.PasswordContentCheck,
                            WebBrowserAuthenticationMethodInternalCode = dispensingSystem.WebBrowserAuthenticationMethodInternalCode,
                            TemporaryPasswordDurationAmount = dispensingSystem.TemporaryPasswordDuration,
                            TemporaryPasswordDurationForNewLocalUserFlag = dispensingSystem.UseTemporaryPasswordDurationForNewLocalUser,
                            UsDoDFlag = dispensingSystem.IsDoD,
                            RecentAuthenticationInformationFlag = dispensingSystem.RecentAuthenticationInformation,
                            AllowLocalUserCreationFlag = dispensingSystem.AllowLocalUserCreation,
                            DisconnectedModeBioIdFlag = dispensingSystem.AllowBioIdInDisconnectedMode,
                            DisconnectedModeCardPinFlag=dispensingSystem.AllowCardPinInDisconnectedMode,
                            DisconnectedModePasswordFlag = dispensingSystem.AllowPasswordInDisconnectedMode,
                            EmailEnabledFlag = dispensingSystem.IsEmailEnabled,
                            SenderEmailAddressValue = dispensingSystem.SenderEmailAddress,
                            SenderUrlId = dispensingSystem.SenderUrlId,
                            SenderPortNumber = dispensingSystem.SenderPortNumber,
                            OltpDataRetentionDurationAmount = dispensingSystem.OltpDataRetentionDuration,
                            StandardRetentionDurationAmount = dispensingSystem.StandardRetentionDuration,
                            PatientRetentionDurationAmount = dispensingSystem.PatientRetentionDuration,
                            InboundMessageErrorRetentionDurationAmount = dispensingSystem.InboundMessageErrorRetentionDuration,
                            InboundMessageNonErrorRetentionDurationAmount = dispensingSystem.InboundMessageNonErrorRetentionDuration,
                            OutboundMessageRetentionDurationAmount = dispensingSystem.OutboundMessageRetentionDuration,
                            InboundContentErrorRetentionDurationAmount = dispensingSystem.InboundContentErrorRetentionDuration,
                            InboundContentNonErrorRetentionDurationAmount = dispensingSystem.InboundContentNonErrorRetentionDuration,
                            OutboundContentRetentionDurationAmount = dispensingSystem.OutboundContentRetentionDuration,
                            SyncRetentionDurationAmount = dispensingSystem.SyncRetentionDuration,
                            GCSMRetentionDurationAmount = dispensingSystem.GCSMRetentionDuration,
                            GCSMCompareReportRetentionDurationAmount = dispensingSystem.GCSMCompareReportRetentionDuration,
                            PurgeableRetentionDurationAmount = dispensingSystem.PurgeableRetentionDuration,
                            PurgeStatisticsRetentionDurationAmount = dispensingSystem.PurgeStatisticsRetentionDuration,
                            SystemOperationRetentionDurationAmount = dispensingSystem.SystemOperationRetentionDuration,
                            ArchiveRetentionDurationAmount = dispensingSystem.ArchiveRetentionDuration,
                            FacilityInboundProblemRetentionDurationAmount = dispensingSystem.FacilityInboundProblemRetentionDuration,
                            ReceivedMessageStatisticsRetentionDurationAmount = dispensingSystem.ReceivedMessageStatisticsRetentionDuration,
                            StorageSpaceSnapshotRetentionDurationAmount = dispensingSystem.StorageSpaceSnapshotRetentionDuration,
                            WorkflowStepEventRetentionDurationAmount = dispensingSystem.WorkflowStepEventRetentionDuration,
                            AbnormalEndSessionRetentionDurationAmount = dispensingSystem.AbnormalEndSessionRetentionDuration,
                            PasswordExpirationNoticeIntervalAmount = dispensingSystem.PasswordExpirationNoticeInterval,
                            SupportPasswordMgmtSystemText = dispensingSystem.SupportPasswordManagementSystem,
                            WarningBannerFlag = dispensingSystem.WarningBanner,
                            WarningBannerHeaderText = dispensingSystem.WarningBannerHeader,
                            WarningBannerTitleText = dispensingSystem.WarningBannerTitle,
                            WarningBannerDescriptionText = dispensingSystem.WarningBannerDescription,
                            SensitiveDataText = dispensingSystem.SensitiveDataBanner,
                            EsSystemFlag = dispensingSystem.ESSystem,
                            PharmogisticsFlag = dispensingSystem.Pharmogistics,
                            GCSMFlag = dispensingSystem.GCSMFlag,
                            ItemDeliveryTransactionPrefixText = dispensingSystem.DeliveryTransactionPrefix,
                            ImageStorageLocationId = dispensingSystem.ImageStorageLocationId,
                            ImageStorageMaximumAmount = dispensingSystem.ImageStorageMaximumAmount,
                            ImageStorageThresholdPercentage = dispensingSystem.ImageStorageThresholdPercentage,
                            ReceivedMessageExportTextFileFlag = dispensingSystem.ReceivedMessageExportTextFile,
                            HandheldSessionInactivityDurationAmount = dispensingSystem.HandheldSessionInactivityDuration,
                            PinFlag = dispensingSystem.Pin,
                            PinMaximumDurationAmount = dispensingSystem.PinMaximumDuration,
                            DifferentPreviousPinQuantity = dispensingSystem.DifferentPreviousPinQuantity,
                            MinimumPinLengthQuantity = dispensingSystem.MinimumPinLength,
                            ParallelizeInboundMessageProcessingFlag = dispensingSystem.ParallelizeInboundMessageProcessing,
                            AlternateSearchFlag = dispensingSystem.AlternateSearch,
                            ItemTransactionRecordQuantityDifferentDurationAmount = dispensingSystem.ItemTransactionRecordQuantityDifferentDuration,
                            ManualPharmacyOrderIdPrefixText = dispensingSystem.ManualPharmacyOrderIdPrefix,
                            SyncChangeTrackingFlag = dispensingSystem.SyncChangeTracking,
                            CardOnlineCertificateStatusUrlId = dispensingSystem.CardOnlineCertificateStatusUrlId,
                            UserDirectorySelfRegistrationFlag = dispensingSystem.UserDirectorySelfRegistrationFlag,                            
                            UserDirectorySelfRegistrationStartUtcDateTime = dispensingSystem.UserDirectorySelfRegistrationStartUtcDateTime,
                            UserDirectorySelfRegistrationStartLocalDateTime = dispensingSystem.UserDirectorySelfRegistrationStartLocalDateTime,
                            UserDirectorySelfRegistrationEndUtcDateTime = dispensingSystem.UserDirectorySelfRegistrationEndUtcDateTime,
                            UserDirectorySelfRegistrationEndLocalDateTime = dispensingSystem.UserDirectorySelfRegistrationEndLocalDateTime,
                            SupportUserCardPINExemptFlag = dispensingSystem.SupportUserCardPINExempt,
                            LastModifiedBinaryValue = dispensingSystem.LastModified
                        });

                    if (dispensingSystem.PrimaryCustomerContact != null)
                    {
                        dispensingSystem.PrimaryCustomerContact.DispensingSystemKey = dispensingSystem.Key;
                        dispensingSystem.PrimaryCustomerContact.Rank = DispensingSystemContact.PrimaryContactRankValue;

                        UpdateDispensingSystemContact(dispensingSystemRepository, context, dispensingSystem.PrimaryCustomerContact);
                    }

                    if (dispensingSystem.SecondaryCustomerContact != null)
                    {
                        dispensingSystem.SecondaryCustomerContact.DispensingSystemKey = dispensingSystem.Key;
                        dispensingSystem.SecondaryCustomerContact.Rank = DispensingSystemContact.SecondaryContactRankValue;

                        UpdateDispensingSystemContact(dispensingSystemRepository, context, dispensingSystem.SecondaryCustomerContact);
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
        
        private static void InsertDispensingSystemContact(CoreDAL.IDispensingSystemRepository dispensingSystemRepository, Context context, DispensingSystemContact dispensingSystemContact)
        {
            dispensingSystemRepository.InsertDispensingSystemContact(context.ToActionContext(),
                new CoreDAL.Models.DispensingSystemContact
                {
                    DispensingSystemContactKey = dispensingSystemContact.Key,
                    DispensingSystemKey = dispensingSystemContact.DispensingSystemKey,
                    RankValue = dispensingSystemContact.Rank,
                    CustomerContactName = dispensingSystemContact.Name,
                    CustomerContactDescriptionText = dispensingSystemContact.Description,
                    CustomerContactPhoneNumberText = dispensingSystemContact.PhoneNumber,
                    CustomerContactFaxNumberText = dispensingSystemContact.FaxNumber,
                    CustomerContactEmailAddressValue = dispensingSystemContact.EmailAddress,
                    CustomerContactPreferredMethodInternalCode = dispensingSystemContact.CustomerContactPreferredMethodInternalCode,
                    LastModifiedBinaryValue = dispensingSystemContact.LastModified
                });
        }
        
        private static void UpdateDispensingSystemContact(CoreDAL.IDispensingSystemRepository dispensingSystemRepository, Context context, DispensingSystemContact dispensingSystemContact)
        {
            SqlBuilder query = new SqlBuilder();
                query.SELECT()
                     ._("DispensingSystemContactKey")
                     .FROM("Core.DispensingSystemContact")
                     .WHERE("EndUTCDateTime IS NULL")
                     .WHERE("DispensingSystemKey = @DispensingSystemKey")
                     .WHERE("RankValue = @RankValue");

            using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
            {
                bool exists = connectionScope.Query(
                    query.ToString(),
                    new
                    {
                        DispensingSystemKey = dispensingSystemContact.DispensingSystemKey,
                        RankValue = dispensingSystemContact.Rank
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.Text).Any();

                if (exists)
                {
                    dispensingSystemRepository.UpdateDispensingSystemContact(context.ToActionContext(),
                        new CoreDAL.Models.DispensingSystemContact
                        {
                            DispensingSystemContactKey = dispensingSystemContact.Key,
                            DispensingSystemKey = dispensingSystemContact.DispensingSystemKey,
                            RankValue = dispensingSystemContact.Rank,
                            CustomerContactName = dispensingSystemContact.Name,
                            CustomerContactDescriptionText = dispensingSystemContact.Description,
                            CustomerContactPhoneNumberText = dispensingSystemContact.PhoneNumber,
                            CustomerContactFaxNumberText = dispensingSystemContact.FaxNumber,
                            CustomerContactEmailAddressValue = dispensingSystemContact.EmailAddress,
                            CustomerContactPreferredMethodInternalCode = dispensingSystemContact.CustomerContactPreferredMethodInternalCode,
                            LastModifiedBinaryValue = dispensingSystemContact.LastModified
                        });
                }
                else
                {
                    InsertDispensingSystemContact(dispensingSystemRepository, context, dispensingSystemContact);
                }
            }
        }
    }
}
