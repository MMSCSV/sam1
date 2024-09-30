using System;
using System.Diagnostics;
using System.Threading;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Services.Business;
using Pyxis.Core.Data.InternalCodes;
using Mms.Logging;

namespace CareFusion.Dispensing.Services
{
    public class WebAuthenticationService : BaseAuthenticationService, IWebAuthenticationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public WebAuthenticationService()
            : this(new WebAuthenticationManager())
        {
        }

        internal WebAuthenticationService(IAuthenticationManager authManager)
            : base(authManager)
        {
        }

        #region IWebAuthenticationService Members

        AuthenticationResult IWebAuthenticationService.Authenticate(Context context, TokenCredentials credentials)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(credentials, "credentials");

            credentials.UserId = credentials.UserId.Trim();

            AuthenticationResult result = null;

            try
            {
                var sw = Stopwatch.StartNew();

                using (new RepositorySessionScope())
                {
                    result = GetAuthUserAccountById(context, credentials.UserId);

                    //If user is found, but, it failed authentication due to other reasons such as inactive domain, multi domains result, etc.,
                    //then fail authentication early. Otherwise, this could be a support user first time login, therefore, let IDS
                    //handle it.
                    //TODO: move this logic to IDS, and only do this check if IDS is not reachable.
                    if (result.ResultCode != AuthenticationResultCode.NotFound &&
                        result.ResultCode != AuthenticationResultCode.Successful)
                    {
                        //Log Authentication Event for invalid UserId
                        LogAuthenticationEventForInvalidUserId(
                            context,
                            credentials.UserId,
                            credentials.AuthenticationMethod.InternalCode,
                            credentials.AuthenticationPurpose);
                        return result;
                    }

                    result = AuthenticationManager.AuthenticateUserAsync(context, result.AuthUserAccount, credentials, CancellationToken.None).Result;

                    //If support user is trying to login for the first time, then get the newly created
                    //support user cache record by IDS from DB.
                    if (result.AuthUserAccount != null &&
                        result.AuthUserAccount.IsSupportUser &&
                        result.AuthUserAccount.Key == Guid.Empty)
                    {
                        result.AuthUserAccount = GetAuthUserAccountById(context, credentials.UserId).AuthUserAccount;
                    }

                    //If we have reached this far and still dont have a user account, then we have an invalid user.
                    //Log the auth event to the invalid user
                    if (result.AuthUserAccount == null)
                    {
                        LogAuthenticationEventForInvalidUserId(
                            context,
                            credentials.UserId,
                            credentials.AuthenticationMethod.InternalCode,
                            credentials.AuthenticationPurpose);
                        return result;
                    }
                    else
                    {
                        result.SignInEventKey = LogAuthenticationEvent(context, result.AuthUserAccount, result,
                            credentials.AuthenticationMethod.InternalCode, credentials.AuthenticationPurpose);
                    }
                }

                Log.Debug("Authenticate() took {0} milliseconds to complete.", sw.ElapsedMilliseconds);
            }
            catch (Exception e)
            {
                Log.Debug("Authenticate() failed");

                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }

