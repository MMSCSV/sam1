using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data;
using CareFusion.Dispensing.Models;
using CareFusion.Dispensing.Server.Contracts;
using CareFusion.Dispensing.Services.Business;
using Mms.Logging;

namespace CareFusion.Dispensing.Services
{
    public class DeviceAuthenticationService : BaseAuthenticationService, IDeviceAuthenticationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IFingerprintManager _fingerprintManager;

        public DeviceAuthenticationService() : this(new DeviceAuthenticationManager())
        {
        }

        internal DeviceAuthenticationService(IAuthenticationManager authManager) : base(authManager)
        {
            _fingerprintManager = new FingerprintManager();
        }

        // Authenticate with MatchScanCode
        AuthenticationResult IDeviceAuthenticationService.Authenticate(Context context, TokenCredentials credentials, bool matchScanCode)
        {
            return AuthenticateAsyncInternal(context, credentials, matchScanCode, CancellationToken.None).Result;
        }

        Task<AuthenticationResult> IDeviceAuthenticationService.AuthenticateAsync(Context context, TokenCredentials credentials, bool matchScanCode, CancellationToken cancellationToken)
        {
            return AuthenticateAsyncInternal(context, credentials, matchScanCode, cancellationToken);
        }

        // Authenticate with AuthUserAccount
        AuthenticationResult IDeviceAuthenticationService.Authenticate(Context context, TokenCredentials credentials, AuthUserAccount userAccount)
        {
            return AuthenticateAsyncInternal(context, credentials, userAccount, CancellationToken.None).Result;
        }

        Task<AuthenticationResult> IDeviceAuthenticationService.AuthenticateAsync(Context context, TokenCredentials credentials, AuthUserAccount userAccount, CancellationToken cancellationToken)
        {
            return AuthenticateAsyncInternal(context, credentials, userAccount, cancellationToken);
        }

        // Authenticate with SmartCard
        Task<AuthenticationResult> IDeviceAuthenticationService.AuthenticateSmartCardUserAsync(Context context, TokenCredentials credentials, X509Certificate2 smartCardCert, CancellationToken cancellationToken)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(credentials, "credentials");
            Guard.ArgumentNotNullOrEmptyString(credentials.UserId, "userId");
            Guard.ArgumentNotNull(smartCardCert, "smartCardCert");

            AuthenticationResult authenticationResult = null;

            var sw = Stopwatch.StartNew();

            try
            {
                using (new RepositorySessionScope())
                {
                    authenticationResult = GetAuthUserAccountById(context, credentials.UserId);
                    if ((authenticationResult.ResultCode != AuthenticationResultCode.NotFound &&
                         authenticationResult.ResultCode != AuthenticationResultCode.Successful)
                        || (authenticationResult.ResultCode == AuthenticationResultCode.NotFound && authenticationResult.AuthUserAccount == null))
                    {
                        Log.Debug($"AuthenticateSmartCardUserAsync, Clinical user [{credentials.UserId}] does not exist.");
                        return Task.FromResult(authenticationResult);
                    }

                    Log.Debug($"AuthenticateSmartCardUserAsync, Validating Smart card user [{credentials.UserId}]");

                    authenticationResult = AuthenticationManager.AuthenticateSmartCardUserAsync(context, credentials, authenticationResult.AuthUserAccount, smartCardCert, cancellationToken).Result;

                    if (authenticationResult.ResultCode == AuthenticationResultCode.Successful)
                    {
                        Log.Debug($"AuthenticateSmartCardUserAsync, Successfully validated user [{authenticationResult.AuthUserAccount.UserId}]");
                    }
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }
            finally
            {
                Log.Debug("AuthenticateSmartCardUserAsync(context, credentials, smartCardCert) took {0} milliseconds to complete.", sw.ElapsedMilliseconds);
            }

            return Task.FromResult(authenticationResult);
        }
        // validate SmartCard certificate
        Task<AuthenticationResult> IDeviceAuthenticationService.ValidateSmartCardCertificateAsync(Context context, TokenCredentials credentials, X509Certificate2 smartCardCert, CancellationToken cancellationToken)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(credentials, "credentials");
            Guard.ArgumentNotNull(smartCardCert, "smartCardCert");

            AuthenticationResult authenticationResult = null;

            var sw = Stopwatch.StartNew();

