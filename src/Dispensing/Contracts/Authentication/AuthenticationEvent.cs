using System;
using System.ComponentModel.DataAnnotations.Schema;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents an event in which a Dispensing System successfully
    /// authenticates a user.
    /// </summary>
    [Serializable]
    public class AuthenticationEvent : IEntity<Guid>
    {
        #region Constructors

        public AuthenticationEvent()
        {
        }

        public AuthenticationEvent(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator AuthenticationEvent(Guid key)
        {
            return FromKey(key);
        }

        public static AuthenticationEvent FromKey(Guid key)
        {
            return new AuthenticationEvent(key);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the surrogate key of an authentication event key.
        /// </summary>
        [Column("AuthenticationEventKey")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies an authentication purpose.
        /// </summary>
        public string AuthenticationPurposeInternalCode { get; set; }

        /// <summary>
        /// Gets the internal code that identifies an authentication purpose.
        /// </summary>
        public AuthenticationPurposeInternalCode AuthenticationPurpose
        {
            get { return AuthenticationPurposeInternalCode.FromInternalCode<AuthenticationPurposeInternalCode>(); }
            set { AuthenticationPurposeInternalCode = value.ToInternalCode(); }
        }

        /// <summary>
        /// Gets or sets the internal code that identifies an authentication method.
        /// </summary>
        public string AuthenticationMethodInternalCode { get; set; }

        /// <summary>
        /// Gets the internal code that identifies an authentication method.
        /// </summary>
        public AuthenticationMethodInternalCode AuthenticationMethod
        {
            get { return AuthenticationMethodInternalCode.FromInternalCode<AuthenticationMethodInternalCode>(); }
            set { AuthenticationMethodInternalCode = value.ToInternalCode(); }
        }

        /// <summary>
        /// Gets or sets the surrogate key of an Active Directory domain.
        /// </summary>
        public Guid? ActiveDirectoryDomainKey { get; set; }

        /// <summary>
        /// Gets or sets the user ID that is either typed in or scanned.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a user account.
        /// </summary>
        public Guid? UserAccountKey { get; set; }
        
        /// <summary>
        /// Gets or sets the UTC date and time of an authentication event.
        /// </summary>
        public DateTime AuthenticationUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time of an authentication event.
        /// </summary>
        [Column("AuthenticationLocalDateTime")]
        public DateTime AuthenticationDateTime { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether is scuccessfully authenticated regardless
        /// of whether a witness to a sign-in subsequently authenticates.
        /// </summary>
        [Column("SuccessfulAuthenticationFlag")]
        public bool SuccessfullyAuthenticated { get; set; }

        /// <summary>
        /// Gets or sets the nonlocalizable code that identifies an authentication failure reason.
        /// </summary>
        public string AuthenticationFailureReasonInternalCode { get; set; }

        /// <summary>
        /// Gets the internal code that identifies an authentication method.
        /// </summary>
        public AuthenticationFailureReasonInternalCode? AuthenticationFailureReason
        {
            get { return AuthenticationFailureReasonInternalCode.FromNullableInternalCode<AuthenticationFailureReasonInternalCode>(); }
            set { AuthenticationFailureReasonInternalCode = value.ToInternalCode(); }
        }

        /// <summary>
        /// Gets or sets the value that indicates whether a user successfully signed in.
        /// </summary>
        [Column("SuccessfulSignInFlag")]
        public bool SuccessfullySignedIn { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time that a user signs out or is signed out.
        /// </summary>
        public DateTime? SignOutUtcDateTime { get; set; }

        /// <summary>
        /// Gets or sets the local date and time that a user signs out or is signed out.
        /// </summary>
        [Column("SignOutLocalDateTime")]
        public DateTime? SignOutDateTime { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates the sign out reason.
        /// </summary>
        public string SignOutReasonInternalCode { get; set; }

        /// <summary>
        /// Gets the value that indicates the sign out reason.
        /// </summary>
        public SignOutReasonInternalCode? SignOutReason
        {
            get { return SignOutReasonInternalCode.FromNullableInternalCode<SignOutReasonInternalCode>(); }
        }

        /// <summary>
        /// Gets or sets the surrogate key of an authentication event.
        /// </summary>
        public Guid? WitnessAuthenticationEventKey { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of a dispensing device.
        /// </summary>
        public Guid? DispensingDeviceKey { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether an authentication event
        /// is via a web browser.
        /// </summary>
        [Column("WebBrowserFlag")]
        public bool IsWebBrowser { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a user signed in under a switch-user
        /// scenario; the value is never set for witness authentication events.
        /// </summary>
        [Column("SwitchUserOnSignInFlag")]
        public bool SwitchUserOnSignIn { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a user signed out at a dispensing device
        /// with the intent to switch user.
        /// </summary>
        [Column("SwitchUserOnSignOutFlag")]
        public bool SwitchUserOnSignOut { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether a user was authenticated by scan.
        /// </summary>
        [Column("UserScanFlag")]
        public bool UserScan { get; set; }

        /// <summary>
        /// Gets or sets the IP address of an accessing machine.
        /// </summary>
        [Column("AccessingAddressValue")]
        public string AccessingAddress { get; set; }

        /// <summary>
        /// Gets or sets the name of an accessing machine
        /// </summary>
        public string AccessingMachineName { get; set; }

        /// <summary>
        /// Gets or sets the nonlocalizable code that identifies an application.
        /// </summary>
        public string SystemApplicationInternalCode { get; set; }

        /// <summary>
        /// Gets or sets the internal code that identifies the result of the system authenticating a user
        /// </summary>
        public string AuthenticationResultInternalCode { get; set; }

        /// <summary>
        /// Gets or sets the value that indicates whether no card reader detected during Card + PIN authentication.
        /// </summary>
        [Column("CardReaderNotDetectedFlag")]
        public bool CardReaderNotDetected { get; set; }

        /// <summary>
        /// Gets or sets the binary value that is unique within a database and that is assigned a new
        /// value on insert and on update.
        /// </summary>
        [Column("LastModifiedBinaryValue")]
        public byte[] LastModified { get; set; }

        #endregion

        #region Public Members

        public bool IsTransient()
        {
            return Key == default(Guid);
        }

        #endregion
    }
}
