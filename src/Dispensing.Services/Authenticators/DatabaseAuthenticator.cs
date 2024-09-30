using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data;
using CareFusion.Dispensing.Data.Entities;
using CareFusion.Dispensing.Data.Repositories;
using CareFusion.Dispensing.Models;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Services.Cryptography;
using Pyxis.Core.Data;
using Pyxis.Core.Data.InternalCodes;
using Mms.Logging;

namespace CareFusion.Dispensing.Services.Authenticators
{
    internal class DatabaseAuthenticator : IAuthenticator
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly DispensingSystem _dispensingSystem;
        private readonly IAuthenticationEventRepository _authenticationEventRepository;

        public DatabaseAuthenticator(DispensingSystem dispensingSystem)
        {
            Guard.ArgumentNotNull(dispensingSystem, "dispensingSystem");

            _dispensingSystem = dispensingSystem;
            _authenticationEventRepository = new AuthenticationEventRepository();
        }

        #region Implementation of IAuthenticator

        AuthenticationResult IAuthenticator.AuthenticateUser(Context context, Credentials credentials, AuthUserAccount userAccount)
        {
            // Verify the user.
            AuthenticationResult result = ((IAuthenticator)this).VerifyUser(context, userAccount);
            if (result.ResultCode != AuthenticationResultCode.Successful)
                return result;

            // Validate the password
            AuthenticationResultCode resultCode = ValidatePassword(context, credentials, userAccount);
            if (resultCode == AuthenticationResultCode.IncorrectPassword)
            {
                int failedCount = IsUserAccountBeLocked(context, userAccount);
                if (userAccount.IsLocked)
                {
                    return new AuthenticationResult(AuthenticationResultCode.AccountLocking)
                    {
                        AuthUserAccount = userAccount,
                        AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.MultipleFailedAttemptsLock,
                        ErrorMessage = ServiceResources.SignInFailure_AccountLocked
                    };
                }

                /*When user is entering wrong password and is one attempt away from being locked out, 
                 * show a warning that user will be locked out. "Attention: Another unsuccessful attempt will lock your account".
                 */
                if (failedCount > -1)
                {
                    if (CheckForLockoutWarnMessage(failedCount))
                    {
                        return new AuthenticationResult(AuthenticationResultCode.WarnAccountLockout)
                        {
                            AuthUserAccount = userAccount,
                            AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.InvalidCredential,
                            ErrorMessage = ServiceResources.SignInFailure_WarnAccountLockout
                        };
                    }
                }

                return new AuthenticationResult(resultCode)
                {
                    AuthUserAccount = userAccount,
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.InvalidCredential,
                    ErrorMessage = ServiceResources.SignInFailure_InvalidUserIdPassword
                };
            }

            if (resultCode == AuthenticationResultCode.TempPasswordExpired)
            {
                return new AuthenticationResult(resultCode)
                {
                    AuthUserAccount = userAccount,
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.TemporaryPasswordExpired,
                    ErrorMessage = ServiceResources.SignInFailure_TempPasswordExpired
                };
            }

            return new AuthenticationResult(resultCode)
            {
                AuthUserAccount = userAccount
            };
        }

        AuthenticationResult IAuthenticator.VerifyUser(Context context, AuthUserAccount userAccount)
        {
            // Is it active
            if (!userAccount.IsActive)
            {
                return new AuthenticationResult(AuthenticationResultCode.AccountInactive)
                {
                    AuthUserAccount = userAccount,
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.AccountInactive,
                    ErrorMessage = ServiceResources.SignInFailure_AccountInactive
                };
            }

            // Is it expired
            if (userAccount.AccountExpirationUtcDate.HasValue &&
                userAccount.AccountExpirationUtcDate <= DateTime.Now.ToUniversalTime())
            {
                return new AuthenticationResult(AuthenticationResultCode.AccountExpired)
                {
                    AuthUserAccount = userAccount,
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.AccountExpired,
                    ErrorMessage = ServiceResources.SignInFailure_AccountExpired
                };
            }

            // Is it locked
            if (userAccount.IsLocked)
            {
                return new AuthenticationResult(AuthenticationResultCode.AccountLocked)
                {
                    AuthUserAccount = userAccount,
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.AccountAlreadyLocked,
                    ErrorMessage = ServiceResources.SignInFailure_AccountLocked
                };
            }

            return new AuthenticationResult(AuthenticationResultCode.Successful)
            {
                AuthUserAccount = userAccount,
            };
        }

