using System;
using System.Collections.Generic;
using System.Diagnostics;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Encryption;
using CareFusion.Dispensing.Resources;
using CareFusion.UserDirectoryManagement;
using CareFusion.UserDirectoryManagement.GenericLdap;
using Pyxis.Core.Data.InternalCodes;
using Mms.Logging;

namespace CareFusion.Dispensing.Services.Authenticators
{
    public class OracleUnifiedDirectoryAuthenticator : IAuthenticator
    {
        private readonly LdapDirectoryAdapter ldapAdapter;
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly string domainName;

        public OracleUnifiedDirectoryAuthenticator(ActiveDirectoryDomain domain)
        {
            domainName = domain.FullyQualifiedName;

            // decrypt pwd
            var encryptor = EncryptorFactory.CreateEncryptor();
            domain.SystemAccountPassword = encryptor.Decrypt(
                domain.SystemAccountPasswordEncrypted,
                KeyContext.ActiveDirectoryPassword);

            var portNumber = domain.PortNumber ?? 389;

            ldapAdapter = new LdapDirectoryAdapter(
                new LdapConnectionWrapper(
                    new LdapConfiguration(domain.FullyQualifiedName,
                        domain.SystemAccountId,
                        domain.SystemAccountPassword,
                        domain.SecuredCommunication,
                        portNumber,
                        domain.LDAPCertificateFileName)));
        }

        public AuthenticationResult AuthenticateUser(Context context, Credentials credentials, AuthUserAccount userAccount)
        {
            credentials.UserId = RemoveDomainFromUserId(credentials.UserId);

            Func<UserDirectoryResult> authenticateUserMethod = () => ldapAdapter.AuthenticateUser(credentials.UserId, credentials.Password);

            return AuthenticateUser(authenticateUserMethod, userAccount);
        }

        public AuthenticationResult VerifyUser(Context context, AuthUserAccount userAccount)
        {
            userAccount.UserId = RemoveDomainFromUserId(userAccount.UserId);

            Func<UserDirectoryResult> verifyUserMethod = () => ldapAdapter.VerifyUser(userAccount.UserId).UserResult;

            return AuthenticateUser(verifyUserMethod, userAccount);
        }

        private AuthenticationResult AuthenticateUser(Func<UserDirectoryResult> method, AuthUserAccount userAccount)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var result = method.Invoke();

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
                Log.Debug($"Domain user ({userAccount.UserId}) authentication took {sw.ElapsedMilliseconds} milliseconds against domain {domainName}.");
            }
        }

        private string RemoveDomainFromUserId(string userId)
        {
            userId = userId.Replace($"{domainName}\\", "");
            userId = userId.Replace($"@{domainName}", "");

            return userId;
        }

        public IEnumerable<string> ListPasswordRules()
        {
            //LDAP does not support this operation
            return new List<string>();
        }
        public void ChangePassword(Context context, AuthUserAccount userAccount, string oldPwd, string newPwd)
        {
            //LDAP does not support this operation
        }
        public int GetMaxPwdAge()
        {
            //LDAP does not support this operation
            return int.MaxValue;
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
    }
}
