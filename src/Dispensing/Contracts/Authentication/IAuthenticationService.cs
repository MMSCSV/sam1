using System;
using System.Collections.Generic;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents all common authentication API's
    /// </summary>
    public interface IAuthenticationService
    {
        void SignOutUser(Context context, Guid signInEventKey, bool abnormal=false);

        void TimeoutUser(Context context, Guid signInEventKey);

        void PowerFailUser(Context context, Guid signInEventKey);

        IEnumerable<string> ListPasswordRules(Guid userAccountKey);

        /// <summary>
        /// Return number of days remaining before password getting expired. If number of days is greater than
        /// PasswordExpirationNoticeInterval setting then it returns MAX of int.
        /// </summary>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        int PasswordExpirationDuration(AuthUserAccount userAccount);

        void UpdateLastPasswordExpirationNotice(Context context, AuthUserAccount userAccount);
    }
}