        IEnumerable<string> IAuthenticator.ListPasswordRules()
        {
            return PasswordChangeValidator.ListPasswordRules(_dispensingSystem);
        }

        void IAuthenticator.ChangePassword(Context context, AuthUserAccount userAccount, string oldPwd, string newPwd)
        {
            // Get the current password credential of the user.
            PasswordCredential currentPasswordCredential = GetPasswordCredential(userAccount.Key);

            // Get past password credentials
            IEnumerable<PasswordCredential> pastCredentials = GetPastPasswordCredentials(userAccount.Key,
                                                                                         currentPasswordCredential,
                                                                                         _dispensingSystem);

            // Create a new password credential.
            PasswordCredential newPasswordCredential = new PasswordCredential
            {
                Key = currentPasswordCredential.Key,
                LastModified = currentPasswordCredential.LastModified,
                Password = newPwd
            };

            // Set the password hash
            PasswordChangeValidator.SetUserAccountPasswordHash(newPasswordCredential, _dispensingSystem.EncryptionAlgorithm);

            // validate password
            PasswordChangeValidator.ValidateUserPasswordChange(_dispensingSystem,
                userAccount, currentPasswordCredential, pastCredentials, newPasswordCredential, oldPwd);
            //set the current date time in PasswordLastModified
            newPasswordCredential.UserChangedOwnPasswordUtcDate = DateTime.UtcNow;
            newPasswordCredential.UserChangedOwnPasswordDate = DateTime.Now;

            UpdatePassword(context, userAccount, newPasswordCredential, isInsert: false, isLocalUser: true);


        }

        int IAuthenticator.GetMaxPwdAge()
        {
            return _dispensingSystem.PasswordExpiration.HasValue
                       ? _dispensingSystem.PasswordExpiration.Value
                       : int.MaxValue;
        }

        #endregion


        #region Public Members

