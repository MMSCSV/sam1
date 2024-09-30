using System;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Validators;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents an account for a person that uses a clinical-technology product.
    /// </summary>
    /// <remarks>
    /// Kinds of user that have accounts: clinician (nurse or physician), pharmacist,
    /// refill tech.
    /// A user does not necessarily provide care for a patient.
    /// </remarks>
    [Serializable]
    public class UserAccount : Entity<Guid>
    {
        #region Constructors

        public UserAccount()
        {

        }

        public UserAccount(Guid key)
        {
            Key = key;
        }

        public UserAccount(Guid key, Guid snapshotKey)
        {
            Key = key;
            SnapshotKey = snapshotKey;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator UserAccount(Guid key)
        {
            return FromKey(key);
        }

        public static UserAccount FromKey(Guid key)
        {
            return new UserAccount(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the UTC date and time of when a password expiration notice was last shown
        /// to a user.
        /// </summary>
        public DateTime? LastPasswordExpirationNoticeUtcDateTime { get; internal set; }

        /// <summary>
        /// Gets the local date and time of when a password expiration notice was last shown
        /// to a user.
        /// </summary>
        public DateTime? LastPasswordExpirationNoticeDateTime { get; internal set; }

        /// <summary>
        /// Gets the UTC date and time of when a user is last successfully authenticated using a
        /// password.
        /// </summary>
        public DateTime? LastSuccessfulPasswordAuthenticationUtcDateTime { get; internal set; }

        /// <summary>
        /// Gets the local date and time of when a user is last successfully authenticated using a
        /// password.
        /// </summary>
        public DateTime? LastSuccessfulPasswordAuthenticationDateTime { get; internal set; }

        /// <summary>
        /// Gets the surrogate key of an area that is the last area a user chose to work with.
        /// </summary>
        public Guid? LastAreaKey { get; set; }

        /// <summary>
        /// Gets or sets the user account snapshot key.
        /// </summary>
        public Guid SnapshotKey { get; set; }

        /// <summary>
        /// Gets or sets the key of an active directory domain.
        /// </summary>
        public ActiveDirectoryDomain ActiveDirectoryDomain { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user (account).
        /// </summary>
        /// <value>The user id.</value>
        [DispensingStringLengthValidator(ValidationConstants.UserIdUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "UserAccountUserIdOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "UserAccountUserIdRequired")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        /// <value>The first name.</value>
        [DispensingStringLengthValidator(ValidationConstants.UserFirstNameUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "UserAccountFirstNameOutOfBounds")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the second and further given names or initials thereof of a user account.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.UserMiddleNameUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "UserAccountMiddleNameOutOfBounds")]
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        /// <value>The last name.</value>
        [DispensingStringLengthValidator(ValidationConstants.UserLastNameUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "UserAccountLastNameOutOfBounds")]
        [RequiredStringValidator(MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "UserAccountLastNameRequired")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the suffix of the user name.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.UserSuffixUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "UserAccountSuffixOutOfBounds")]
        public string Suffix { get; set; }

        /// <summary>
        /// Gets or sets the full name of the user.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the initials of the user.
        /// </summary>
        /// <value>The initials.</value>
        [DispensingStringLengthValidator(ValidationConstants.UserInitialsUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "UserAccountInitialsOutOfBounds")]
        public string Initials { get; set; }

        /// <summary>
        /// Gets or sets the qualifications that a user has.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.UserJobTitleUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "UserAccountJobTitleOutOfBounds")]
        public string JobTitle { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [DispensingStringLengthValidator(ValidationConstants.UserEmailAddressUpperBound,
            MessageTemplateResourceType = typeof(ValidationStrings),
            MessageTemplateResourceName = "UserAccountEmailAddressOutOfBounds")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the text that provides user contact information such as phone or extension.
        /// </summary>
        public string ContactInformation { get; set; }

        /// <summary>
        /// Gets or sets the value that can be scanned for a user.
        /// </summary>
        public string ScanCode { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time when a user account expires.
        /// </summary>
        /// <value>The account expiration UTC date.</value>
        public DateTime? AccountExpirationUtcDate { get; set; }

        /// <summary>
        /// Gets or sets the local date and time when a user account expires.
        /// </summary>
        /// <value>The account expiration date.</value>
        public DateTime? AccountExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this user account has the fingerprint exempt.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this user account is exempt of fingerprint; otherwise, <c>false</c>.
        /// </value>
        public bool IsFingerprintExempt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this user account is a temporary account
        /// (in contrast to a permanent account) in the system.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is temporary; otherwise, <c>false</c>.
        /// </value>
        public bool IsTemporary { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this user account is disabled.
        /// </summary>
        /// <value><c>true</c> if this instance is locked; otherwise, <c>false</c>.</value>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this user account is the super user.
        /// </summary>
        /// <value><c>true</c> if this user account is the super user; otherwise, <c>false</c>.</value>
        public bool IsSuperUser { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this user account is the support user.
        /// </summary>
        /// <value><c>true</c> if this user account is the support user; otherwise, <c>false</c>.</value>
        public bool IsSupportUser { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a user type.
        /// </summary>
        public Guid? UserTypeKey { get; set; }

        /// <summary>
        /// Gets or sets the immutable GUID as per Active Directory, which does not change even if
        /// the user changes their domain.
        /// </summary>
        public Guid? ActiveDirectoryObjectGuid { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time of a when a password was last changed within a user directory.
        /// </summary>
        public DateTime? DirectoryChangePasswordUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time of a when a password was last changed within a user directory.
        /// </summary>
        public DateTime? DirectoryChangePasswordDateTime { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether this user account is exempt from using card+pin
        /// </summary>
        public bool IsCardPinExempt { get; set; }

        /// <summary>
        /// Gets or sets the Card Serial number
        /// </summary>
        public string CardSerialId { get; set; }

        /// <summary>
        /// Gets or sets the RFID Card Serial number
        /// </summary>
        public string RFIDCardSerialID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this user account is active.
        /// </summary>
        /// <value><c>true</c> if this user account is active; otherwise, <c>false</c>.</value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the password credential.
        /// </summary>
        /// <value>The password credential.</value>
        public PasswordCredential PasswordCredential { get; set; }

        /// <summary>
        /// Gets or sets the password confirmation property.
        /// </summary>
        /// <value>The password confirmation</value>
        /// <remarks>This property is not part of the data model.  It is for use in the view to maintain state of a value.</remarks>
        public string ConfirmPassword { get; set; } 

        /// <summary>
        /// Gets or sets the list of facilities associated with this user.
        /// </summary>
        public Guid[] FacilityKeys { get; internal set; }

        /// <summary>
        /// Gets or sets the list of user roles associated with a user account.
        /// </summary>
        public UserRole[] UserRoles { get; set; }

        #endregion

        #region Public Members

        public bool IsExpired()
        {
            return (AccountExpirationUtcDate <= DateTime.UtcNow);
        }

        public void Expire()
        {
            DateTime localNow = DateTime.Now;

            AccountExpirationDate = localNow;
            AccountExpirationUtcDate = localNow.ToUniversalTime();    
        }

        public bool IsActiveDirectoryUser
        {
            get
            {
                return ActiveDirectoryObjectGuid.HasValue;    
            }
        }

        #endregion
    }
}
