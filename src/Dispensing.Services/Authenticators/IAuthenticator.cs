using System.Collections.Generic;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Services.Authenticators
{
    public interface IAuthenticator
    {
        AuthenticationResult AuthenticateUser(Context context, Credentials credentials, AuthUserAccount userAccount);

        AuthenticationResult VerifyUser(Context context, AuthUserAccount userAccount);
        
        IEnumerable<string> ListPasswordRules();

        void ChangePassword(Context context, AuthUserAccount userAccount, string oldPwd, string newPwd);

        int GetMaxPwdAge();
    }
}
