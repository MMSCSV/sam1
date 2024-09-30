using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents web-only authentication API's.
    /// </summary>
    public interface IWebAuthenticationService : IAuthenticationService
    {
        AuthenticationResult Authenticate(Context context, TokenCredentials credentials);

        void ChangePassword(Context context, Guid userAccountKey, string oldPassword, string newPassword);

        AuthUserAccount GetAuthUserAccount(Guid userAccountKey);
    }
}