        public AuthenticationResult HandleActiveDirectoryAuthenticationResult(AuthenticationResult result, Credentials credentials, AuthUserAccount userAccount, Guid? dispensingDeviceKey)
        {
            if (!userAccount.IsDomainAccount)
                return result;

            Context addContext = new Context(new ActiveDirectoryDomainActor(userAccount.ActiveDirectoryDomainKey.Value));
            if (dispensingDeviceKey != null)
                addContext.Device = new DeviceContextInfo(dispensingDeviceKey.Value);

            //allow support users to always login with cached credentials, regardless of the server settings
            bool isPwdCacheEnabled = _dispensingSystem.AllowPasswordInDisconnectedMode
                                    || userAccount.IsSupportUser;

            //if its domain error i.e. either domain connectivity error or system account credential error
            //then if caching is enabled then authenticate against local db else return error code as is to end user
            if (isPwdCacheEnabled)
            {
                switch (result.ResultCode)
                {
                    case AuthenticationResultCode.Successful:
                        {
                            //support user will be added to the database after authenticated successfully
                            //so user key will be empty
                            if (userAccount.Key == Guid.Empty)
                            {
                                return result;
                            }

                            Stopwatch sw = Stopwatch.StartNew();

                            //Persist this new pwd only if modified
                            PersistDomainPasswordIfModified(userAccount, _dispensingSystem, credentials, addContext);

                            Log.Debug("HandleActiveDirectoryAuthenticationResult() caching domain password took {0} milliseconds.", sw.ElapsedMilliseconds);

                            return result;
                        }
                    case AuthenticationResultCode.DomainError:
                    case AuthenticationResultCode.NotFound:
                    case AuthenticationResultCode.IdentityServerUrlNotConfigured:
                    case AuthenticationResultCode.RequestTimedOut:
                        {
                            //check if current useraccount previously signed in using domain credential? 
                            var storedPasswordCredential = GetPasswordCredential(userAccount.Key);
                            if (storedPasswordCredential == null)
                            {
                                //TODO:LOG
                                return new AuthenticationResult(AuthenticationResultCode.DomainError)
                                {
                                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.DomainErrorOrUnreachable,
                                    ErrorMessage = ServiceResources.SignInFailure_NoCachePwdFound
                                };
                            }

                            Stopwatch sw = Stopwatch.StartNew();

                            //if domain pwd is not updated for a while(say pwd age) and user tried to sign in disconnected mode
                            //then we may get result code as ChangePasswordRequired which does not make any sense for domain user 
                            //in disconnected mode. In such case converting it to Success
                            AuthenticationResult dbResult = ((IAuthenticator)this).AuthenticateUser(addContext, credentials, userAccount);
                            if (dbResult.ResultCode == AuthenticationResultCode.ChangePasswordRequired)
                                dbResult.ResultCode = AuthenticationResultCode.Successful;

                            Log.Debug("HandleActiveDirectoryAuthenticationResult() re-authenticating domain user using cached password took {0} milliseconds.", sw.ElapsedMilliseconds);

                            return dbResult;
                        }

                    default:
                        return result;
                }
            }

            return result;
        }

        #endregion

        #region Private Members

        private AuthenticationResultCode ValidatePassword(Context context, Credentials credentials,
            AuthUserAccount userAccount)
        {
            var storedPasswordCredential = GetPasswordCredential(userAccount.Key);
            if (storedPasswordCredential != null)
            {
                var encryptor = TextEncryptorFactory.GetEncryptor(storedPasswordCredential.EncryptionAlgorithm);
                bool isMatch = encryptor.IsMatch(credentials.Password, storedPasswordCredential.Salt,
                    storedPasswordCredential.PasswordHash);

                //do following check only for domain user. DB Authentication Handler can be used in case of domain user in disconnected environment.
                if (isMatch && userAccount.IsDomainAccount)
                    return AuthenticationResultCode.Successful;

                if (isMatch)
                {
                    //check if pwd expired..
                    //VerifyEncryptionAlgorithm(context, credentials, userAccount.Key, storedPasswordCredential);

                    var expired = PasswordChangeValidator.IsPasswordExpired(_dispensingSystem, storedPasswordCredential);

                    if (expired)
                    {
                        AuthenticationResultCode result = VerifyTempPasswordExpiration(userAccount.Key,
                            storedPasswordCredential);
                        //check if pwd is reset and this temp pwd is within "Temp Password Expiration" duration
                        if (result != AuthenticationResultCode.Successful)
                            return result;
                        return AuthenticationResultCode.ChangePasswordRequired;
                    }

                    //Update user password if encryption algorithm is different than the current dispensing system settings
                    if (!_dispensingSystem.EncryptionAlgorithm.Equals(
                        storedPasswordCredential.EncryptionAlgorithm))
                    {
                        UpdatePasswordWithCurrentEncryptionAlgorithm(context, credentials, userAccount,
                            storedPasswordCredential);
                    }
                    return AuthenticationResultCode.Successful;
                }
            }
            return AuthenticationResultCode.IncorrectPassword;
        }

