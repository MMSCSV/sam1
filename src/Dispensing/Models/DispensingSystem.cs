using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Resources;
using Pyxis.Core.Data.InternalCodes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareFusion.Dispensing.Models
{
    /// <summary>
    /// Represents the highest level of a dispensing system installation, and 
    /// settings that are applicable at all levels
    /// </summary>
    [Serializable]
    public class DispensingSystem : IEntity<Guid>
    {
        #region Constructors

        public DispensingSystem()
        {
            PrimaryCustomerContact = new DispensingSystemContact();
            SecondaryCustomerContact = new DispensingSystemContact();
        }

        public DispensingSystem(Guid key)
        {
            Key = key;
        }

        public static DispensingSystem Default
        {
            get
            {
                return new DispensingSystem
                {
                    EncryptionAlgorithmInternalCode = EncryptionAlgorithmInternalCodes.SHA_256,
                    WebBrowserAuthenticationMethodInternalCode = AuthenticationMethodInternalCodes.UIDPWD,
                    SessionInactivityTimeout = ValidationConstants.DispensingSystemSessionInactivityTimeoutDefaultValue,
                    FailedAuthenticationLockoutInterval =
                        ValidationConstants.DispensingSystemFailedAuthenticationLockoutIntervalDefaultValue,
                    FailedAuthenticationAttemptsAllowed =
                        ValidationConstants.DispensingSystemFailedAuthenticationAttemptsAllowedDefaultValue,
                    MinimumPasswordLength = ValidationConstants.DispensingSystemMinimumPasswordLengthDefaultValue,
                    MinimumPasswordAge = ValidationConstants.DispensingSystemMinimumPasswordAgeDefaultValue,
                    PasswordHistory = ValidationConstants.DispensingSystemPasswordHistoryDefaultValue,
                    PasswordExpiration = ValidationConstants.DispensingSystemPasswordExpirationDefaultValue,
                    TemporaryPasswordDuration = ValidationConstants.DispensingSystemTemporaryPasswordDurationDefaultValue,
                    AllowLocalUserCreation = ValidationConstants.DispensingSystemAllowLocalUserCreationDefaultValue,
                    PasswordExpirationNoticeInterval = ValidationConstants.DispensingSystemPassordExpirationNoticeDefaultValue,
                    ESSystem = ValidationConstants.DispensingSystemDefaultDeployedSystem,
                    DeliveryTransactionPrefix = ValidationConstants.DispensingSystemDefaultDeliveryTransactionPrefix,
                    ManualPharmacyOrderIdPrefix = DispensingResources.DispensingSystemDefaultManualPharmacyOrderIdPrefix,
                    ImageStorageThresholdPercentage = 15,
                    DifferentPreviousPinQuantity = 8,
                    MinimumPinLength = 4,
                    PinMaximumDuration = 90,
                    HandheldSessionInactivityDuration = 60,
                    ItemTransactionRecordQuantityDifferentDuration = 60,
                    SystemOperationRetentionDuration = 93,
                    ArchiveRetentionDuration = 365,
                    FacilityInboundProblemRetentionDuration = 93,
                    ReceivedMessageStatisticsRetentionDuration = 365,
                    StorageSpaceSnapshotRetentionDuration = 93,
                    WorkflowStepEventRetentionDuration = 365,
                    AbnormalEndSessionRetentionDuration = 365,
                    ParallelizeInboundMessageProcessing = true,

                    // Non-UI properties
                    OltpDataRetentionDuration = 31,
                    StandardRetentionDuration = 31,
                    PatientRetentionDuration = 31,
                    InboundMessageErrorRetentionDuration = 31,
                    InboundMessageNonErrorRetentionDuration = 31,
                    PurgeableRetentionDuration = 93,
                    PurgeStatisticsRetentionDuration = 31,
                    OutboundMessageRetentionDuration = 93,
                    InboundContentErrorRetentionDuration = 7,
                    InboundContentNonErrorRetentionDuration = 7,
                    OutboundContentRetentionDuration = 7,
                    SyncRetentionDuration = 31,
                    GCSMRetentionDuration = 731,
                    GCSMCompareReportRetentionDuration = 6,

                    SupportUserCardPINExempt = ValidationConstants.DispensingSystemSupportUserCardPINExemptFlagDefaultValue
                };
            }
        }

        #endregion

        #region Operator Overloads

        public static implicit operator DispensingSystem(Guid key)
        {
            return FromKey(key);
        }

        public static DispensingSystem FromKey(Guid key)
        {
            return new DispensingSystem(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of a dispensing system.
        /// </summary>
        [Column("DispensingSystemKey")]
        public Guid Key { get; set; }

        public string ServerId { get; set; }

        /// <summary>
        /// Gets or sets the ID of a Dispensing System as per the company.
        /// </summary>
        public string DispensingSystemId { get; set; }

        /// <summary>
        /// Gets or sets the name of a customer.
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or sets the part of a customer's main address that is more specific than
        /// a city name.
        /// </summary>
        [Column("CustomerMainStreetAddressText")]
        public string CustomerStreetAddress { get; set; }

        /// <summary>
        /// Gets or sets the name of a city in a customer's main address.
        /// </summary>
        [Column("CustomerMainCityName")]
        public string CustomerCity { get; set; }

        /// <summary>
        /// Gets or sets the name of a state in a customer's main address.
        /// </summary>
        [Column("CustomerMainSubdivisionName")]
        public string CustomerState { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies a postal location in a customer's
        /// main address.
        /// </summary>
        [Column("CustomerMainPostalCode")]
        public string CustomerPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the name of a country in a cusomter's main address.
        /// </summary>
        [Column("CustomerMainCountryName")]
        public string CustomerCountry { get; set; }

        /// <summary>
        /// Gets or sets the free-form notes about a customer.
        /// </summary>
        [Column("CustomerNotesText")]
        public string CustomerNotes { get; set; }

        /// <summary>
        /// Gets or sets the phone number for the company's service center.
        /// </summary>
        [Column("ServiceCenterPhoneNumberText")]
        public string ServiceCenterPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the part of an address that is more specific that a city name where
        /// a server is located.
        /// </summary>
        [Column("ServerStreetAddressText")]
        public string ServerStreetAddress { get; set; }

        /// <summary>
        /// Gets or sets the name of a city where a server is located.
        /// </summary>
        [Column("ServerCityName")]
        public string ServerCity { get; set; }

        /// <summary>
        /// Gets or sets the state where a server is located.
        /// </summary>
        [Column("ServerSubdivisionName")]
        public string ServerState { get; set; }

        /// <summary>
        /// Gets or sets the code that identifies a postal location where a server is located.
        /// </summary>
        public string ServerPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the name of a country where a server is located.
        /// </summary>
        [Column("ServerCountryName")]
        public string ServerCountry { get; set; }

        /// <summary>
        /// Gets or sets the ID of a location where a server is located as per the company.
        /// </summary>
        public string ServerLocationId { get; set; }

        /// <summary>
        /// Gets or sets the free-form notes about a Dispensing System.
        /// </summary>
        [Column("DispensingSystemNotesText")]
        public string ServerNotes { get; set; }

        /// <summary>
        /// Gets or sets the encryption algorithm internal code used by the system.
        /// </summary>
        public string EncryptionAlgorithmInternalCode { get; set; }

        /// <summary>
        /// Gets the encryption algorithm used by the system.
        /// </summary>
        public EncryptionAlgorithmInternalCode EncryptionAlgorithm
        {
            get { return EncryptionAlgorithmInternalCode.FromInternalCode<EncryptionAlgorithmInternalCode>(); }
        }

        /// <summary>
        /// Gets or sets the duration in days after which a password expires.
        /// </summary>
        [Column("PasswordMaximumDurationAmount")]
        public short? PasswordExpiration { get; set; }

        /// <summary>
        /// Gets or sets the minimum duration in days from the last time a password was changed by a user until the user
        /// may change the password again.
        /// </summary>
        [Column("PasswordMinimumDurationAmount")]
        public short MinimumPasswordAge { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of password changes that must occur before a password maybe reused.
        /// </summary>
        [Column("DifferentPreviousPasswordQuantity")]
        public short PasswordHistory { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of characters in a password.
        /// </summary>
        [Column("MinimumPasswordLengthQuantity")]
        public byte MinimumPasswordLength { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of lowercase letters in a password.
        /// </summary>
        [Column("PasswordMinimumLowercaseQuantity")]
        public byte PasswordMinimumLowercaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of uppercase letters in a password.
        /// </summary>
        [Column("PasswordMinimumUppercaseQuantity")]
        public byte PasswordMinimumUppercaseQuantity { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of digits in a password.
        /// </summary>
        [Column("PasswordMinimumDigitQuantity")]
        public byte PasswordMinimumDigitQuantity { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of special characters in a password.
        /// </summary>
        [Column("PasswordMinimumSpecialCharacterQuantity")]
        public byte PasswordMinimumSpecialCharacterQuantity { get; set; }

        /// <summary>
        /// Gets or sets the number of challenge questions that a user must provide an answer to.
        /// </summary>
        [Column("PasswordChallengeQuestionQuantity")]
        public byte? PasswordChallengeQuestionQuantity { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a new user must enter challenge question answers
        /// when signing in to the system for the first time.
        /// </summary>
        [Column("RequirePasswordChallengeSetupFlag")]
        public bool RequirePasswordChallengeSetup { get; set; }

        /// <summary>
        /// Gets or sets the duration in minutes after which a user is signed out following no activity.
        /// </summary>
        [Column("InactivitySignOutDurationAmount")]
        public int SessionInactivityTimeout { get; set; }

        /// <summary>
        /// Gets or sets the number of failed authentication attempts since the last successful authentication
        /// after which a user account is blocked.
        /// </summary>
        [Column("LockedFailedAuthenticationQuantity")]
        public byte? FailedAuthenticationAttemptsAllowed { get; set; }

        /// <summary>
        /// Gets or sets the duration in minutes for which the number of failed authentication attempts are counted.
        /// </summary>
        [Column("LockedFailedAuthenticationDurationAmount")]
        public int? FailedAuthenticationLockoutInterval { get; set; }

        /// <summary>
        /// Gets or sets the duration (in days) after which an account is locked because no recent authentication 
        /// has occurred.
        /// </summary>
        [Column("LockedNoAuthenticationDurationAmount")]
        public short? LockedNoAuthenticationDuration { get; set; }

        /// <summary>
        /// Gets or sets the value that determines whether a password is checked against a dictionary and also whether
        /// a check is performed that sufficient content has changed from prior version.
        /// </summary>
        [Column("PasswordContentCheckFlag")]
        public bool PasswordContentCheck { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies an authentication method that us used at the
        /// web browser.
        /// </summary>
        public string WebBrowserAuthenticationMethodInternalCode { get; set; }

        /// <summary>
        /// Gets the internal code that identifies an authentication method that us used at the
        /// web browser.
        /// </summary>
        public AuthenticationMethod WebBrowserAuthenticationMethod
        {
            get { return WebBrowserAuthenticationMethodInternalCode.FromInternalCode<AuthenticationMethodInternalCode>(); }
        }

        /// <summary>
        /// Gets or sets the temporary password duration amount.
        /// </summary>
        [Column("TemporaryPasswordDurationAmount")]
        public short? TemporaryPasswordDuration { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether temporary password duration amount should be used against
        /// new local user accounts.
        /// </summary>
        [Column("TemporaryPasswordDurationForNewLocalUserFlag")]
        public bool UseTemporaryPasswordDurationForNewLocalUser { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a Dispensing System is used 
        /// by the U.S. Department of Defense.
        /// </summary>
        [Column("USDoDFlag")]
        [Obsolete("Replaced by various other fields.")]
        public bool IsDoD { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the system displays recent authentication information, including
        /// last successful authentication and last unsuccessful authentication information.
        /// </summary>
        [Column("RecentAuthenticationInformationFlag")]
        public bool RecentAuthenticationInformation { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether user accounts may be created not using an Actve Directory.
        /// </summary>
        [Column("AllowLocalUserCreationFlag")]
        public bool AllowLocalUserCreation { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a user may authenticate using BioID in disconnected mode.
        /// </summary>
        [Column("DisconnectedModeBioIDFlag")]
        public bool AllowBioIdInDisconnectedMode { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a user may authenticate using BioID in disconnected mode.
        /// </summary>
        [Column("DisconnectedModeCardPINFlag")]
        public bool AllowCardPinInDisconnectedMode { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a user may be authenticated using a password in disconnected mode.
        /// </summary>
        [Column("DisconnectedModePasswordFlag")]
        public bool AllowPasswordInDisconnectedMode { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a Dispensing System is sending emails.
        /// </summary>
        [Column("EmailEnabledFlag")]
        public bool IsEmailEnabled { get; set; }

        /// <summary>
        /// Gets or sets the email address for a Dispensing System's email sender.
        /// </summary>
        [Column("SenderEmailAddressValue")]
        public string SenderEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the URL of a Dispensing System's email sender.
        /// </summary>
        public string SenderUrlId { get; set; }

        /// <summary>
        /// Gets or sets the port number of a Dispensing System's email sender.
        /// </summary>
        public int? SenderPortNumber { get; set; }

        /// <summary>
        /// Gets or sets the duration (in days) for which OLTP data is retained.
        /// </summary>
        [Column("OLTPDataRetentionDurationAmount")]
        public short OltpDataRetentionDuration { get; set; }

        /// <summary>
        /// Gets or sets the retention period (in days) that is used for most records.
        /// </summary>
        [Column("StandardRetentionDurationAmount")]
        public short StandardRetentionDuration { get; set; }

        /// <summary>
        /// Gets or sets the retention period (in days) that is used for patient-related 
        /// (but not encounter) records.
        /// </summary>
        [Column("PatientRetentionDurationAmount")]
        public short PatientRetentionDuration { get; set; }

        /// <summary>
        /// Gets or sets the retention period (in days) that is used for received-message 
        /// records involving errors.
        /// </summary>
        [Column("InboundMessageErrorRetentionDurationAmount")]
        public short InboundMessageErrorRetentionDuration { get; set; }

        /// <summary>
        /// Gets or sets the retention period (in days) that is used for received-message 
        /// records not involving errors.
        /// </summary>
        [Column("InboundMessageNonErrorRetentionDurationAmount")]
        public short InboundMessageNonErrorRetentionDuration { get; set; }

        /// <summary>
        /// Gets or sets the retention period (in days) that is used for published-message records.
        /// </summary>
        [Column("OutboundMessageRetentionDurationAmount")]
        public short OutboundMessageRetentionDuration { get; set; }

        /// <summary>
        /// Gets or sets the retention period (in days) that is used for received-content
        /// records involving errors.
        /// </summary>
        [Column("InboundContentErrorRetentionDurationAmount")]
        public short InboundContentErrorRetentionDuration { get; set; }

        /// <summary>
        /// Gets or sets the retention period (in days) that is used for received-content
        /// records not involving errors.
        /// </summary>
        [Column("InboundContentNonErrorRetentionDurationAmount")]
        public short InboundContentNonErrorRetentionDuration { get; set; }

        /// <summary>
        /// Gets or sets the retention period (in days) that is used for published-content records.
        /// </summary>
        [Column("OutboundContentRetentionDurationAmount")]
        public short OutboundContentRetentionDuration { get; set; }

        /// <summary>
        /// Gets or sets the retention period (in days) that is used for sync records.
        /// </summary>
        [Column("SyncRetentionDurationAmount")]
        public short SyncRetentionDuration { get; set; }

        /// <summary>
        /// Gets or sets the retention period for GCSM
        /// </summary>
        [Column("GCSMRetentionDurationAmount")]
        public short GCSMRetentionDuration { get; set; }

        /// <summary>
        /// Gets or sets the compare report retention period for GCSM
        /// </summary>
        [Column("GCSMCompareReportRetentionDurationAmount")]
        public short GCSMCompareReportRetentionDuration { get; set; }

        /// <summary>
        /// Gets or sets the retention period (in days) that is used for records in 'Purgeable XXX' tables.
        /// </summary>
        [Column("PurgeableRetentionDurationAmount")]
        public short PurgeableRetentionDuration { get; set; }

        [Column("PurgeStatisticsRetentionDurationAmount")]
        public short PurgeStatisticsRetentionDuration { get; set; }

        /// <summary>
        /// Gets or sets the interval (in days) between which password expiration notices are shown to a user
        /// if their password is about to expire; 0 means do not show.
        /// </summary>
        [Column("PasswordExpirationNoticeIntervalAmount")]
        public short PasswordExpirationNoticeInterval { get; set; }

        /// <summary>
        /// Gets or sets the text that contains settings for the password management system.
        /// </summary>
        [Column("SupportPasswordMgmtSystemText")]
        public string SupportPasswordManagementSystem { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the configurable warning banner is displayed at dispensing devices and server.
        /// </summary>
        [Column("WarningBannerFlag")]
        public bool WarningBanner { get; set; }

        /// <summary>
        /// Gets or sets the header for the warning that displays at dispensing devices and server when the Activate Warning Banner is turned on.
        /// </summary>
        [Column("WarningBannerHeaderText")]
        public string WarningBannerHeader { get; set; }

        /// <summary>
        /// Gets or sets the title of the warning that displays at dispensing devices and server when the Activate Warning Banner is turned on.
        /// </summary>
        [Column("WarningBannerTitleText")]
        public string WarningBannerTitle { get; set; }

        /// <summary>
        /// Gets or sets the description that displays at dispensing devices and server when the Activate Warning Banner is turned on.
        /// </summary>
        [Column("WarningBannerDescriptionText")]
        public string WarningBannerDescription { get; set; }

        /// <summary>
        /// Gets or sets the text that states that the data is sensitive.
        /// </summary>
        [Column("SensitiveDataText")]
        public string SensitiveDataBanner { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the Pyxis ES system is deployed (or whether the ES server is 
        /// deployed just to support Pyxis Pharmogistics).
        /// </summary>
        [Column("ESSystemFlag")]
        public bool ESSystem { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether the Pyxis Pharmogistics system is deployed.
        /// </summary>
        [Column("PharmogisticsFlag")]
        public bool Pharmogistics { get; set; }

        /// <summary>
        /// Gets or sets the text that is included at the start of an item delivery transaction ID barcodes.
        /// </summary>
        [Column("ItemDeliveryTransactionPrefixText")]
        public string DeliveryTransactionPrefix { get; set; }

        /// <summary>
        /// Gets or sets the ID of where images are located.
        /// </summary>
        public string ImageStorageLocationId { get; set; }

        /// <summary>
        /// Gets or sets the maximum storage space in megabytes (MB).
        /// </summary>
        public long? ImageStorageMaximumAmount { get; set; }

        /// <summary>
        /// Gets or sets the percentage of image storage after which a warning is given.
        /// </summary>
        public byte ImageStorageThresholdPercentage { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether the dispensing system exports received messages to text files
        /// </summary>
        [Obsolete("SR1.7.3 - Not Used")]
        [Column("ReceivedMessageExportTextFileFlag")]
        public bool ReceivedMessageExportTextFile { get; set; }

        /// <summary>
        /// Gets or sets duration (in minutes) after which a handheld device goes into sleep mode
        /// </summary>
        [Column("HandheldSessionInactivityDurationAmount")]
        public short HandheldSessionInactivityDuration { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicates whether waking up a handheld is allowed by entering a PIN 
        /// </summary>
        [Column("PINFlag")]
        public bool Pin { get; set; }

        /// <summary>
        /// Gets or sets the duration (in days) after which a  PIN expires
        /// </summary>
        [Column("PINMaximumDurationAmount")]
        public short? PinMaximumDuration { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of PIN changes that must occur before a PIN may be reused
        /// </summary>
        [Column("DifferentPreviousPINQuantity")]
        public short DifferentPreviousPinQuantity { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of password changes that must occur before a password may be reused
        /// </summary>
        [Column("MinimumPINLengthQuantity")]
        public byte MinimumPinLength { get; set; }

        /// <summary>
        /// Gets or sets the flag that indicated whether inbound messages are processed in parallel, with the different streams 
        /// segmented by patient silo
        /// </summary>
        [Column("ParallelizeInboundMessageProcessingFlag")]
        public bool ParallelizeInboundMessageProcessing { get; set; }

        [Column("AlternateSearchFlag")]
        public bool AlternateSearch { get; set; }

        [Column("ItemTransactionRecordQuantityDifferentDurationAmount")]
        public short ItemTransactionRecordQuantityDifferentDuration { get; set; }

        [Column("ManualPharmacyOrderIDPrefixText")]
        public string ManualPharmacyOrderIdPrefix { get; set; }

        [Column("SystemOperationRetentionDurationAmount")]
        public short SystemOperationRetentionDuration { get; set; }

        [Column("ArchiveRetentionDurationAmount")]
        public short ArchiveRetentionDuration { get; set; }

        [Column("FacilityInboundProblemRetentionDurationAmount")]
        public short FacilityInboundProblemRetentionDuration { get; set; }

        [Column("ReceivedMessageStatisticsRetentionDurationAmount")]
        public short ReceivedMessageStatisticsRetentionDuration { get; set; }

        [Column("StorageSpaceSnapshotRetentionDurationAmount")]
        public short StorageSpaceSnapshotRetentionDuration { get; set; }

        [Column("WorkflowStepEventRetentionDurationAmount")]
        public short WorkflowStepEventRetentionDuration { get; set; }

        [Column("AbnormalEndSessionRetentionDurationAmount")]
        public short AbnormalEndSessionRetentionDuration { get; set; }

        /// <summary>
        /// Gets or sets the sync change tracking flag.  
        /// </summary>
        [Column("SyncChangeTrackingFlag")]
        public bool SyncChangeTracking { get; set; }

        /// <summary>
        /// Gets or sets the CardOnlineCertificateStatusUrlId
        /// </summary>
        [Column("CardOnlineCertificateStatusURLID")]
        public string CardOnlineCertificateStatusUrlId { get; set; }

        /// <summary>
        /// Gets or sets the binary value that is unique within a database and that is assigned a new
        /// value on insert and on update.
        /// </summary>
        [Column("LastModifiedBinaryValue")]
        public byte[] LastModified { get; set; }

        /// <summary>
        /// Gets or sets the primary customer contact.
        /// </summary>
        public DispensingSystemContact PrimaryCustomerContact { get; set; }

        /// <summary>
        /// Gets or sets the secondary customer contact.
        /// </summary>
        public DispensingSystemContact SecondaryCustomerContact { get; set; }

        /// <summary>
        /// Gets or sets UserDirectorySelfRegistrationFlag
        /// </summary>
        [Column("UserDirectorySelfRegistrationFlag")]
        public bool UserDirectorySelfRegistrationFlag { get; set; }

        /// <summary>
        /// Gets or sets UserDirectorySelfRegistrationStartDateTime
        /// </summary>
        [Column("UserDirectorySelfRegistrationStartUtcDateTime")]
        public DateTime? UserDirectorySelfRegistrationStartUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets UserDirectorySelfRegistrationEndDateTime
        /// </summary>
        [Column("UserDirectorySelfRegistrationStartLocalDateTime")]
        public DateTime? UserDirectorySelfRegistrationStartLocalDateTime { get; set; }

        /// <summary>
        /// Gets or sets UserDirectorySelfRegistrationStartDateTime
        /// </summary>
        [Column("UserDirectorySelfRegistrationEndUtcDateTime")]
        public DateTime? UserDirectorySelfRegistrationEndUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets UserDirectorySelfRegistrationEndDateTime
        /// </summary>
        [Column("UserDirectorySelfRegistrationEndLocalDateTime")]
        public DateTime? UserDirectorySelfRegistrationEndLocalDateTime { get; set; }

        /// <summary>
        /// Gets or sets GCSMFlag
        /// </summary>
        [Column("GCSMFlag")]
        public bool GCSMFlag { get; set; }

        /// <summary>
        /// Specification: 1316631.
        /// Gets or sets indication whether support users can sign in without Card + PIN.
        /// </summary>
        [Column("SupportUserCardPINExemptFlag")]
        public bool SupportUserCardPINExempt { get; set; }

        #endregion

        #region Public Members

        public bool IsTransient()
        {
            return Key == default(Guid);
        }

        #endregion
    }
}
