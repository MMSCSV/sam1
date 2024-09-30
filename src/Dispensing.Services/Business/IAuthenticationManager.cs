using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Models;
using Pyxis.Core.Data.InternalCodes;
using Pyxis.IdentityServer.Middleware.Models;

namespace CareFusion.Dispensing.Services.Business
{
    internal interface IAuthenticationManager
    {
        Task<AuthenticationResult> AuthenticateUserAsync(Context context, AuthUserAccount userAccount, TokenCredentials credentials, CancellationToken cancellationToken);

        Task<AuthenticationResult> AuthenticateSmartCardUserAsync(Context context, TokenCredentials credentials, AuthUserAccount userAccount, X509Certificate2 smartCardCert, CancellationToken cancellationToken);
        Task<AuthenticationResult> ValidateSmartCardCertificateAsync(Context context, TokenCredentials credentials, X509Certificate2 smartCardCert, CancellationToken cancellationToken = default);

        /// <summary>Unlike <see cref="VerifyUserAccountStatusAsync"/>, this method goes to DB only and not to IDS</summary>
        AuthenticationResult VerifyUserAccountStatus(Context context, AuthUserAccount userAccount);

        Task<AuthenticationResult> VerifyUserAccountStatusAsync(Context context, AuthUserAccount userAccount, TokenCredentials credentials, CancellationToken cancellationToken);

        Task<UserAccountResponse> GetUserAccountFromIdentityServerAsync(string accessToken, CancellationToken cancellationToken);

        void SignOutUser(Context context, Guid signInEventKey, SignOutReasonInternalCode signOutReason);

        void SignOutViaSwitchUser(Context context, Guid signInEventKey);

        void ChangeUserPassword(Context context, string oldPwd, string newPwd, AuthUserAccount userAccount);

        IEnumerable<string> ListPasswordRules(AuthUserAccount userAccount);

        Guid LogAuthenticationEvent(AuthenticationEvent authEvent);

        void UpdateWitnessAuthenticationEvent(Guid authenticationEventKey, Guid witnessEventKey, DateTime authLocalDatetime, DateTime authUtcDatetime, bool success);

        AuthUserAccount GetAuthUserAccount(Guid userAccountKey);

        AuthUserAccount GetAuthUserAccount(string cardSerialID);

        AuthUserAccount GetAuthUserAccountByRFIDCardSerialID(string rfidCardSerialID);

        AuthUserAccount GetAuthUserAccount(string domain, string userId);

        IEnumerable<AuthUserAccount> GetAuthUserAccount(string userId, bool matchScanCode = false);

        IEnumerable<ActiveDirectoryDomain> GetActiveDirectoryDomains();

        int GetPasswordExpirationDuration(AuthUserAccount userAccount);

        bool IsDodDisclaimerNeeded();

        DispensingSystem GetDispensingSystemSettings();

        void UpdateLastPwdExpirationNotice(Context context, AuthUserAccount userAccount);

        void UpdateUserAccountLastSuccessfulPasswordAuthentication(Guid? deviceKey, Guid userAccountKey);

        void EnrollCardSerialID(Context context, Guid userKey, string cardSerialId);

        void EnrollRFIDCardSerialID(Context context, Guid userKey, string rfidCardSerialID);

        ActiveDirectoryDomain GetActiveDirectoryDomain(Guid activeDirectoryKey);

        AuthenticationResult GetUserForDomainAppendedUserId(string userId);

        Guid UpsertSupportUserAccount(Context context, UserAccount supportUser);

        AuthenticationEvent GetLastSuccessfulAuthenticationEvent(Guid userAccountKey);
    }
}