        private void UpdatePasswordWithCurrentEncryptionAlgorithm(Context context, Credentials credentials,
            AuthUserAccount userAccount, PasswordCredential storedPasswordCredential)
        {
            // Create a new password credential
            var newPasswordCredential = new PasswordCredential
            {
                Key = storedPasswordCredential.Key,
                LastModified = storedPasswordCredential.LastModified,
                Password = credentials.Password
            };
            PasswordChangeValidator.SetUserAccountPasswordHashWithoutValidation(newPasswordCredential,
                _dispensingSystem.EncryptionAlgorithm);
            //set the current date time in PasswordLastModified
            newPasswordCredential.UserChangedOwnPasswordUtcDate = storedPasswordCredential.UserChangedOwnPasswordUtcDate;
            newPasswordCredential.UserChangedOwnPasswordDate = storedPasswordCredential.UserChangedOwnPasswordDate;
            UpdatePassword(context, userAccount, newPasswordCredential, isInsert: false, isLocalUser: true);
        }

        private AuthenticationResultCode VerifyTempPasswordExpiration(Guid userAccountKey, PasswordCredential passwordCredential)
        {
            //first check if user change pwd after reset
            if (!passwordCredential.UserChangedOwnPasswordUtcDate.HasValue)
            {
                //check if temp pwd duration is within "Temp Password Expiration" duration
                //There is a flag  which indicates whether New user account should be exempt from Temp pwd expiration period.
                //BELOW CODE IS UGLY WAY TO IDENTIFY NEW USER ACCOUNT. FOR EVERY NEW ACCOUNT CREATED THERE WILL BE 
                //A RECORD ENTERED IN PASSWORDCREDENTIAL with FirstRecordFlag = 1. 
                if (!_dispensingSystem.UseTemporaryPasswordDurationForNewLocalUser)
                    if (HasInitialPasswordCredential(userAccountKey))
                    {
                        //Log...
                        return AuthenticationResultCode.Successful;
                    }

                DateTime pwdResetTime = passwordCredential.CreatedUtcDateTime;
                if (_dispensingSystem.TemporaryPasswordDuration.HasValue)
                {
                    pwdResetTime = pwdResetTime.AddHours(_dispensingSystem.TemporaryPasswordDuration.Value);
                    if (DateTime.UtcNow > pwdResetTime)
                        return AuthenticationResultCode.TempPasswordExpired;
                }
                else
                {
                    //default add 24 hours 
                    pwdResetTime = pwdResetTime.AddHours(24);
                    if (DateTime.UtcNow > pwdResetTime)
                        return AuthenticationResultCode.TempPasswordExpired;
                }
            }

            return AuthenticationResultCode.Successful;
        }

        private int IsUserAccountBeLocked(Context context, AuthUserAccount userAccount)
        {
            DateTime now = DateTime.UtcNow;

            int? lockoutWindowToCheck = _dispensingSystem.FailedAuthenticationLockoutInterval;
            int? maxRetryAttempts = _dispensingSystem.FailedAuthenticationAttemptsAllowed;

            if (!lockoutWindowToCheck.HasValue ||
                !maxRetryAttempts.HasValue)
                return -1;

            // Get the last successful time, user logged in.
            DateTime lastSuccessfulAuthentication = GetLastSuccessfulLoginTime(
                userAccount.Key);

            // Get the recent unlock time.
            DateTime lastUnlockTime = GetLastUnlockTime(
                userAccount.Key);

            //Take the recent time between successful sign in time and unlock time
            DateTime baseTime = lastSuccessfulAuthentication < lastUnlockTime
                                    ? lastUnlockTime
                                    : lastSuccessfulAuthentication;

            DateTime startTime = now.AddMinutes(-lockoutWindowToCheck.Value);

            // if base time was within window, shorten the window
            if (baseTime > startTime)
                startTime = baseTime;

            /*BUG# 44505 Load/Refill: User can login at the station with locked account
             * After meeting with Maria J. it was decided to take off auto-unlock feature. If User account is locked
             * then it can be unlocked only via user management.
             */
            // Get the number of failed login attempts.
            int numberOfFailedAttempts = GetFailedLoginAttemptsCount(context,
                userAccount.Key, startTime, now);

            //Increment numberOfFailedAttempts by 1 as this one which is not yet stored in DB is still failed attempt
            if (numberOfFailedAttempts + 1 >= maxRetryAttempts.Value)
            {
                using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
                {
                    repository.LockUserAccount(context, userAccount.Key);
                    userAccount.IsLocked = true;
                }
            }

            return numberOfFailedAttempts + 1;
        }

