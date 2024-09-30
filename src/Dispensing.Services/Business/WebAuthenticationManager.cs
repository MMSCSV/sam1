using System;
using System.Threading;
using System.Threading.Tasks;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data.Repositories;

namespace CareFusion.Dispensing.Services.Business
{
    internal class WebAuthenticationManager : BaseAuthenticationManager
    {
        public WebAuthenticationManager()
        {

        }

        public WebAuthenticationManager(IDispensingSystemRepository dispensingSystemRepository, IAuthenticationEventRepository authenticationEventRepository,
            IIdentityServerAuthenticationManager identityServerAuthenticationManager)
            : base(dispensingSystemRepository, authenticationEventRepository, identityServerAuthenticationManager)
        {

        }

        #region IAuthenticationManager Members

        protected override AuthenticationResult VerifyUserAccountStatus(Context context, AuthUserAccount userAccount)
        {
            //for web app there is not requirement of this api
            throw new NotSupportedException();
        }

        protected override bool IsSystemDoD()
        {
            //for web app there is no requirement of this api
            throw new NotSupportedException();
        }

        protected override Task<AuthenticationResult> AuthenticateUserAsync(Context context, AuthUserAccount userAccount, TokenCredentials credentials, CancellationToken cancellationToken)
        {
            var authResult = base.AuthenticateUserAsync(context, userAccount, credentials, cancellationToken).Result;

            if(authResult.AuthUserAccount == null || authResult.ResultCode != AuthenticationResultCode.Successful)
            {
                return Task.FromResult(authResult);
            }

            var dispensingSystem = GetDispensingSystem();
            if (dispensingSystem?.LockedNoAuthenticationDuration != null &&
                dispensingSystem.LockedNoAuthenticationDuration > 0)
            {
                var authenticationEvent =
                    AuthenticationEventRepository.GetLastSuccessfulAuthenticationEvent(userAccount.Key);

                if (authenticationEvent != null)
                {
                    var lockedUserResult = LockNoAuthenticationUserAccount(context, userAccount,
                        authenticationEvent.AuthenticationUtcDateTime,
                        dispensingSystem.LockedNoAuthenticationDuration);

                    if (lockedUserResult.ResultCode != AuthenticationResultCode.Successful)
                        return Task.FromResult(lockedUserResult);
                }
            }

            return Task.FromResult(authResult);
        }

        #endregion
    }
}
