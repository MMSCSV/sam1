using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using CareFusion.Dispensing.Models;
using CareFusion.Dispensing.Server.Contracts;

namespace CareFusion.Dispensing.Contracts
{
    public class AuthenticationContext
    {
        public bool MatchScanCode { get; set; }
        public Context Context { get; set; }
        public TokenCredentials TokenCredentials { get; set; }
        public AuthUserAccount AuthUserAccount { get; set; }
    }

    /// <summary>
    /// Represents Device-only authentication API's.
    /// </summary>
    public interface IDeviceAuthenticationService : IAuthenticationService
    {
        [Obsolete("Use the asynchronous method 'AuthenticateAsync' instead.")]
        AuthenticationResult Authenticate(Context context, TokenCredentials credentials, bool matchScanCode);
        [Obsolete("Use the asynchronous method 'AuthenticateAsync' instead.")]
        AuthenticationResult Authenticate(Context context, TokenCredentials credentials, AuthUserAccount userAccount);
        Task<AuthenticationResult> AuthenticateAsync(Context context, TokenCredentials credentials, bool matchScanCode, CancellationToken cancellationToken = default);
        Task<AuthenticationResult> AuthenticateAsync(Context context, TokenCredentials credentials, AuthUserAccount userAccount, CancellationToken cancellationToken = default);
        Task<AuthenticationResult> AuthenticateSmartCardUserAsync(Context context, TokenCredentials credentials, X509Certificate2 smartCardCert, CancellationToken cancellationToken = default);
        Task<AuthenticationResult> ValidateSmartCardCertificateAsync(Context context, TokenCredentials credentials, X509Certificate2 smartCardCert, CancellationToken cancellationToken = default);

        [Obsolete("Use the asynchronous method 'VerifyUserAccountStatusAsync' instead.")]
        AuthenticationResult VerifyUserAccountStatus(Context context, TokenCredentials credentials, bool matchScanCode);
        [Obsolete("Use the asynchronous method 'VerifyUserAccountStatusAsync' instead.")]
        AuthenticationResult VerifyUserAccountStatus(Context context, AuthUserAccount userAccount, TokenCredentials credentials);
        Task<AuthenticationResult> VerifyUserAccountStatusAsync(Context context, TokenCredentials credentials, bool matchScanCode, CancellationToken cancellationToken = default);
        Task<AuthenticationResult> VerifyUserAccountStatusAsync(Context context, AuthUserAccount userAccount, TokenCredentials credentials, CancellationToken cancellationToken = default);
        void SignOutViaSwitchUser(Context context, Guid signInEventKey);
        //TODO: move to base IAuthenticationService if Web can pass UserAccount too
        void ChangePassword(Context context, AuthUserAccount userAccount, string oldPwd, string newPwd);
        void RegisterFingerprint(Context context, UserFingerprint userFingerprint);
        UserFingerprint GetUserFingerprintTemplates(Guid userAccountKey);
        Guid LogAuthenticationEvent(AuthenticationEvent authenticationEvent);
        void UpdateAuthenticationEvent(
            Guid authenticationEventKey,
            Guid witnessEventKey,
            DateTime authLocalDatetime,
            DateTime authUtcDatetime,
            bool success);
        void UpdateUserAccountLastSuccessfulPasswordAuthentication(Guid? deviceKey, Guid userAccountKey );
        AuthUserAccount GetUserAccount(Guid userAccountKey);
        AuthUserAccount GetUserAccount(string cardSerialID);
        AuthUserAccount GetUserAccountByRFIDCardSerialID(string rfidCardSerialID);
        void EnrollCardSerialID(Context context, Guid userAccountKey, string cardSerialID);
        void EnrollRFIDCardSerialID(Context context, Guid userAccountKey, string rfidCardSerialID);
        bool IsDodDisclaimerNeeded();
        DispensingSystem GetDispensingSystemSettings();
        IEnumerable<ActiveDirectoryDomain> GetActiveDirectoryDomains();
        RecentUserAccountAuthenticationAttemptData GetPastAuthenticationDetails(Context context, Guid userKey, Guid ignoreAuthenticationEventKey);
    }
}
