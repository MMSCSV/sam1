using System;
using System.Collections.Generic;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents non-web authentication API's.
    /// </summary>
    public interface IUserAuthenticationService : IAuthenticationService
    {
        // TODO: Change to a different contract since this loads the UserAccount with partial data,
        // hence updating the user account will delete all roles since they are not loaded.
        IEnumerable<UserAccount> GetUserAccounts(string userId);

        // TODO: Change to a different contract since this loads the UserAccount with partial data,
        // hence updating the user account will delete all roles since they are not loaded.
        UserAccount GetUserAccount(Guid userAccountKey);

        AuthenticationResult SignInUser(Context context, Credentials credentials, UserAccount userAccount);

        AuthenticationResult SignInUser(Context context, Guid userAccountKey, Credentials credentials);

        void ChangePassword(Context context, UserAccount userAccount, string oldPwd, string newPwd);

        AuthenticationResult RegisterFingerprint(Context context, UserFingerprint userFingerprint);

        UserFingerprint GetUserFingerprintTemplates(Guid userAccountKey);

        Guid LogAuthenticationEvent(AuthenticationEvent authenticationEvent);

        void ChangePassword(Context context, Guid userAccountKey, string oldPwd, string newPwd);

        void UpdateAuthenticationEvent(
            Guid authenticationEventKey,
            Guid witnessEventKey,
            DateTime authLocalDatetime,
            DateTime authUtcDatetime,
            bool success);

        AuthenticationResult PreUserVerification(Context context, string userId);

        AuthenticationResult DomainUserVerification(Context context, string userId, Guid? domainKey);

        AuthenticationResult VerifyUserAccountStatus(Context context, UserAccount userAccount);
    }
}
