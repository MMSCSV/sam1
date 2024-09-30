using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CareFusion.Dispensing.Caching;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data;
using CareFusion.Dispensing.Data.Repositories;
using CareFusion.Dispensing.Models;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Resources.Common;
using CareFusion.Dispensing.Services.Authenticators;
using Pyxis.Core.Data.InternalCodes;
using Mms.Logging;
using Pyxis.IdentityServer.Middleware.Models;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace CareFusion.Dispensing.Services.Business
{
    internal abstract class BaseAuthenticationManager : IAuthenticationManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ObjectCache _cache = MemoryCache.Default;
        private readonly IDispensingSystemRepository _dispensingSystemRepository;
        private readonly IIdentityServerAuthenticationManager _identityServerAuthenticationManager;

        private static readonly List<AuthenticationResultCode> IdsIssues = new List<AuthenticationResultCode>
        {
            AuthenticationResultCode.NotFound,
            AuthenticationResultCode.IdentityServerUrlNotConfigured,
            AuthenticationResultCode.DomainError,
            AuthenticationResultCode.RequestTimedOut
        };

        protected BaseAuthenticationManager()
        {
            _dispensingSystemRepository = new DispensingSystemRepository();
            AuthenticationEventRepository = new AuthenticationEventRepository();
            _identityServerAuthenticationManager = new IdentityServerAuthenticationManager(new SystemConfigurationRepository());
        }

        protected BaseAuthenticationManager(IDispensingSystemRepository dispensingSystemRepository,
            IAuthenticationEventRepository authenticationEventRepository,
            IIdentityServerAuthenticationManager identityServerAuthenticationManager)
        {
            _dispensingSystemRepository = dispensingSystemRepository;
            AuthenticationEventRepository = authenticationEventRepository;
            _identityServerAuthenticationManager = identityServerAuthenticationManager;
        }

        protected IAuthenticationEventRepository AuthenticationEventRepository { get; }

        #region IAuthenticationManager Members

        Task<AuthenticationResult> IAuthenticationManager.AuthenticateUserAsync(Context context, AuthUserAccount userAccount, TokenCredentials credentials, CancellationToken cancellationToken)
        {
            return AuthenticateUserAsync(context, userAccount, credentials, cancellationToken);
        }

        Task<AuthenticationResult> IAuthenticationManager.AuthenticateSmartCardUserAsync(Context context, TokenCredentials credentials, AuthUserAccount userAccount, X509Certificate2 smartCardCert, CancellationToken cancellationToken = default)
        {
            return AuthenticateSmartCardUserAsync(context, credentials, userAccount, smartCardCert, cancellationToken);
        }
        Task<AuthenticationResult> IAuthenticationManager.ValidateSmartCardCertificateAsync(Context context, TokenCredentials credentials, X509Certificate2 smartCardCert, CancellationToken cancellationToken = default)
        {
            return ValidateSmartCardCertificateAsync(context, credentials, smartCardCert, cancellationToken);
        }

        void IAuthenticationManager.SignOutUser(Context context, Guid signInEventKey, SignOutReasonInternalCode signOutReason)
        {
            Guard.ArgumentNotNull(context, "context");

            AuthenticationEventRepository.UpdateAuthenticationEvent(context, signInEventKey, signOutReason);
        }

        void IAuthenticationManager.ChangeUserPassword(Context context, string oldPwd, string newPwd, AuthUserAccount userAccount)
        {
            ChangeClinicalUserPassword(context, oldPwd, newPwd, userAccount);
        }

        IEnumerable<string> IAuthenticationManager.ListPasswordRules(AuthUserAccount userAccount)
        {
            return ListPasswordRules(userAccount);
        }

        Guid IAuthenticationManager.LogAuthenticationEvent(AuthenticationEvent authEvent)
        {
            return LogAuthenticationEvent(authEvent);
        }

        void IAuthenticationManager.UpdateWitnessAuthenticationEvent(Guid authenticationEventKey, Guid witnessEventKey, DateTime authLocalDatetime, DateTime authUtcDatetime, bool success)
        {
            AuthenticationEventRepository.UpdateAuthenticationEvent(
                authenticationEventKey,
                authUtcDatetime,
                authLocalDatetime,
                success,
                witnessEventKey
                );
        }

        Task<AuthenticationResult> IAuthenticationManager.VerifyUserAccountStatusAsync(Context context, AuthUserAccount userAccount, TokenCredentials credentials, CancellationToken cancellationToken)
        {
            return VerifyUserAccountStatusAsync(context, userAccount, credentials, cancellationToken);
        }

        AuthenticationResult IAuthenticationManager.VerifyUserAccountStatus(Context context, AuthUserAccount userAccount)
        {
            return VerifyUserAccountStatus(context, userAccount);
        }

        AuthUserAccount IAuthenticationManager.GetAuthUserAccount(Guid userAccountKey)
        {
            return GetAuthUserAccount(userAccountKey);
        }

        AuthUserAccount IAuthenticationManager.GetAuthUserAccount(string cardSerialID)
        {
            return GetAuthUserAccount(cardSerialID);
        }

        AuthUserAccount IAuthenticationManager.GetAuthUserAccountByRFIDCardSerialID(string rfidCardSerialID)
        {
            return GetAuthUserAccountByRFIDCardSerialID(rfidCardSerialID);
        }

        AuthenticationEvent IAuthenticationManager.GetLastSuccessfulAuthenticationEvent(Guid userAccountKey)
        {
            return GetLastSuccessfulAuthenticationEvent(userAccountKey);
        }

        public void EnrollCardSerialID(Context context, Guid userAccountKey, string cardSerialId)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userAccountKey, "userAccountKey");
            Guard.ArgumentNotNullOrEmptyString(cardSerialId, "cardSerialId");

            try
            {
                using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
                {
                    //Check if user is re-Enrolling SerialID
                    UserAccount existingUserAccount = repository.GetUserAccount(userAccountKey);
                    if (existingUserAccount != null)
                    {
                        existingUserAccount.CardSerialId = cardSerialId;

                        repository.UpdateUserAccount(context, existingUserAccount);
                    }
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }

        }

        public void EnrollRFIDCardSerialID(Context context, Guid userAccountKey, string rfidCardSerialID)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userAccountKey, "userAccountKey");
            Guard.ArgumentNotNullOrEmptyString(rfidCardSerialID, "rfidCardSerialID");

            try
            {
                using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
                {
                    //Check if user is re-Enrolling RFID SerialID
                    UserAccount existingUserAccount = repository.GetUserAccount(userAccountKey);
                    if (existingUserAccount != null)
                    {
                        existingUserAccount.RFIDCardSerialID = rfidCardSerialID;

                        repository.UpdateUserAccount(context, existingUserAccount);
                    }
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }

        }

        AuthUserAccount IAuthenticationManager.GetAuthUserAccount(string domain, string userId)
        {
            return GetAuthUserAccount(domain, userId);
        }

        IEnumerable<AuthUserAccount> IAuthenticationManager.GetAuthUserAccount(string userId, bool matchScanCode = false)
        {
            return GetAuthUserAccount(userId, matchScanCode);
        }

        IEnumerable<ActiveDirectoryDomain> IAuthenticationManager.GetActiveDirectoryDomains()
        {
            return GetActiveDirectoryDomains();
        }

        ActiveDirectoryDomain IAuthenticationManager.GetActiveDirectoryDomain(Guid activeDirectoryKey)
        {
            return GetActiveDirectoryDomains().Where(d => d != null && d.Key == activeDirectoryKey).Select(d => d).SingleOrDefault();
        }

        bool IAuthenticationManager.IsDodDisclaimerNeeded()
        {
            return IsSystemDoD();
        }
        DispensingSystem IAuthenticationManager.GetDispensingSystemSettings()
        {
            return GetDispensingSystem();
        }

        void IAuthenticationManager.SignOutViaSwitchUser(Context context, Guid signInEventKey)
        {
            SignOutViaSwitchUser(context, signInEventKey);
        }

        int IAuthenticationManager.GetPasswordExpirationDuration(AuthUserAccount userAccount)
        {
            return GetPwdExpirationDuration(userAccount);
        }

        void IAuthenticationManager.UpdateLastPwdExpirationNotice(Context context, AuthUserAccount userAccount)
        {
            UpdatePwdExpirationNotice(context, userAccount);
        }

        void IAuthenticationManager.UpdateUserAccountLastSuccessfulPasswordAuthentication(Guid? deviceKey,
            Guid userAccountKey)
        {
            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                //Update LastSuccessfulPassword datetime
                repository.UpdateUserAccountLastSuccessfulPasswordAuthentication(deviceKey,
                    userAccountKey,
                    DateTime.UtcNow, DateTime.Now);
            }
        }

        AuthenticationResult IAuthenticationManager.GetUserForDomainAppendedUserId(string userId)
        {
            var originalUserId = userId;
            var domain = GetDomainAppendedInUserId(ref userId);
            if (string.IsNullOrEmpty(domain))
            {
                return new AuthenticationResult(AuthenticationResultCode.NotFound)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.InvalidCredential,
                    ErrorMessage = AuthenticationResources.DomainNotFound
                };
            }

            AuthUserAccount ua = GetAuthUserAccount(domain, userId);
            if (ua == null)
            {
                return new AuthenticationResult(AuthenticationResultCode.NotFound)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.InvalidCredential,
                    ErrorMessage = ServiceResources.SignInFailure_InvalidUserIdPassword
                };
            }

            var activeDirectoryDomains = GetActiveDirectoryDomains();
            var adDomains = activeDirectoryDomains.Where(a => string.Equals(a.Name, domain, StringComparison.InvariantCultureIgnoreCase)
                                                        && a.IsActive).ToList();
            if (adDomains.Count == 1)
            {
                ua.UserId = originalUserId;
                ua.IsSupportDomainUser = adDomains.First().IsSupportDomain;
            }

            return new AuthenticationResult(AuthenticationResultCode.Successful)
            {
                AuthUserAccount = ua
            };
        }

        #endregion

        #region Protected Members

        public Task<UserAccountResponse> GetUserAccountFromIdentityServerAsync(string accessToken, CancellationToken cancellationToken)
        {
            return _identityServerAuthenticationManager.GetUserAccountAsync(accessToken, cancellationToken);
        }

        protected virtual Task<AuthenticationResult> AuthenticateUserAsync(Context context, AuthUserAccount userAccount, TokenCredentials credentials, CancellationToken cancellationToken)
        {
            var idsAuthenticationResult = _identityServerAuthenticationManager.AuthenticateAsync(context, credentials, cancellationToken).Result;

            if (userAccount != null)
            {
                if (idsAuthenticationResult.ResultCode == AuthenticationResultCode.Successful)
                {
                    userAccount = idsAuthenticationResult.AuthUserAccount;
                }
                else
                {
                    idsAuthenticationResult.AuthUserAccount = userAccount;
                }
            }

            // If user account is not found in the local db OR if its not an IDS related issue then just return the authentication result.
            if (userAccount is null || !IdsIssues.Contains(idsAuthenticationResult.ResultCode))
            {
                return Task.FromResult(idsAuthenticationResult);
            }

            // If the execution has reached here that means its an IDS related issue
            Log.Debug("Authenticating User - {0} from DB", userAccount.UserId);

            var dbAuthenticator = GetDatabaseAuthenticator();

            if (userAccount.IsDomainAccount)
            {
                var result = ((DatabaseAuthenticator)dbAuthenticator).HandleActiveDirectoryAuthenticationResult(idsAuthenticationResult, credentials, userAccount, (Guid?)context.Device);
                result.UserPasswordExpiresInDays = result.ResultCode == AuthenticationResultCode.Successful ? GetPwdExpirationDuration(userAccount): int.MaxValue;
                return Task.FromResult(result);
            }
            var authResult = dbAuthenticator.AuthenticateUser(context, credentials, userAccount);
            authResult.UserPasswordExpiresInDays = authResult.ResultCode == AuthenticationResultCode.Successful ? GetPwdExpirationDuration(userAccount) : int.MaxValue; ;
            return Task.FromResult(authResult);

        }

        protected virtual void ChangeClinicalUserPassword(Context context, string oldPwd, string newPwd, AuthUserAccount userAccount)
        {
            //check if clinical user is domain user.
            if (userAccount.IsDomainAccount)
            {
                // TODO: Right now not supporting Change Password for domain user
                // ChangeClinicalUserPasswordAgainstAD(context, oldPwd, newPwd, currentUserAccount);

                ValidationError[] validationErrors =
                    new[]
                        {
                            ValidationError.CreateValidationError<PasswordCredential>(
                                ValidationStrings.DomainUserChangePasswordNotSupported)
                        };

                throw new ValidationException(
                    ValidationStrings.DomainUserChangePasswordNotSupported,
                    validationErrors);
            }
            else
            {
                // For temp user/super user ActiveDirectoryDomainKey will always be null so
                // do operation against local db
                IAuthenticator authenticator = null;
                AuthenticationResult authResult = GetAuthenticator(userAccount, ref authenticator);
                if (authResult.ResultCode != AuthenticationResultCode.Successful)
                    return; //should never fall into this

                authenticator.ChangePassword(context, userAccount, oldPwd, newPwd);
            }
        }

        protected virtual IEnumerable<string> ListPasswordRules(AuthUserAccount ua)
        {
            IAuthenticator authenticator = null;
            AuthenticationResult authResult = GetAuthenticator(ua, ref authenticator);
            if (authResult.ResultCode != AuthenticationResultCode.Successful)
                return new List<string>(); //should never fall into this

            return authenticator.ListPasswordRules();
        }

        protected virtual Guid LogAuthenticationEvent(AuthenticationEvent authEvent)
        {
            Guid authenticationEventKey = AuthenticationEventRepository.InsertAuthenticationEvent(authEvent);

            if (authenticationEventKey == default(Guid))
            {
                return default(Guid);
            }

            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                //Do not update for CF Support account. Updating last successful password will populate LastModifiedDispensingDeviceKey in UserAccount table.
                //This creates problem in enforcing constraint on UserAccount table while doing full download again.
                //Bug Fixed 202500 (While syncing the station which is already sync to a server build to another server build "Failed to Delete" error occurs.)
                //in case of card reader not detected userid will be null
                if (authEvent.UserId?.Equals(AuthenticationConstants.CFSupport, StringComparison.CurrentCultureIgnoreCase) ?? false)
                    return authenticationEventKey;
                //If user has signed in using pwd then update lastSuccessfullPwdAuthentication datetime
                if (authEvent.AuthenticationMethod == AuthenticationMethodInternalCode.UIDPWDW ||
                    authEvent.AuthenticationMethod == AuthenticationMethodInternalCode.UIDPWD)
                {
                    if (authEvent.SuccessfullyAuthenticated || authEvent.SuccessfullySignedIn)
                    {
                        //Update LastSuccessfulPassword datetime
                        repository.UpdateUserAccountLastSuccessfulPasswordAuthentication(authEvent.DispensingDeviceKey,
                                                                                 authEvent.UserAccountKey.Value,
                                                                                 DateTime.UtcNow, DateTime.Now);

                    }
                }
            }

            return authenticationEventKey;
        }

        protected virtual AuthenticationResult VerifyUserAccountStatus(Context context, AuthUserAccount userAccount)
        {
            if (userAccount.IsSupportDomainUser || userAccount.IsSupportUser)
            {
                // Let IDS handle the user verification of support domain users
                return new AuthenticationResult(AuthenticationResultCode.Successful) { AuthUserAccount = userAccount };
            }

            IAuthenticator authenticator = null;

            var authResult = GetAuthenticator(userAccount, ref authenticator);

            if (authResult.ResultCode != AuthenticationResultCode.Successful)
            {
                return authResult;
            }

            var result = authenticator.VerifyUser(context, userAccount);

            if (result.ResultCode == AuthenticationResultCode.DomainError) //only occur for domain user
            {
                return HandleVerificationResult(result, context, userAccount);
            }

            return result;
        }

        protected virtual Task<AuthenticationResult> VerifyUserAccountStatusAsync(Context context, AuthUserAccount userAccount, TokenCredentials credentials, CancellationToken cancellationToken)
        {
            if (userAccount.IsSupportDomainUser || userAccount.IsSupportUser)
            {
                //Let IDS handle the user verification of support domain users
                return Task.FromResult(new AuthenticationResult(AuthenticationResultCode.Successful) { AuthUserAccount = userAccount });
            }

            var idsAccountVerificationResult = _identityServerAuthenticationManager.VerifyUserAccountStatus(context, credentials, cancellationToken).Result;

            idsAccountVerificationResult.AuthUserAccount = userAccount;

            // If its NOT an IDS related issue then return the result.
            if (!IdsIssues.Contains(idsAccountVerificationResult.ResultCode))
            {
                return Task.FromResult(idsAccountVerificationResult);
            }

            // If execution has reached here means there is an IDS related issue, try to authenticate locally
            if(userAccount.IsDomainAccount)
            {
                var systemSettings = GetDispensingSystem();

                Log.Debug($"Verify user id from DB only if dispensing system setting AllowBioIdInDisconnectedMode: [{systemSettings.AllowBioIdInDisconnectedMode}] is enabled");

                if (!systemSettings.AllowBioIdInDisconnectedMode)
                {
                    return Task.FromResult(idsAccountVerificationResult);
                }
            }

            Log.Debug("Verifying User - {0} status from DB", userAccount.UserId);

            var dbAuthenticator = GetDatabaseAuthenticator();

            var result = dbAuthenticator.VerifyUser(context, userAccount);
            result.UserPasswordExpiresInDays = result.ResultCode == AuthenticationResultCode.Successful ? GetPwdExpirationDuration(userAccount) : int.MaxValue;
            return Task.FromResult(result);
        }

        protected virtual Task<AuthenticationResult> AuthenticateSmartCardUserAsync(Context context, TokenCredentials credentials, AuthUserAccount userAccount, X509Certificate2 smartCardCert, CancellationToken cancellationToken)
        {
            var idsAuthenticationResult = _identityServerAuthenticationManager.AuthenticateSmartCardUserAsync(context, credentials, smartCardCert, cancellationToken).Result;

            if (userAccount != null)
            {
                idsAuthenticationResult.AuthUserAccount = userAccount;
            }

            // If we don't have a user account within local db OR if its not an IDS related issue then just return the authentication result.
            if (userAccount is null || !IdsIssues.Contains(idsAuthenticationResult.ResultCode))
            {
                return Task.FromResult(idsAuthenticationResult);
            }

            Log.Debug($"AuthenticateSmartCardUserAsync, Identity server not reachable for user {credentials.UserId}, trigger fail-over authentication mode");

            idsAuthenticationResult.ResultCode = AuthenticationResultCode.IdentityServerUrlNotReachable;

            return Task.FromResult(idsAuthenticationResult);
        }
        protected virtual Task<AuthenticationResult> ValidateSmartCardCertificateAsync(Context context, TokenCredentials credentials, X509Certificate2 smartCardCert, CancellationToken cancellationToken)
        {
            var idsAuthenticationResult = _identityServerAuthenticationManager.AuthenticateSmartCardUserAsync(context, credentials, smartCardCert, cancellationToken).Result;
               
            if (!IdsIssues.Contains(idsAuthenticationResult.ResultCode))
            {
                return Task.FromResult(idsAuthenticationResult);
            }

            Log.Debug($"ValidateSmartCardCertificateSync, Identity server not reachable for user {credentials.UserId}, trigger fail-over authentication mode");

            idsAuthenticationResult.ResultCode = AuthenticationResultCode.IdentityServerUrlNotReachable;

            return Task.FromResult(idsAuthenticationResult);
        }

        protected virtual bool IsSystemDoD()
        {
            return false;
        }

        protected virtual void SignOutViaSwitchUser(Context context, Guid signInEventKey)
        {
            return;
        }

        protected virtual void UpdatePwdExpirationNotice(Context context, AuthUserAccount userAccount)
        {
            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                repository.UpdateUserAccountActivity(context, userAccount.Key, DateTime.UtcNow, DateTime.Now);
            }
        }

        protected AuthenticationResult LockNoAuthenticationUserAccount(Context context, AuthUserAccount account, DateTime? lastSuccessfulAuthenticationUtcDateTime,
            short? lockedNoAuthenticationDuration)
        {
            if (!account.IsSupportUser && // Check will not apply to support users
                !account.IsLocked &&
                lastSuccessfulAuthenticationUtcDateTime != null &&
                (lockedNoAuthenticationDuration != null && lockedNoAuthenticationDuration > 0))
            {
                TimeSpan notAuthenticatedDuration = TimeSpan.FromDays(lockedNoAuthenticationDuration.Value);
                using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
                {
                    // Check if the user account has not successfully authenticated in the past configured duration.
                    if ((DateTime.UtcNow - lastSuccessfulAuthenticationUtcDateTime) > notAuthenticatedDuration)
                    {
                        DateTime? lastUnlockedUtcDateTime = repository.GetLastUnlockedUtcDateTime(account.Key,
                            lastSuccessfulAuthenticationUtcDateTime.Value);
                        DateTime? lastUndeletedUtcDateTime = repository.GetLastUndeletedUtcDateTime(account.Key,
                            lastSuccessfulAuthenticationUtcDateTime.Value);

                        DateTime? lastUnlockedOrUndeletedUtcDateTime;
                        if (lastUnlockedUtcDateTime != null &&
                            lastUndeletedUtcDateTime != null)
                        {
                            // Get the earliest date
                            lastUnlockedOrUndeletedUtcDateTime = (lastUnlockedUtcDateTime > lastUndeletedUtcDateTime)
                                ? lastUnlockedUtcDateTime
                                : lastUndeletedUtcDateTime;
                        }
                        else
                        {
                            // Get the not NULL date if possible.
                            lastUnlockedOrUndeletedUtcDateTime = lastUnlockedUtcDateTime ?? lastUndeletedUtcDateTime;
                        }

                        // Check if the user account has recently been unlocked and/or undeleted.
                        if (lastUnlockedOrUndeletedUtcDateTime == null ||
                            (DateTime.UtcNow - lastUnlockedOrUndeletedUtcDateTime) > notAuthenticatedDuration)
                        {
                            //Lock the account
                            repository.LockUserAccount(context, account.Key);
                            account.IsLocked = true;

                            return new AuthenticationResult(AuthenticationResultCode.NoRecentAccessLock)
                            {
                                AuthUserAccount = account,
                                AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.NoRecentAccessLock,
                                ErrorMessage = ServiceResources.SignInFailure_AccountLocked
                            };
                        }
                    }
                }
            }

            //Bug#364661 - User Account Inactivity - Active Directory Users are locked at the first attempt but are able to login on subsequent attempts
            if (account.IsLocked) //if account is already locked then return locked status
            {
                return new AuthenticationResult(AuthenticationResultCode.AccountLocked)
                {
                    AuthUserAccount = account,
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.AccountAlreadyLocked,
                    ErrorMessage = ServiceResources.SignInFailure_AccountLocked
                };
            }

            return new AuthenticationResult(AuthenticationResultCode.Successful)
            {
                AuthUserAccount = account,
            };
        }

        #endregion

        #region Private Members

        private AuthenticationEvent GetLastSuccessfulAuthenticationEvent(Guid userAccountKey)
        {
            return AuthenticationEventRepository.GetLastSuccessfulAuthenticationEvent(userAccountKey);
        }

        private AuthenticationResult GetAuthenticator(AuthUserAccount userAccount, ref IAuthenticator authenticator)
        {
            if (userAccount.IsDomainAccount)
            {
                IEnumerable<ActiveDirectoryDomain> activeDirectoryDomains = GetActiveDirectoryDomains();
                var domain = activeDirectoryDomains.FirstOrDefault(add => add.Key == userAccount.ActiveDirectoryDomainKey);
                if (!domain.IsActive)
                    return new AuthenticationResult
                    {
                        ResultCode = AuthenticationResultCode.InactiveDomain,
                        AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.InactiveDomain,
                        ErrorMessage = AuthenticationResources.InactiveDomainAuthenticationError,
                        AuthUserAccount = userAccount
                    };

                switch (domain.UserDirectoryType)
                {
                    case UserDirectoryTypeInternalCode.AD:
                        authenticator = new ActiveDirectoryAuthenticator(domain);
                        break;

                    case UserDirectoryTypeInternalCode.OUD:
                        authenticator = new OracleUnifiedDirectoryAuthenticator(domain);
                        break;

                    //There are not Authenticators for the follwoing types.
                    //Yes they are redundant, they are here to show explicity
                    //That they are have not been overlooked.
                    case UserDirectoryTypeInternalCode.AAD:
                        break;
                    case UserDirectoryTypeInternalCode.UnknownInternalCode:
                    case null:
                    default:
                        return new AuthenticationResult(AuthenticationResultCode.DomainError)
                        {
                            AuthUserAccount = userAccount,
                            ErrorMessage = AuthenticationResources.DomainNotFound
                        };
                }

                return new AuthenticationResult(AuthenticationResultCode.Successful) { AuthUserAccount = userAccount };
            }

            // Default authenticator
            authenticator = GetDatabaseAuthenticator();
            return new AuthenticationResult(AuthenticationResultCode.Successful) { AuthUserAccount = userAccount };
        }

        private IAuthenticator GetDatabaseAuthenticator()
        {
            return new DatabaseAuthenticator(GetDispensingSystem());
        }

        private AuthenticationResult HandleVerificationResult(AuthenticationResult result, Context context, AuthUserAccount userAccount)
        {
            //if its domain error i.e. either domain connectivity error or SYSTEM ACCOUNT (not user account) credential error
            //then if AllowBioIdInDisconnectedMode is enabled then verify status against local db else return error code as is to end user

            DispensingSystem systemSettings = GetDispensingSystem();
            if (context.Device != null)
            {
                if (context.Device.DeviceAuthenticationMode == AuthenticationMethodInternalCode.UIDPWD ||
                    context.Device.DeviceAuthenticationMode == AuthenticationMethodInternalCode.UIDPWDW)
                {
                    if (systemSettings.AllowPasswordInDisconnectedMode)
                        //Verify against cached status
                        return GetDatabaseAuthenticator().VerifyUser(context, userAccount);
                }
                else if (context.Device.DeviceAuthenticationMode == AuthenticationMethodInternalCode.UIDFP ||
                         context.Device.DeviceAuthenticationMode == AuthenticationMethodInternalCode.FP)
                {
                    //If user not exempt from bioId and allow bioId in disconnected mode is on
                    if (!userAccount.IsFingerprintExempt &&
                        systemSettings.AllowBioIdInDisconnectedMode)
                    {
                        //Verify against cached status
                        return GetDatabaseAuthenticator().VerifyUser(context, userAccount);
                    }
                    //If user is exempt from bioId and allow cached pwd authentication in disconnected mode is on
                    if (userAccount.IsFingerprintExempt &&
                        systemSettings.AllowPasswordInDisconnectedMode)
                    {
                        //Verify against cached status
                        return GetDatabaseAuthenticator().VerifyUser(context, userAccount);
                    }
                }
                else if (context.Device.DeviceAuthenticationMode == AuthenticationMethodInternalCode.CardPIN)
                {
                    if (!userAccount.IsCardPinExempt &&
                        systemSettings.AllowCardPinInDisconnectedMode)
                    {
                        //Verify against cached status
                        return GetDatabaseAuthenticator().VerifyUser(context, userAccount);
                    }
                    //If user is exempt from CardPin and allow cached pwd authentication in disconnected mode is on
                    if (userAccount.IsCardPinExempt &&
                        systemSettings.AllowPasswordInDisconnectedMode)
                    {
                        //Verify against cached status
                        return GetDatabaseAuthenticator().VerifyUser(context, userAccount);
                    }
                }
                return result;
            }
            if (systemSettings.AllowPasswordInDisconnectedMode)
                //Verify against cached status
                return GetDatabaseAuthenticator().VerifyUser(context, userAccount);
            return result;
        }

        private int GetPwdExpirationDuration(AuthUserAccount userAccount)
        {
            var dispensingSystem = GetDispensingSystem();
            int noticeInterval = dispensingSystem.PasswordExpirationNoticeInterval;
            if (noticeInterval == 0)
                return int.MaxValue;
            //Check if user is already notified in last 24 hours
            if (userAccount.LastPasswordExpirationNoticeUtcDateTime.HasValue)
            {
                if (userAccount.LastPasswordExpirationNoticeUtcDateTime.Value.AddHours(24) > DateTime.UtcNow)
                    return int.MaxValue;
            }
            //if IDS not reachable ,then return max for AD user
            int maxAge = userAccount.IsDomainAccount ? int.MaxValue:GetDatabaseAuthenticator().GetMaxPwdAge();
            if (maxAge == int.MaxValue) //if PwdExpirationAge is set to never expired then return Max
                return int.MaxValue;

            //This API (GetPwdExpirationDuration()) is always called after user is authenticated.
            //If user pwd is expired or never changed after it is reset DirectoryChangePasswordUtcDateTime conatins NULL.
            //Logically following code should never execute as User will never get authenticated.
            if (!userAccount.DirectoryChangePasswordUtcDateTime.HasValue) //Pwd is expired or never changed after reset
            {
                Log.Debug("User - {0} password is either expired or never changed after reset! Returning 0.", userAccount.UserId);
                return 0;
            }
            //Fix for Bug# 210011 and 210013
            //NOTE: While storing max Date value in database during AD Sync it is converted to ShortDateString which is stored in db as 12/31/9999 12:00:00 AM
            //But DateTime.MaxValue returns 12/31/9999 11:59:59 AM which is not equal so for comparision with MaxValue converting to short date
            //i.e. 12/31/9999
            if (userAccount.DirectoryChangePasswordUtcDateTime.Value.ToShortDateString() == DateTime.MaxValue.ToShortDateString())//if user pwd is set to never expired then  DirectoryChangePasswordUtcDateTime contains MaxValue
            {
                Log.Debug("User - {0} password is set to never expired", userAccount.UserId);
                return int.MaxValue;
            }
            //If PwdLastChanged + (PwdAge - NoticeInterval) > Now then do not notify user
            if (userAccount.DirectoryChangePasswordUtcDateTime.Value.AddDays(maxAge - noticeInterval) > DateTime.UtcNow)
                return int.MaxValue;
            //return # of days left before pwd expire - pwdLastSet + maxAge - Now
            return userAccount.DirectoryChangePasswordUtcDateTime.Value.AddDays(maxAge).Subtract(DateTime.UtcNow).Days;
        }

        private string GetDomainAppendedInUserId(ref string userId)
        {
            string originalUserId = userId;
            //if domain\userId
            if (originalUserId.Contains('\\'))
            {
                userId = userId.Substring(userId.IndexOf('\\') + 1);
                return originalUserId.Substring(0, originalUserId.IndexOf('\\'));
            }

            if (originalUserId.Contains('@'))
            {
                var configuredAdDomains = GetActiveDirectoryDomains();

                if (configuredAdDomains == null)
                {
                    return null;
                }

                userId = userId.Substring(0, userId.IndexOf('@'));

                var userEnteredDomain = originalUserId.Substring(originalUserId.IndexOf('@') + 1);

                if (configuredAdDomains.Any(domain => string.Equals(domain.Name, userEnteredDomain, StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(domain.FullyQualifiedName, userEnteredDomain, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return userEnteredDomain;
                }

                //In order to allow BD users to authenticate with "@bd.com", we need to retrieve the domain wihtout the top-level domain. So, it
                //can be matched with Azure AD short domain name in database
                var fqdnParts = userEnteredDomain.Split('.');
                var strippedDomain = string.Join(".", fqdnParts.Take(fqdnParts.Length - 1));

                //There is no need to match with the FQDN, since we dropped the top-level domain
                if (configuredAdDomains.Any(domain => string.Equals(domain.Name, strippedDomain, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return strippedDomain;
                }
            }
            return null;
        }

        #endregion

        #region Queries

        private AuthUserAccount GetAuthUserAccount(Guid userAccountKey)
        {
            AuthUserAccount userAccount;

            Stopwatch sw = Stopwatch.StartNew();
            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                userAccount = repository.GetAuthenticationUserAccount(userAccountKey);
            }
            Log.Debug("GetAuthUserAccount(userAccountKey) query took {0} milliseconds.", sw.ElapsedMilliseconds);

            return userAccount;
        }

        private AuthUserAccount GetAuthUserAccount(string cardSerialID)
        {
            AuthUserAccount userAccount;

            Stopwatch sw = Stopwatch.StartNew();
            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                userAccount = repository.GetAuthenticationUserAccount(cardSerialID);
            }
            Log.Debug("GetAuthUserAccount(cardSerialID) query took {0} milliseconds.", sw.ElapsedMilliseconds);

            return userAccount;
        }

        private AuthUserAccount GetAuthUserAccountByRFIDCardSerialID(string rfidCardSerialID)
        {
            AuthUserAccount userAccount;

            Stopwatch sw = Stopwatch.StartNew();
            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                userAccount = repository.GetAuthUserAccountByRFIDCardSerialID(rfidCardSerialID);
            }
            Log.Debug("GetAuthUserAccountByRFIDCardSerialID(rfidCardSerialID) query took {0} milliseconds.", sw.ElapsedMilliseconds);

            return userAccount;
        }

        private AuthUserAccount GetAuthUserAccount(string domain, string userId)
        {
            AuthUserAccount userAccount;

            Stopwatch sw = Stopwatch.StartNew();
            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                if (domain.Equals(AuthenticationResources.NonDomainAccount, StringComparison.CurrentCultureIgnoreCase))
                {
                    userAccount = repository.GetAuthenticationUserAccount(null, userId);
                }
                else
                {
                    userAccount = repository.GetAuthenticationUserAccount(domain, userId);
                }

                if (userAccount == null)
                {
                    var uds = GetActiveDirectoryDomains().ToList();
                    var supports = uds.Where(a => (a.IsSupportDomain || (a.Groups != null && a.Groups.Any(g => g.SupportUser))) && a.IsActive);

                    Guid? domainKey = null;
                    var userDomain = default(ActiveDirectoryDomain);
                    if (supports.Any())
                    {
                        userDomain = supports.FirstOrDefault(u => u.FullyQualifiedName.Equals(domain, StringComparison.InvariantCultureIgnoreCase) ||
                            u.Name.Equals(domain, StringComparison.InvariantCultureIgnoreCase));
                        if (userDomain != null)
                        {
                            domainKey = userDomain.Key;
                        }
                    }

                    if (domainKey.HasValue)
                    {
                        userAccount = new AuthUserAccount
                        {
                            UserId = userId,
                            ActiveDirectoryDomainKey = domainKey.Value,
                            IsSupportUser = true,
                            IsSupportDomainUser = userDomain?.IsSupportDomain == true
                        };
                    }
                }
            }
            Log.Debug("GetAuthUserAccount(domain, userId) query took {0} milliseconds.", sw.ElapsedMilliseconds);

            return userAccount;
        }

        private IEnumerable<AuthUserAccount> GetAuthUserAccount(string userId, bool matchScanCode = false)
        {
            IEnumerable<AuthUserAccount> userAccounts;

            Stopwatch sw = Stopwatch.StartNew();
            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                if (matchScanCode)
                    userAccounts = repository.GetAuthenticationUserAccountsByScanCode(userId);
                else
                    userAccounts = repository.GetAuthenticationUserAccounts(userId);
            }
            Log.Debug("GetAuthUserAccount(userId) query took {0} milliseconds.", sw.ElapsedMilliseconds);

            return userAccounts;
        }

        private IEnumerable<ActiveDirectoryDomain> GetActiveDirectoryDomains()
        {
            IEnumerable<ActiveDirectoryDomain> activeDirectoryDomains =
                _cache.Get(CacheKeys.CommonActiveDirectoryDomains) as IEnumerable<ActiveDirectoryDomain>;

            if (activeDirectoryDomains == null)
            {
                Stopwatch sw = Stopwatch.StartNew();
                IActiveDirectoryDomainRepository repository = new ActiveDirectoryDomainRepository();
                IEnumerable<ActiveDirectoryDomain> results = repository.GetActiveDirectoryDomains();

                if (results != null && results.Any())
                {
                    var absoluteExpiration = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(2));
                    _cache.Add(CacheKeys.CommonActiveDirectoryDomains, results, absoluteExpiration);
                }
                activeDirectoryDomains = results;
                Log.Debug("GetActiveDirectoryDomains() query took {0} milliseconds.", sw.ElapsedMilliseconds);
            }

            return activeDirectoryDomains;
        }

        protected DispensingSystem GetDispensingSystem()
        {
            DispensingSystem dispensingSystem =
                _cache.Get(CacheKeys.CommonDispensingSystem) as DispensingSystem;

            if (dispensingSystem == null)
            {
                Stopwatch sw = Stopwatch.StartNew();
                dispensingSystem = _dispensingSystemRepository.GetDispensingSystem();
                Log.Debug("GetDispensingSystem() query took {0} milliseconds.", sw.ElapsedMilliseconds);
                if (dispensingSystem != null)
                {
                    var absoluteExpiration = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(5));
                    _cache.Add(CacheKeys.CommonDispensingSystem, dispensingSystem, absoluteExpiration);
                }
            }

            return dispensingSystem;
        }

        public Guid UpsertSupportUserAccount(Context context, UserAccount supportUser)
        {
            var sw = Stopwatch.StartNew();
            using (var repo = RepositoryFactory.Create<ICoreRepository>())
            {
                repo.UpsertSupportUserAccount(context, supportUser);
            }
            Log.Debug("UpsertSupportUserAccount(context, userAccountKey) query took {0} milliseconds.", sw.ElapsedMilliseconds);

            return supportUser.Key;

        }

        #endregion
    }
}