            try
            {
                Log.Debug($"ValidateSmartCardCertificateAsync, Validating Smart card certificate [{credentials.UserId}]");

                authenticationResult = AuthenticationManager.ValidateSmartCardCertificateAsync(context, credentials, smartCardCert, cancellationToken).Result;

                if (authenticationResult.ResultCode == AuthenticationResultCode.Successful)
                {
                    Log.Debug($"ValidateSmartCardCertificateAsync, Successfully validated user certificate");
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }
            finally
            {
                Log.Debug("ValidateSmartCardCertificateAsync(context, credentials, smartCardCert) took {0} milliseconds to complete.", sw.ElapsedMilliseconds);
            }

            return Task.FromResult(authenticationResult);
        }

        // VerifyUser with MatchScanCode
        AuthenticationResult IDeviceAuthenticationService.VerifyUserAccountStatus(Context context, TokenCredentials credentials, bool matchScanCode)
        {
            return VerifyUserAccountStatusAsyncInternal(context, credentials, matchScanCode, CancellationToken.None).Result;
        }

        Task<AuthenticationResult> IDeviceAuthenticationService.VerifyUserAccountStatusAsync(Context context, TokenCredentials credentials, bool matchScanCode, CancellationToken cancellationToken)
        {
            return VerifyUserAccountStatusAsyncInternal(context, credentials, matchScanCode, cancellationToken);
        }

        // VerifyUser with AuthUserAccount
        AuthenticationResult IDeviceAuthenticationService.VerifyUserAccountStatus(Context context, AuthUserAccount userAccount, TokenCredentials credentials)
        {
            return VerifyUserAccountStatusAsyncInternal(context, userAccount, credentials, CancellationToken.None).Result;
        }

        Task<AuthenticationResult> IDeviceAuthenticationService.VerifyUserAccountStatusAsync(Context context, AuthUserAccount userAccount, TokenCredentials credentials, CancellationToken cancellationToken)
        {
            return VerifyUserAccountStatusAsyncInternal(context, userAccount, credentials, cancellationToken);
        }

        private Task<AuthenticationResult> VerifyUserAccountStatusAsyncInternal(Context context, TokenCredentials credentials, bool matchScanCode, CancellationToken cancellationToken)
        {
            AuthenticationResult result = null;

            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(credentials, "credentials");
            Guard.ArgumentNotNullOrEmptyString(credentials.UserId, "userId");

            var sw = Stopwatch.StartNew();

            try
            {
                using (new RepositorySessionScope())
                {
                    result = GetAuthUserAccountById(context, credentials.UserId, matchScanCode);

                    if ((result.ResultCode != AuthenticationResultCode.NotFound &&
                         result.ResultCode != AuthenticationResultCode.Successful)
                        || (result.ResultCode == AuthenticationResultCode.NotFound && result.AuthUserAccount == null))
                    {
                        return Task.FromResult(result);
                    }
                    //add domain name to user id for ids to distinguish AD/local user with same userid if domain name is missing in token credential
                    UpdateTokenCredentials(credentials, result.AuthUserAccount, matchScanCode);

                    result = AuthenticationManager.VerifyUserAccountStatusAsync(context, result.AuthUserAccount, credentials, cancellationToken).Result;
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }
            finally
            {
                Log.Debug("VerifyUserAccountStatus() took {0} milliseconds to complete.", sw.ElapsedMilliseconds);
            }

            return Task.FromResult(result);
        }

        private Task<AuthenticationResult> VerifyUserAccountStatusAsyncInternal(Context context, AuthUserAccount userAccount, TokenCredentials credentials, CancellationToken cancellationToken)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userAccount, "userAccount");

            AuthenticationResult result = null;

            var sw = Stopwatch.StartNew();

            try
            {
                using (new RepositorySessionScope())
                {
                    result = AuthenticationManager.VerifyUserAccountStatusAsync(context, userAccount, credentials, cancellationToken).Result;
                    result.AuthUserAccount = userAccount; //reassign userAccount
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }
            finally
            {
                Log.Debug("VerifyUserAccountStatus(AuthUserAccount) took {0} milliseconds to complete.", sw.ElapsedMilliseconds);
            }

            return Task.FromResult(result);
        }

