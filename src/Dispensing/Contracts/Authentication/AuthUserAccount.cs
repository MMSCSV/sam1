using System;

namespace CareFusion.Dispensing.Contracts
{
    public class AuthUserAccount
    {
        public AuthUserAccount()
        {
                
        }
        
        public Guid Key { get; set; }

        public DateTime? LastPasswordExpirationNoticeUtcDateTime { get; set; }

        public DateTime? LastSuccessfulPasswordAuthenticationUtcDateTime { get; set; }

        public Guid SnapshotKey { get; set; }

        public Guid? UserTypeKey { get; set; }

        public Guid? ActiveDirectoryDomainKey { get; set; }

        public DateTime? AccountExpirationUtcDate { get; set; }

        public string UserId { get; set; }

		public string FirstName { get; set; }

		public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string InitialsText { get; set; }

        public DateTime? DirectoryChangePasswordUtcDateTime { get; set; }

        public bool IsSupportUser { get; set; }

        //True, if user is part of a support domain (ie. BD Azure AD). False, otherwise.
        public bool IsSupportDomainUser { get; set; }

        public bool IsSuperUser { get; set; }

        public bool IsTemporary { get; set; }

        public bool IsFingerprintExempt { get; set; }

        public bool IsCardPinExempt { get; set; }

        public string CardSerialId { get; set; }

        public string RFIDCardSerialID { get; set; }

        public bool IsActive { get; set; }

        public bool IsLocked { get; set; }

        public string ScanCode { get; set; }

        public bool IsDomainAccount
        {
            get { return (ActiveDirectoryDomainKey != null); }
        }

        public bool IsCfSupportUser
        {
            get{return (UserId != null && UserId.Equals(AuthenticationConstants.CFSupport, StringComparison.CurrentCultureIgnoreCase));}
        }
        
    }
}