        private bool CheckForLockoutWarnMessage(int failedCount)
        {
            int? maxRetryAttempts = _dispensingSystem.FailedAuthenticationAttemptsAllowed;
            if (!maxRetryAttempts.HasValue)
                return false;

            if (maxRetryAttempts.Value - failedCount == 1)
                return true;

            return false;
        }

        private void UpdatePassword(Context context, AuthUserAccount authUserAccount,
            PasswordCredential password, bool isInsert, bool isLocalUser)
        {
            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            using (var tx = TransactionScopeFactory.Create())
            {
                if (isInsert)
                    repository.InsertPasswordCredential(context, authUserAccount.Key,
                                                        password);
                else
                    repository.UpdatePasswordCredential(context, authUserAccount.Key, password);

                //Update DirectoryChangePasswordLocal/UTCDateTime for the user on UserAccountSnapshot table for local users
                if (isLocalUser)
                {
                    //AuthUserAccount is not same as UserAccountSnapshot record so need to retrieve UAS record for corresponding user
                    var userAccount = repository.GetUserAccount(authUserAccount.Key);
                    if (userAccount != null)
                    {
                        //Bug# 210873 Update DirectoryChangePasswordLocal/UtcDateTime into AuthUserAccount as well to avoid Password expiration notification
                        userAccount.DirectoryChangePasswordDateTime = password.UserChangedOwnPasswordDate;
                        authUserAccount.DirectoryChangePasswordUtcDateTime = userAccount.DirectoryChangePasswordUtcDateTime = password.UserChangedOwnPasswordUtcDate;
                        repository.UpdateUserAccount(context, userAccount);
                    }
                }
                tx.Complete();
            }
        }

        private void PersistDomainPasswordIfModified(AuthUserAccount userAccount,
                                                    DispensingSystem systemSettings,
                                                    Credentials credentials,
                                                    Context context)
        {
            PasswordCredential passwordCredential = GetPasswordCredential(userAccount.Key);
            bool isInsert = false;

            //PasswordCredential cud be null if domain user is signing for first time
            if (passwordCredential != null)
            {
                //check if pwd is changed by comparing hashed value using stored salt
                var encryptor = TextEncryptorFactory.GetEncryptor(passwordCredential.EncryptionAlgorithm);
                if (encryptor.IsMatch(credentials.Password, passwordCredential.Salt, passwordCredential.PasswordHash))
                {
                    //Update user password if encryption algorithm is different than the current dispensing system settings
                    if (!_dispensingSystem.EncryptionAlgorithm.Equals(
                        passwordCredential.EncryptionAlgorithm))
                    {
                        UpdatePasswordWithCurrentEncryptionAlgorithm(context, credentials, userAccount,
                            passwordCredential);
                    }
                    return; //password didnt change from last stored
                }
            }
            else //signing in for first time so create new PasswordCredential
            {
                passwordCredential = new PasswordCredential();
                passwordCredential.Password = credentials.Password;
                isInsert = true;
            }

            //for domain user account set UserChangedOwnPasswordDate as null 
            passwordCredential.UserChangedOwnPasswordDate = null;
            passwordCredential.UserChangedOwnPasswordUtcDate = null;

            //Bug(380548) : Domain users are unable to login after password has been changed and then server/client are no longer communicating with active directory
            //Issue was after user change password, new password is never assigned to passwordCredential object and new hash is always created with password value as "null"
            //set the new password to passwordCredential object
            passwordCredential.Password = credentials.Password;
            PasswordChangeValidator.SetUserAccountPasswordHashWithoutValidation(passwordCredential,
                                                                        systemSettings.EncryptionAlgorithm);
            UpdatePassword(context, userAccount, passwordCredential, isInsert, isLocalUser: false);
        }
        #endregion

