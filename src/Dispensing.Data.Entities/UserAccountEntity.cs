using System;
using System.Linq;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class UserAccountEntity : IContractConvertible<UserAccount>
    {
        #region IContractConvertible<UserAccount> Members

        public UserAccount ToContract()
        {
            Guid[] facilityKeys = UserAccountFacilityEntities
                    .Select(uaf => uaf.FacilityKey)
                    .ToArray();

            // Exclude temporary roles from the user account, this is managed separately.
            UserRole[] userRoles = UserRoleMemberEntities
                    .Where(urm => urm.MedTemporaryAccessFlag == false)
                    .Select(urm => urm.ToContract())
                    .ToArray();

            ActiveDirectoryDomain activeDirectoryDomain = null;
            if (ActiveDirectoryDomainKey.HasValue)
            {
                activeDirectoryDomain = (ActiveDirectoryDomainEntity != null)
                    ? new ActiveDirectoryDomain(ActiveDirectoryDomainEntity.Key)
                          {
                              Name = ActiveDirectoryDomainEntity.ShortDomainName,
                              FullyQualifiedName = ActiveDirectoryDomainEntity.FullyQualifiedDomainName,
                              ControllerAddress = ActiveDirectoryDomainEntity.DomainControllerAddressValue,
                              SystemAccountId = ActiveDirectoryDomainEntity.SystemAccountID,
                              SystemAccountPasswordEncrypted = ActiveDirectoryDomainEntity.SystemAccountPasswordEncryptedValue,
                              PollingDuration = ActiveDirectoryDomainEntity.PollingDurationAmount,
                              IsActive = ActiveDirectoryDomainEntity.ActiveFlag,
                              InvocationId = ActiveDirectoryDomainEntity.InvocationID,
                              HighestCommittedUsn = ActiveDirectoryDomainEntity.HighestCommittedUSNValue,
                              LastSuccessfulPollDateTime = ActiveDirectoryDomainEntity.LastSuccessfulPollLocalDateTime,
                              LastSuccessfulPollUtcDateTime = ActiveDirectoryDomainEntity.LastSuccessfulPollUTCDateTime,
                              Workgroup = ActiveDirectoryDomainEntity.WorkgroupFlag,
                              LastPollStatusInternalCode = ActiveDirectoryDomainEntity.LastPollStatusInternalCode,
                              SecuredCommunication = ActiveDirectoryDomainEntity.SecuredCommunicationFlag,
                              UserDirectoryTypeInternalCode = ActiveDirectoryDomainEntity.UserDirectoryTypeInternalCode,
                              LastModified = LastModifiedBinaryValue.ToArray()
                          }
                    : ActiveDirectoryDomainKey.Value;
            }

            // There should only be ONE current password credential per user account.
            PasswordCredential passwordCredential = PasswordCredentialEntities
                .Select(pc => pc.ToContract())
                .SingleOrDefault();

            return new UserAccount(Key, UserAccountSnapshotKey)
            {
                LastPasswordExpirationNoticeDateTime = LastPasswordExpirationNoticeLocalDateTime,
                LastPasswordExpirationNoticeUtcDateTime = LastPasswordExpirationNoticeUTCDateTime,
                LastSuccessfulPasswordAuthenticationDateTime = LastSuccessfulPasswordAuthenticationLocalDateTime,
                LastSuccessfulPasswordAuthenticationUtcDateTime = LastSuccessfulPasswordAuthenticationUTCDateTime,
                LastAreaKey = LastAreaKey,
                AccountExpirationDate = AccountExpirationLocalDateTime,
                AccountExpirationUtcDate = AccountExpirationUTCDateTime,
                ActiveDirectoryDomain = activeDirectoryDomain,
                ActiveDirectoryObjectGuid = ActiveDirectoryObjectGloballyUniqueID,
                ContactInformation = ContactInformation,
                DirectoryChangePasswordDateTime = DirectoryChangePasswordLocalDateTime,
                DirectoryChangePasswordUtcDateTime = DirectoryChangePasswordUTCDateTime,
                EmailAddress = EmailAddressValue,
                FullName = FullName,
                FacilityKeys = facilityKeys,
                FirstName = FirstName,
                Initials = InitialsText,
                IsActive = ActiveFlag,
                IsFingerprintExempt = FingerprintExemptFlag,
                IsCardPinExempt = CardPINExemptFlag,
                IsLocked = LockedFlag,
                IsTemporary = TemporaryFlag,
                JobTitle = JobTitleText, 
                LastName = LastName,
                MiddleName = MiddleName,
                ScanCode = ScanCodeValue,
                Suffix = SuffixText,
                PasswordCredential = passwordCredential,
                UserRoles = userRoles,
                UserId = UserID,
                IsSuperUser = SuperUserFlag,
                IsSupportUser = SupportUserFlag,
                UserTypeKey = UserTypeKey,
                CardSerialId=CardSerialID,
                RFIDCardSerialID = RFIDCardSerialID,
                LastModified = LastModifiedBinaryValue.ToArray()
            };
        }

        #endregion
    }
}