            return result;
        }

        private AuthenticationEvent GetLastSuccessfulAuthenticationEvent(Guid userAccountKey)
        {
            if (userAccountKey == null)
            {
                throw new Exception("UserAccountKey cannot be null to retrieve last Authentication Event!");
            }

            return AuthenticationManager.GetLastSuccessfulAuthenticationEvent(userAccountKey);
        }

        AuthUserAccount IWebAuthenticationService.GetAuthUserAccount(Guid userAccountKey)
        {
            return AuthenticationManager.GetAuthUserAccount(userAccountKey);
        }

        void IWebAuthenticationService.ChangePassword(Context context, Guid userAccountKey, string oldPwd, string newPwd)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userAccountKey, "userAccountKey");
            Guard.ArgumentNotNullOrEmptyString(oldPwd, "oldPwd");
            Guard.ArgumentNotNullOrEmptyString(newPwd, "newPwd");

            Stopwatch sw = Stopwatch.StartNew();

            using (new RepositorySessionScope())
            {
                //First check if user exists in local database
                AuthUserAccount currentUserAccount = AuthenticationManager.GetAuthUserAccount(userAccountKey);
                if (currentUserAccount == null)
                {
                    Log.Debug(string.Format("No user found by user key :{0}", userAccountKey));
                    throw new ServiceException(ValidationStrings.SignInFailure_InvalidUserId);
                }

                AuthenticationManager.ChangeUserPassword(context, oldPwd, newPwd, currentUserAccount);
            }

            Log.Debug("ChangePassword() took {0} milliseconds to complete.", sw.ElapsedMilliseconds);
        }

        #endregion

        #region Private Methods

        private void LogAuthenticationEventForInvalidUserId(
            Context context,
            string userId,
            AuthenticationMethodInternalCode code,
            AuthenticationPurposeInternalCode purpose)
        {

            AuthenticationEvent authEvent = new AuthenticationEvent()
            {
                AuthenticationMethodInternalCode = code.ToInternalCode(),
                UserId = userId,
                AuthenticationDateTime = context.ActionDateTime,
                AuthenticationUtcDateTime = context.ActionUtcDateTime,
                IsWebBrowser = true,
                AuthenticationPurposeInternalCode = purpose.ToInternalCode(),
                DispensingDeviceKey = null,
                SuccessfullyAuthenticated = false,
                SuccessfullySignedIn = false,
                AccessingAddress = context.AccessingAddress,
                AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.InvalidCredential,
                SystemApplicationInternalCode = SystemApplicationInternalCode.ServerWeb.ToInternalCode()
            };

            AuthenticationManager.LogAuthenticationEvent(authEvent);
        }

        private Guid LogAuthenticationEvent(
            Context context,
            AuthUserAccount userAccount,
            AuthenticationResult authenticationResult,
            AuthenticationMethodInternalCode code,
            AuthenticationPurposeInternalCode purpose)
        {
            if (userAccount == null)
            {
                throw new Exception("UserAccount cannot be null while logging AuthenticationEvent!");
            }

            if (userAccount.Key == Guid.Empty)
            {
                //Trying to Log after Failed New Support User Log in
                return default(Guid);
            }

            AuthenticationEvent authEvent = new AuthenticationEvent()
            {
                AuthenticationMethodInternalCode = code.ToInternalCode(),
                ActiveDirectoryDomainKey = userAccount.ActiveDirectoryDomainKey,
                UserId = userAccount.UserId,
                UserAccountKey = userAccount.Key,
                AuthenticationDateTime = context.ActionDateTime,
                AuthenticationUtcDateTime = context.ActionUtcDateTime,
                IsWebBrowser = true,
                AuthenticationPurposeInternalCode = purpose.ToInternalCode(),
                DispensingDeviceKey = null,
                AccessingAddress = context.AccessingAddress,
                AuthenticationFailureReason = authenticationResult.AuthenticationFailureReason,
                AuthenticationResultInternalCode = (authenticationResult.ResultCode.ToString() == AuthenticationResultInternalCodes.Successful) ?
                                                    AuthenticationResultInternalCodes.Successful : (authenticationResult.ResultCode.ToString() == AuthenticationResultCode.WarnAccountLockout.ToString()) ?
                                                    AuthenticationResultInternalCodes.NextFailLock : (authenticationResult.ResultCode.ToString() == AuthenticationResultCode.AccountLocking.ToString()) ?
                                                    AuthenticationResultInternalCodes.AccountLocked : authenticationResult.ResultCode.ToString(),
                SystemApplicationInternalCode = SystemApplicationInternalCode.ServerWeb.ToInternalCode()
            };

            if (authenticationResult.ResultCode == AuthenticationResultCode.Successful
                || authenticationResult.ResultCode == AuthenticationResultCode.ChangePasswordRequired)
            {
                authEvent.SuccessfullyAuthenticated = true;
                authEvent.SuccessfullySignedIn = (purpose == AuthenticationPurposeInternalCode.SI) || (purpose == AuthenticationPurposeInternalCode.UDCONV);
            }
            else
            {
                authEvent.SuccessfullyAuthenticated = false;
                authEvent.SuccessfullySignedIn = false;
            }

            return AuthenticationManager.LogAuthenticationEvent(authEvent);
        }

        #endregion
    }
}