        #region Queries

        private PasswordCredential GetPasswordCredential(Guid userAccountKey)
        {
            PasswordCredential passwordCredential;

            Stopwatch sw = Stopwatch.StartNew();
            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                passwordCredential = repository.GetPasswordCredential(userAccountKey);
            }
            Log.Debug("GetPasswordCredential() query took {0} milliseconds.", sw.ElapsedMilliseconds);

            return passwordCredential;
        }

        private IEnumerable<PasswordCredential> GetPastPasswordCredentials(Guid userAccountKey, PasswordCredential currentPasswordCredential, DispensingSystem systemSettings)
        {
            var pastCredentials = new List<PasswordCredential>();
            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                // get past credentials as per system settings
                // we always want to use the current credential as 1 past credential,
                int pwdHistory = systemSettings.PasswordHistory;
                if (pwdHistory > 0)
                {
                    if (currentPasswordCredential != null)
                        pastCredentials.Add(currentPasswordCredential);

                    Stopwatch sw = Stopwatch.StartNew();
                    IEnumerable<PasswordCredential> fromDb =
                        repository.GetPasswordHistory(userAccountKey, pwdHistory - 1);
                    Log.Debug("GetPastPasswordCredentials() query took {0} milliseconds.", sw.ElapsedMilliseconds);
                    pastCredentials.AddRange(fromDb);
                }
            }

            return pastCredentials;
        }

        private bool HasInitialPasswordCredential(Guid userAccountKey)
        {
            bool hasInitialPasswordCredential = false;
            Stopwatch sw = Stopwatch.StartNew();
            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                hasInitialPasswordCredential = repository.Exists<Data.Schema.PasswordCredential>(pwc =>
                    pwc.UserAccountKey == userAccountKey &&
                    pwc.FirstRecordFlag == true &&
                    pwc.EndUTCDateTime == null);
            }
            Log.Debug("HasInitialPasswordCredential() query took {0} milliseconds.", sw.ElapsedMilliseconds);

            return hasInitialPasswordCredential;
        }

        private DateTime GetLastSuccessfulLoginTime(Guid userAccountKey)
        {
            Stopwatch sw = Stopwatch.StartNew();
            AuthenticationEvent authenticationEvent =
                _authenticationEventRepository.GetLastSuccessfulAuthenticationEvent(userAccountKey);
            Log.Debug("GetLastSuccessfulLoginTime() query took {0} milliseconds.", sw.ElapsedMilliseconds);

            return authenticationEvent != null ? authenticationEvent.AuthenticationUtcDateTime : DateTime.MinValue;
        }

        private DateTime GetLastUnlockTime(Guid userAccountKey)
        {
            DateTime lastUnlock;

            Stopwatch sw = Stopwatch.StartNew();
            using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
            {
                lastUnlock = (
                    from ua in repository.GetQueryableEntity<UserAccountEntity>()
                    where ua.Key == userAccountKey &&
                        ua.LockedFlag == false
                    orderby ua.StartUTCDateTime descending
                    select ua.StartUTCDateTime)
                    .FirstOrDefault();
            }
            Log.Debug("GetLastUnlockTime() query took {0} milliseconds.", sw.ElapsedMilliseconds);

            return lastUnlock;
        }

        private int GetFailedLoginAttemptsCount(Context context, Guid userAccountKey, DateTime startUtcTime, DateTime endUtcTime)
        {
            int failedAttempts;
            Stopwatch sw = Stopwatch.StartNew();
            failedAttempts = _authenticationEventRepository.GetFailedLoginAttemptsCount((Guid?)context.Device, userAccountKey,
                startUtcTime, endUtcTime);
            Debug.WriteLine("GetFailedLoginAttemptsCount query took {0} milliseconds.", sw.ElapsedMilliseconds);

            return failedAttempts;
        }
        #endregion
    }
}
