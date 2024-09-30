using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CareFusion.Dispensing.Caching;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Encryption;
using CareFusion.Dispensing.Resources;
using CareFusion.UserDirectoryManagement;
using Pyxis.Core.Data.InternalCodes;
using Mms.Logging;
using System.Runtime.Caching;

namespace CareFusion.Dispensing.Services.Authenticators
{
    internal class ActiveDirectoryAuthenticator : IAuthenticator
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly ObjectCache Cache = MemoryCache.Default;

        private readonly ActiveDirectoryDomain _domain;

        #region Constructor

        public ActiveDirectoryAuthenticator(ActiveDirectoryDomain domain)
        {
            Guard.ArgumentNotNull(domain, "domain");
            _domain = domain;

            // decrypt pwd
            var encryptor = EncryptorFactory.CreateEncryptor();
            _domain.SystemAccountPassword = encryptor.Decrypt(
                domain.SystemAccountPasswordEncrypted,
                KeyContext.ActiveDirectoryPassword);
        }
        
        #endregion

        #region Implementation of IAuthenticator

        AuthenticationResult IAuthenticator.AuthenticateUser(Context context, Credentials credentials, AuthUserAccount userAccount)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                using (var adAuthMgr = GetAuthenticationManager())
                {
                    UserDirectoryResult result = adAuthMgr.AuthenticateUser(userAccount.UserId, credentials.Password);

                    if (result == UserDirectoryResult.Success)
                    {
                        return new AuthenticationResult(AuthenticationResultCode.Successful)
                                   {
                                       AuthUserAccount = userAccount
                                   };
                    }
                    var authResult = MapToAuthenticationResult(result);
                    authResult.AuthUserAccount = userAccount;
                    return authResult;
                }
            }
            catch(Exception e)
            {
                Log.Error(EventId.ADError, ServiceResources.DomainAuthenticationError, e);
                return new AuthenticationResult(AuthenticationResultCode.DomainError)
                {
                    AuthUserAccount = userAccount,
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.DomainErrorOrUnreachable,
                    ErrorMessage = ServiceResources.DomainAuthenticationError
                };
            }
            finally
            {
                Log.Debug(string.Format("Domain user ({0}) authentication took {1} milliseconds against domain {2}.", userAccount.UserId, sw.ElapsedMilliseconds, _domain.FullyQualifiedName));
            }
        }

        AuthenticationResult IAuthenticator.VerifyUser(Context context, AuthUserAccount userAccount)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                using (var adAuthMgr = GetAuthenticationManager())
                {
                    var result = adAuthMgr.VerifyUser(userAccount.UserId);
                    if (result.UserResult == UserDirectoryResult.Success)
                    {
                        return new AuthenticationResult(AuthenticationResultCode.Successful)
                                   {
                                       AuthUserAccount = userAccount
                                   };
                    }
                    var authResult = MapToAuthenticationResult(result.UserResult);
                    authResult.AuthUserAccount = userAccount;
                    return authResult;
                }
            }
            catch (Exception e)
            {
                Log.Error(EventId.ADError, ServiceResources.DomainAuthenticationError, e);
                return new AuthenticationResult(AuthenticationResultCode.DomainError)
                {
                    AuthUserAccount = userAccount,
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.DomainErrorOrUnreachable,
                    ErrorMessage = ServiceResources.DomainAuthenticationError
                };
            }
            finally
            {
                Log.Debug(string.Format("Domain user ({0}) verification took {1} milliseconds against domain {2}.", userAccount.UserId, sw.ElapsedMilliseconds, _domain.FullyQualifiedName));
            }
        }

        IEnumerable<string> IAuthenticator.ListPasswordRules()
        {
            List<string> rules = new List<string>();
            try
            {
                using (var adAuthMgr = GetAuthenticationManager())
                {
                    //FOR CHANGING PWD, BETTER GET REAL TIME DOMAIN POLICY
                    UserDirectoryPasswordPolicy pwdpolicy = adAuthMgr.GetUserDirectoryPasswordPolicy();
                    
                    // min length rule
                    rules.Add(string.Format(ValidationStrings.UserAccountPasswordLengthRule, pwdpolicy.MinPwdLength));

                    if (pwdpolicy.PwdComplexity)
                    {
                        // user id and password dont match rule
                        rules.Add(ValidationStrings.UserAccountUserNameIdPasswordNoMatchRule);
                        rules.Add(GetPasswordComplexityString());
                    }

                    // uniqueness rule
                    rules.Add(string.Format(
                        ValidationStrings.UserAccountPasswordUniquenessRule,
                        pwdpolicy.PwdHistoryLength));

                    // age rule
                    rules.Add(string.Format(
                        ValidationStrings.UserAccountPasswordAgeRule,
                        pwdpolicy.MinPwdAge));

                }
            }
            catch (Exception e)
            {
                Log.Error(EventId.ADError, ServiceResources.DomainAuthenticationError, e);
            }
            return rules;
        }

        void IAuthenticator.ChangePassword(Context context, AuthUserAccount userAccount, string oldPwd, string newPwd)
        {
            //TODO: Require in 1.1.4
            //_adAuthMgr.ChangePassword(oldPwd, newPwd, userAccount);
        }

        int IAuthenticator.GetMaxPwdAge()
        {
            var policy = GetDirectoryPasswordPolicy();
            if (policy != null)
                return policy.MaxPwdAge;
            Log.Error("Error while retrieving policy for domain [{0}]", _domain.FullyQualifiedName);
            return int.MaxValue; //in case of error setting max pwd age to max.
        }
        #endregion

        #region Private Members

        IUserDirectoryAuthenticationManager GetAuthenticationManager()
        {
            return UserStoreCreator.GetUserDirectoryAuthenticationManager(UserStoreContextType.Domain,
                _domain.FullyQualifiedName,
                _domain.SystemAccountId,
                _domain.SystemAccountPassword,
                _domain.Groups.Select(g => g.GroupName).ToList(),
                _domain.SecuredCommunication,
                (_domain.PortNumber != null) ? Convert.ToInt16(_domain.PortNumber) : 636);
        }

        AuthenticationResult MapToAuthenticationResult(UserDirectoryResult userDirectoryResult)
        {
            var authResult = new AuthenticationResult()
                                 {
                                     ResultCode = AuthenticationResultCode.IncorrectPassword,
                                     AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.InvalidCredential,
                                     ErrorMessage = ServiceResources.SignInFailure_InvalidUserIdPassword
                                 };
            switch (userDirectoryResult)
            {
                case UserDirectoryResult.AuthneticationFailed:
                    {
                        return authResult;
                    }
                case UserDirectoryResult.UserAcccountExpired:
                    {
                        authResult.ResultCode = AuthenticationResultCode.AccountExpired;
                        authResult.AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.AccountExpired;
                        authResult.ErrorMessage = ServiceResources.SignInFailure_AccountExpired;
                        return authResult;
                    }
                case UserDirectoryResult.UserAccountDisabled:
                    {
                        authResult.ResultCode = AuthenticationResultCode.AccountInactive;
                        authResult.AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.AccountInactive;
                        authResult.ErrorMessage = ServiceResources.SignInFailure_AccountInactive;
                        return authResult;
                    }
                case UserDirectoryResult.UserAccountLocked:
                    {
                        authResult.ResultCode = AuthenticationResultCode.AccountLocked;
                        authResult.AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.AccountAlreadyLocked;
                        authResult.ErrorMessage = ServiceResources.SignInFailure_AccountLocked;
                        return authResult;
                    }
                case UserDirectoryResult.LastAttemptWarning:
                    {
                        authResult.ResultCode = AuthenticationResultCode.WarnAccountLockout;
                        authResult.ErrorMessage = ServiceResources.SignInFailure_WarnAccountLockout;
                        return authResult;
                    }
            }
            return authResult;
        }
        
        string GetPasswordComplexityString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ValidationStrings.UserAccountDomainPasswordComplexityRule);
            sb.Append(string.Format(ValidationStrings.UserAccountPasswordComplexityRuleUpperCase,
                                        ""));
            sb.Append(',');
            sb.Append(string.Format(ValidationStrings.UserAccountPasswordComplexityRuleLowerCase,
                                    ""));
            sb.Append(',');
            sb.Append(string.Format(ValidationStrings.UserAccountPasswordComplexityRuleDigit,
                                    ""));
            sb.Append(',');
            sb.Append(string.Format(ValidationStrings.UserAccountPasswordComplexityRuleSpecialChars,
                                        ""));
            return sb.ToString();
        }

        UserDirectoryPasswordPolicy GetDirectoryPasswordPolicy()
        {
            //caching domain policy for an hour..
            var directoryPasswordPolicies = Cache.AddOrGetExisting(
                CacheKeys.CommonActiveDirectoryDomainPolicy,
                new Dictionary<string, UserDirectoryPasswordPolicy>(),
                DateTimeOffset.UtcNow.Add(TimeSpan.FromHours(1))) as Dictionary<string, UserDirectoryPasswordPolicy> ?? new Dictionary<string, UserDirectoryPasswordPolicy>();

            UserDirectoryPasswordPolicy policy = null;
            if (!directoryPasswordPolicies.TryGetValue(_domain.FullyQualifiedName, out policy))
            {
                try
                {
                    using (var adAuthMgr = GetAuthenticationManager())
                    {
                        policy = adAuthMgr.GetUserDirectoryPasswordPolicy();
                        if (policy != null)
                            directoryPasswordPolicies[_domain.FullyQualifiedName] = policy;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(EventId.ADError, ServiceResources.DomainAuthenticationError, e);
                }
            }
            return policy;
        }

        #endregion
    }
}
