using System;
using System.Collections.Generic;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Resources.Common;
using CareFusion.Dispensing.Services.Business;
using Pyxis.Core.Data.InternalCodes;
using Mms.Logging;

namespace CareFusion.Dispensing.Services
{
    public class BaseAuthenticationService : IAuthenticationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IAuthenticationManager _authenticationManager;

        internal BaseAuthenticationService(IAuthenticationManager authenticationManager)
        {
            _authenticationManager = authenticationManager;
        }

        internal IAuthenticationManager AuthenticationManager
        {
            get { return _authenticationManager; }
        }

        #region IAuthenticationService Members

        void IAuthenticationService.SignOutUser(Context context, Guid signInEventKey, bool abnormal)
        {
            try
            {
                Guard.ArgumentNotNull(context, "context");

                using (new RepositorySessionScope())
                {
                    AuthenticationManager.SignOutUser(context, signInEventKey, abnormal ? SignOutReasonInternalCode.ABNORMAL : SignOutReasonInternalCode.MSO);
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IAuthenticationService.TimeoutUser(Context context, Guid signInEventKey)
        {
            try
            {
                Guard.ArgumentNotNull(context, "context");

                using (new RepositorySessionScope())
                {
                    AuthenticationManager.SignOutUser(context, signInEventKey, SignOutReasonInternalCode.TO);
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IAuthenticationService.PowerFailUser(Context context, Guid signInEventKey)
        {
            try
            {
                Guard.ArgumentNotNull(context, "context");

                using (new RepositorySessionScope())
                {
                    AuthenticationManager.SignOutUser(context, signInEventKey, SignOutReasonInternalCode.PF);
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }
        }

        IEnumerable<string> IAuthenticationService.ListPasswordRules(Guid userAccountKey)
        {
            IEnumerable<string> rules = Enumerable.Empty<string>();

            try
            {
                using (new RepositorySessionScope())
                {
                    AuthUserAccount userAccount = AuthenticationManager.GetAuthUserAccount(userAccountKey);
                    if (userAccount == null)
                        return Enumerable.Empty<string>();

                    rules = AuthenticationManager.ListPasswordRules(userAccount);
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }

            return rules;
        }

        int IAuthenticationService.PasswordExpirationDuration(AuthUserAccount userAccount)
        {
            int duration = int.MaxValue;
            //if its SupportUser then we dont display pwd expiry notification
            if (userAccount.IsSupportUser)
                return int.MaxValue;
            try
            {
                using (new RepositorySessionScope())
                {
                    duration = AuthenticationManager.GetPasswordExpirationDuration(userAccount);
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }
            return duration;
        }

        void IAuthenticationService.UpdateLastPasswordExpirationNotice(Context context, AuthUserAccount userAccount)
        {
            try
            {
                using (new RepositorySessionScope())
                {
                    AuthenticationManager.UpdateLastPwdExpirationNotice(context, userAccount);
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion

        #region Protected Members

        protected AuthenticationResult GetAuthUserAccountById(Context context, string enteredUsername, bool matchScanCode = false)
        {
            enteredUsername = enteredUsername?.ToLowerInvariant();

            //check if user id has domain appended
            if (enteredUsername.Contains('\\') || enteredUsername.Contains('@'))
            {
                return AuthenticationManager.GetUserForDomainAppendedUserId(enteredUsername);
            }

            var uAccounts = AuthenticationManager.GetAuthUserAccount(enteredUsername, matchScanCode);

            var userAccounts = uAccounts.ToArray();
            if (!userAccounts.Any())
            {
                return new AuthenticationResult(AuthenticationResultCode.NotFound)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.InvalidCredential,
                    ErrorMessage = ServiceResources.SignInFailure_InvalidUserIdPassword
                };
            }

            // Only one User Account
            if (userAccounts.Count() == 1)
            {
                return new AuthenticationResult(AuthenticationResultCode.Successful)
                {
                    AuthUserAccount = userAccounts.First()
                };
            }

            // More than one User Account for this ID 
            // Need to resolve
            var domains = _authenticationManager.GetActiveDirectoryDomains();

            // Get the list of Domains for this User 
            var userDomainList = (from ua in userAccounts
                                  join ad in domains
                                  on ua.ActiveDirectoryDomainKey equals ad.Key
                                  select ad).ToArray();

            // Get only Active Domains
            var fD = userDomainList.Where(ad => ad.IsActive.Equals(true)).Select(ad => ad);
            var filteredDomainList = fD.ToArray();

            // User doesn't belong to any Active Domains or have a local account
            if (!filteredDomainList.Any() && !userAccounts.Any(ua => ua.IsDomainAccount.Equals(false)))
            {
                return new AuthenticationResult
                {
                    ResultCode = AuthenticationResultCode.InactiveDomain,
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.InactiveDomain,
                    ErrorMessage = AuthenticationResources.InactiveDomainAuthenticationError,
                    AuthUserAccount = userAccounts.FirstOrDefault()
                };
            }

            // Miltiple Active Domains or At least one ActiveDomain and a active local account
            // User must select account
            if (filteredDomainList.Count() > 1 ||
                (filteredDomainList.Any() && userAccounts.Any(ua => ua.IsDomainAccount.Equals(false) && ua.IsActive.Equals(true))))
            {
                //filter out the inactive local user account
                var localInactiveAuthUserAccounts =
                       userAccounts.Where(ua => ua.IsDomainAccount.Equals(false) && ua.IsActive.Equals(false));
                return CreateMultipleUserAuthenticationResult(userAccounts.Except(localInactiveAuthUserAccounts),
                       context, filteredDomainList);
            }

            // One Active Domain - Retrieve the corresponding User Account
            if (filteredDomainList.Count() == 1)
            {
                return new AuthenticationResult(AuthenticationResultCode.Successful)
                {
                    AuthUserAccount = userAccounts.First(ua => ua.ActiveDirectoryDomainKey.Equals(filteredDomainList.FirstOrDefault().Key))
                };
            }

            // Only local account, No Active Domains - Retrieve the corresponding local User Account
            return new AuthenticationResult(AuthenticationResultCode.Successful)
            {
                AuthUserAccount = userAccounts.First(ua => ua.IsDomainAccount.Equals(false))
            };
        }

        protected string GetFullyQualifiedUserId(AuthUserAccount userAccount)
        {
            var userId = userAccount.UserId.Trim();

            if (userAccount.ActiveDirectoryDomainKey is null ||
                userAccount.ActiveDirectoryDomainKey == Guid.Empty ||
                userId.Contains('\\') ||
                userId.Contains('@'))
            {
                return userId;
            }

            var domainName = _authenticationManager.GetActiveDirectoryDomain(userAccount.ActiveDirectoryDomainKey.Value)?.FullyQualifiedName;

            if (string.IsNullOrEmpty(domainName))
                return userId;

            userId = $"{userId}@{domainName}";

            Log.Debug($"Userid appended with domain name:{userId}");

            return userId;
        }
        #endregion

        #region Private Members

        MultipleUserAuthenticationResult CreateMultipleUserAuthenticationResult(IEnumerable<AuthUserAccount> userAccounts, Context context, IEnumerable<ActiveDirectoryDomain> filteredDomainList)
        {
            //Get complete domain list
            return new MultipleUserAuthenticationResult(AuthenticationResultCode.MultipleUserId)
            {
                //diff error msg for both web and device application
                ErrorMessage = context.Device != null
                                    ? ServiceResources.
                                        SignInFailure_DeviceMultipleUserAccount
                                    : string.Format(ServiceResources.SignInFailure_WebMultipleUserAccount, AuthenticationResources.NonDomainAccount),
                DomainList = filteredDomainList,
                MultipleAuthUserAccount = userAccounts
            };
        }
        #endregion

        protected Guid UpsertSupportUserAccount(Context context, UserAccount userAccount)
        {
            Guard.ArgumentNotNull(context, nameof(context));
            Guard.ArgumentNotNull(userAccount, nameof(userAccount));

            using (new RepositorySessionScope())
            {
                return AuthenticationManager.UpsertSupportUserAccount(context, userAccount);
            }
        }

        protected AuthUserAccount GetAuthUserAccountByKey(Guid key)
        {
            Guard.ArgumentNotNull(key, nameof(key));

            using (new RepositorySessionScope())
            {
                return AuthenticationManager.GetAuthUserAccount(key);
            }
        }
        protected void UpdateTokenCredentials(TokenCredentials credentials, AuthUserAccount authUserAccount, bool matchScanCode)
        {
            // Append Domain to User Id if it is a domain account
            if (authUserAccount.IsDomainAccount &&
                !(credentials.UserId.Contains("\\") ||
                credentials.UserId.Contains("@")))
            {
                Log.Debug($"The auth user:{authUserAccount.UserId} is domain user.Add domain name to user id for ids to distinguish AD/local user with same userid.");
               
                 credentials.UserId = GetFullyQualifiedUserId(authUserAccount);
            }
            // When UserId is matched by scan code, re-assigning the UserId to token credentials for authentication
            else if (matchScanCode)
            {
                Log.Debug($"user is matched by scan id {credentials.UserId} user id {authUserAccount.UserId}");
                credentials.UserId = authUserAccount.UserId;
            }
        }

    }
}