        void IDeviceAuthenticationService.SignOutViaSwitchUser(Context context, Guid signInEventKey)
        {
            try
            {
                Guard.ArgumentNotNull(context, "context");

                using (new RepositorySessionScope())
                {
                    AuthenticationManager.SignOutViaSwitchUser(context, signInEventKey);
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IDeviceAuthenticationService.ChangePassword(Context context, AuthUserAccount userAccount, string oldPwd, string newPwd)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userAccount, "userAccount");
            Guard.ArgumentNotNullOrEmptyString(oldPwd, "oldPwd");
            Guard.ArgumentNotNullOrEmptyString(newPwd, "newPwd");

            Stopwatch sw = Stopwatch.StartNew();

            using (new RepositorySessionScope())
            {
                AuthenticationManager.ChangeUserPassword(context, oldPwd, newPwd, userAccount);
            }

            Log.Debug("IDeviceAuthenticationService.ChangePassword() took {0} milliseconds to complete.", sw.ElapsedMilliseconds);
        }

        AuthUserAccount IDeviceAuthenticationService.GetUserAccount(Guid userAccountKey)
        {
            AuthUserAccount userAccount = null;
            Guard.ArgumentNotNull(userAccountKey, "userAccountKey");

            try
            {
                using (new RepositorySessionScope())
                {
                    userAccount = AuthenticationManager.GetAuthUserAccount(userAccountKey);
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }

            return userAccount;
        }

        AuthUserAccount IDeviceAuthenticationService.GetUserAccount(string cardSerialID)
        {
            AuthUserAccount userAccount = null;
            Guard.ArgumentNotNullOrEmptyString(cardSerialID, "cardSerialID");

            try
            {
                using (new RepositorySessionScope())
                {
                    userAccount = AuthenticationManager.GetAuthUserAccount(cardSerialID);
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }

            return userAccount;
        }

        AuthUserAccount IDeviceAuthenticationService.GetUserAccountByRFIDCardSerialID(string rfidCardSerialID)
        {
            AuthUserAccount userAccount = null;
            Guard.ArgumentNotNullOrEmptyString(rfidCardSerialID, "rfidCardSerialID");

            try
            {
                using (new RepositorySessionScope())
                {
                    userAccount = AuthenticationManager.GetAuthUserAccountByRFIDCardSerialID(rfidCardSerialID);
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }

            return userAccount;
        }

        public void EnrollCardSerialID(Context context, Guid userAccountKey, string cardSerialID)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userAccountKey, "userAccountKey");
            Guard.ArgumentNotNullOrEmptyString(cardSerialID, "cardSerialID");

            try
            {
                AuthenticationManager.EnrollCardSerialID(context, userAccountKey, cardSerialID);
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
                AuthenticationManager.EnrollRFIDCardSerialID(context, userAccountKey, rfidCardSerialID);
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IDeviceAuthenticationService.RegisterFingerprint(Context context, UserFingerprint userFingerprint)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(userFingerprint, "userFingerprint");

            try
            {
                _fingerprintManager.RegisterFingerprint(context, userFingerprint);
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }
        }

        UserFingerprint IDeviceAuthenticationService.GetUserFingerprintTemplates(Guid userAccountKey)
        {
            UserFingerprint userFingerprint = null;

            try
            {
                userFingerprint = _fingerprintManager.GetFingerprintForUser(userAccountKey);
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }

            return userFingerprint;
        }

        Guid IDeviceAuthenticationService.LogAuthenticationEvent(AuthenticationEvent authenticationEvent)
        {
            Guid eventKey = Guid.Empty;

            try
            {
                using (new RepositorySessionScope())
                {
                    eventKey = AuthenticationManager.LogAuthenticationEvent(authenticationEvent);
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }

            return eventKey;
        }

        void IDeviceAuthenticationService.UpdateAuthenticationEvent(Guid authenticationEventKey, Guid witnessEventKey, DateTime authLocalDatetime, DateTime authUtcDatetime, bool success)
        {
            try
            {
                using (new RepositorySessionScope())
                {
                    AuthenticationManager.UpdateWitnessAuthenticationEvent(authenticationEventKey, witnessEventKey,
                        authLocalDatetime, authUtcDatetime, success);
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }
        }

        bool IDeviceAuthenticationService.IsDodDisclaimerNeeded()
        {
            var isDod = false;
            try
            {
                using (new RepositorySessionScope())
                {
                    isDod = AuthenticationManager.IsDodDisclaimerNeeded();
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }

            return isDod;
        }

        DispensingSystem IDeviceAuthenticationService.GetDispensingSystemSettings()
        {
            try
            {
                return AuthenticationManager.GetDispensingSystemSettings();
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }

            return null;
        }

        IEnumerable<ActiveDirectoryDomain> IDeviceAuthenticationService.GetActiveDirectoryDomains()
        {
            try
            {
                using (new RepositorySessionScope())
                {
                    return AuthenticationManager.GetActiveDirectoryDomains();
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }

            return null;
        }

        void IDeviceAuthenticationService.UpdateUserAccountLastSuccessfulPasswordAuthentication(Guid? deviceKey, Guid userAccountKey)
        {
            try
            {
                using (new RepositorySessionScope())
                {
                    AuthenticationManager.UpdateUserAccountLastSuccessfulPasswordAuthentication(deviceKey, userAccountKey);
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }

        }

        RecentUserAccountAuthenticationAttemptData IDeviceAuthenticationService.GetPastAuthenticationDetails(Context context, Guid userKey, Guid ignoreAuthenticationEventKey)
        {
            RecentUserAccountAuthenticationAttemptData data = null;
            // get from server
            using (var client = new DispensingServiceClient<IDispensingServerService>("DispensingServerService", context.Device.ServerAddress))
            {
                try
                {
                    // call server to get information
                    data = client.Proxy.GetRecentUserAccountAuthenticationAttempts(userKey, ignoreAuthenticationEventKey);
                }
                catch (Exception ex)
                {

                    Log.Warn("Could not communicate with server to retreive last sign in information", ex);
                }
            }

            return data;
        }

        #region Private Functions

        private Task<AuthenticationResult> AuthenticateAsyncInternal(Context context, TokenCredentials credentials, AuthUserAccount userAccount, CancellationToken cancellationToken)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(credentials, "credentials");
            Guard.ArgumentNotNull(userAccount, "userAccount");

            credentials.UserId = credentials.UserId.Trim();

            var sw = Stopwatch.StartNew();

            if (userAccount.IsDomainAccount)
            {
                Log.Debug("User account with user id = '{UserId}' is a domain account. Retrieving fully qualified user id.'", userAccount.UserId);

                var fullyQualifiedUserId = GetFullyQualifiedUserId(userAccount);

                Log.Debug("User id = {UserId}, FullyQualifiedUserId={FullyQualifiedUserId}", userAccount.UserId, fullyQualifiedUserId);

                //add domain name to user id for ids to distinguish AD/local user with same userid
                credentials.UserId = fullyQualifiedUserId;
            }

            AuthenticationResult authenticationResult = null;

            try
            {
                using (new RepositorySessionScope())
                {
                    var existingUserAccountKey = userAccount.Key;

                    authenticationResult = AuthenticationManager.AuthenticateUserAsync(context, userAccount, credentials, cancellationToken).Result;

                    var tempUser = userAccount;

                    if (authenticationResult.ResultCode == AuthenticationResultCode.Successful)
                    {
                        if (authenticationResult.AuthUserAccount.IsSupportUser && !string.IsNullOrEmpty(authenticationResult.AccessToken))
                        {
                            tempUser = ProcessSupportUserAsync(context, authenticationResult, cancellationToken).Result;
                        }
                        // If clinical user has not been synced down to station yet then do not allow the user to login
                        else if (existingUserAccountKey == Guid.Empty)
                        {
                            Log.Debug($"Clinical user [{tempUser.UserId}] has not synced down to station yet, so not allowed to login");

                            authenticationResult.ResultCode = AuthenticationResultCode.AuthenticationFailed;
                        }
                    }

                    authenticationResult.AuthUserAccount = tempUser;
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }
            finally
            {
                Log.Debug("Authenticate(context, credentials, userAccount) took {0} milliseconds to complete.", sw.ElapsedMilliseconds);
            }

            return Task.FromResult(authenticationResult);
        }

        private Task<AuthenticationResult> AuthenticateAsyncInternal(Context context, TokenCredentials credentials, bool matchScanCode, CancellationToken cancellationToken)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(credentials, "credentials");

            credentials.UserId = credentials.UserId.Trim();

            AuthenticationResult authenticationResult = null;

            var sw = Stopwatch.StartNew();

            try
            {
                using (new RepositorySessionScope())
                {
                    authenticationResult = GetAuthUserAccountById(context, credentials.UserId, matchScanCode);
                   
                    //If user is found, but, it failed due to other reasons such as inactive domain, multi domains result, etc.,
                    //then fail authentication early. Otherwise, this could be a support user first time login, therefore, let IDS
                    //handle it.
                    //TODO: move this logic to IDS, and only do this check if IDS is not reachable.
                    if ((authenticationResult.ResultCode != AuthenticationResultCode.NotFound &&
                         authenticationResult.ResultCode != AuthenticationResultCode.Successful)
                        || (authenticationResult.ResultCode == AuthenticationResultCode.NotFound
                            && authenticationResult.AuthUserAccount == null))
                    {
                        return Task.FromResult(authenticationResult);
                    }
                    //add domain name to user id for ids to distinguish AD/local user with same userid if domain name is missing in token credential
                    UpdateTokenCredentials(credentials, authenticationResult.AuthUserAccount, matchScanCode);

                    var existingUserAccountKey = authenticationResult.AuthUserAccount.Key;

                    authenticationResult = AuthenticationManager.AuthenticateUserAsync(context, authenticationResult.AuthUserAccount, credentials, cancellationToken).Result;

                    if (authenticationResult.ResultCode == AuthenticationResultCode.Successful)
                    {
                        if (authenticationResult.AuthUserAccount.IsSupportUser && !string.IsNullOrEmpty(authenticationResult.AccessToken))
                        {
                            authenticationResult.AuthUserAccount = ProcessSupportUserAsync(context, authenticationResult, cancellationToken).Result;
                        }
                        // If clinical user has not been synced down to station yet then do not allow the user to login
                        else if (existingUserAccountKey == Guid.Empty)
                        {
                            Log.Debug($"Clinical user [{authenticationResult.AuthUserAccount.UserId}] has not synced down to station yet, so not allowed to login");
                            authenticationResult.ResultCode = AuthenticationResultCode.AuthenticationFailed;
                            authenticationResult.AuthUserAccount = null;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }
            finally
            {
                Log.Debug("Authenticate(context, credentials) took {0} milliseconds to complete.", sw.ElapsedMilliseconds);
            }

            return Task.FromResult(authenticationResult);
        }

        private Task<AuthUserAccount> ProcessSupportUserAsync(Context context, AuthenticationResult authenticationResult, CancellationToken cancellationToken)
        {
            var response = AuthenticationManager.GetUserAccountFromIdentityServerAsync(authenticationResult.AccessToken, cancellationToken).Result;

            // response will be null when IDS is unreachable
            if (response?.UserProfile == null)
                return Task.FromResult(authenticationResult.AuthUserAccount);

            var userAccount = new UserAccount
            {
                Key = response.UserProfile.UserAccountKey,
                UserId = response.UserProfile.UserId,
                FirstName = response.UserProfile.FirstName ?? string.Empty,
                LastName = response.UserProfile.LastName ?? string.Empty,
                FullName = response.UserProfile.FullName ?? string.Empty,
                SnapshotKey = response.UserProfile.UserAccountSnapshotKey,
                IsSupportUser = response.UserProfile.IsSupportUser,
                ActiveDirectoryDomain = new ActiveDirectoryDomain()
                {
                    Key = response.UserProfile.ActiveDirectoryDomainKey ?? Guid.Empty
                },
                ActiveDirectoryObjectGuid = response.UserProfile.ActiveDirectoryGlobalId,
                ScanCode = response.UserProfile.ScanCode,
                IsFingerprintExempt = response.UserProfile.FingerprintExempt,
                IsTemporary = response.UserProfile.IsTemporary,
                IsLocked = response.UserProfile.IsAccountLocked,
                IsActive = response.UserProfile.IsAccountActive,
                IsSuperUser = response.UserProfile.IsSuperUser,
                CardSerialId = response.UserProfile.CardSerialId,
                RFIDCardSerialID = response.UserProfile.RFIDCardSerialID,
                DirectoryChangePasswordUtcDateTime = response.UserProfile.DirectoryChangePasswordUtc,
                DirectoryChangePasswordDateTime = response.UserProfile.DirectoryChangePasswordUtc?.ToLocalTime()
            };

            var key = UpsertSupportUserAccount(context, userAccount);

            return Task.FromResult(new AuthUserAccount
            {
                Key = key,
                UserId = userAccount.UserId,
                FirstName = userAccount.FirstName,
                LastName = userAccount.LastName,
                IsSupportUser = userAccount.IsSupportUser,
                IsActive = userAccount.IsActive,
                ActiveDirectoryDomainKey = userAccount.ActiveDirectoryDomain.Key
            });
        }

        #endregion
    }
}
