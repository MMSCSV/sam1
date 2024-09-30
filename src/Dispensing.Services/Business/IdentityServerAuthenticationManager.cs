using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data.Repositories;
using CareFusion.Dispensing.Resources;
using Newtonsoft.Json;
using Pyxis.Core.Data.InternalCodes;
using Mms.Logging;
using Pyxis.IdentityServer.Middleware.Models;
using Pyxis.IdentityServer.Middleware.Modules;
using System;
using System.IdentityModel.Tokens.Jwt;
using CareFusion.Dispensing.Resources.Common;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace CareFusion.Dispensing.Services.Business
{
    public class IdentityServerAuthenticationManager : IIdentityServerAuthenticationManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private const string ProfileClaimType = "profile";
        private const string UserPasswordExpiresInDays = "UserPasswordExpiresInDays";
        private const string PasswordExpiresSoon = "PasswordExpiresSoon";
        private const string PasswordExpirationNoticeHasBeenShownWithin24Hours = "PasswordExpirationNoticeHasBeenShownWithin24Hours";

        private readonly ISystemConfigurationRepository _systemConfigurationRepository;
        protected IIdentityServerMiddleware IdsMiddleware;

        public IdentityServerAuthenticationManager(ISystemConfigurationRepository systemConfigurationRepository)
        {
            _systemConfigurationRepository = systemConfigurationRepository ?? new SystemConfigurationRepository();
        }

        public Task<AuthenticationResult> AuthenticateAsync(Context context, TokenCredentials credentials, CancellationToken cancellationToken = default)
        {
            var identityServerUrl = GetIdentityServerUrl();

            if (string.IsNullOrEmpty(identityServerUrl))
            {
                Log.Debug($"IDS Authentication: User:{credentials.UserId} | IDS Url: not configured");

                return Task.FromResult(new AuthenticationResult(AuthenticationResultCode.IdentityServerUrlNotConfigured)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.DomainErrorOrUnreachable,
                    ErrorMessage = ServiceResources.SignInFailure_IdentityServerUrl
                });
            }

            CreateIdentityServerMiddleware(identityServerUrl, cancellationToken);

            if (IdsMiddleware is null)
            {
                return Task.FromResult(new AuthenticationResult(AuthenticationResultCode.DomainError)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.DomainErrorOrUnreachable,
                    ErrorMessage = ServiceResources.SignInFailure_IdentityServerNotAvailable
                });
            }

            Log.Debug($"IDS Authentication - Sending Request: User:{credentials.UserId} | IDS Url:{identityServerUrl} | Client ID:{credentials.ClientId}");

            //Todo: Can we make Authenticate just take the TokenCredential?
            var result = IdsMiddleware.Authenticate(credentials.ClientId,
                                                          credentials.ClientSecret,
                                                          credentials.GrantType,
                                                          credentials.Scope,
                                                          credentials.UserId,
                                                          credentials.Password,
                                                          cancellationToken).Result;

            Log.Debug($"IDS Authentication - Result Received: User:{credentials.UserId} | HTTPCode:{result.StatusCode} | Status Message:{result.StatusMessage}");

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return Task.FromResult(new AuthenticationResult(AuthenticationResultCode.NotFound)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.DomainErrorOrUnreachable,
                    ErrorMessage = ServiceResources.SignInFailure_IdentityServerNotAvailable
                });
            }

            if (result.StatusCode == HttpStatusCode.RequestTimeout)
            {
                return Task.FromResult(new AuthenticationResult(AuthenticationResultCode.RequestTimedOut)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.RequestTimedOut,
                    ErrorMessage = ServiceResources.SignInFailure_IdentityServerRequestTimedOut
                });
            }

            if (!string.IsNullOrEmpty(result.Data.AccessToken) && string.IsNullOrEmpty(result.Data.Error))
            {
                Log.Debug($"IDS Authentication - Successfully received token. Attempting to retrieve profile claim for user.");
                var token = new JwtSecurityToken(jwtEncodedString: result.Data.AccessToken);

                if (token.Claims != null)
                {
                    var claims = token.Claims.ToHashSet();
                    foreach (Claim claim in claims)
                    {
                        if (claim.Type.Equals(ProfileClaimType))
                        {
                            var authResult = new AuthenticationResult(AuthenticationResultCode.Successful, result.Data.AccessToken);
                            var userProfile = JsonConvert.DeserializeObject<UserProfile>(claim.Value);
                            authResult.AuthUserAccount = new AuthUserAccount
                            {
                                Key = userProfile.UserAccountKey,
                                UserId = userProfile.UserId,
                                FirstName = userProfile.FirstName ?? string.Empty,
                                LastName = userProfile.LastName ?? string.Empty,
                                MiddleName = userProfile.MiddleName ?? string.Empty,
                                SnapshotKey = userProfile.UserAccountSnapshotKey,
                                IsSupportUser = userProfile.IsSupportUser,
                                ActiveDirectoryDomainKey = userProfile.ActiveDirectoryDomainKey,
                                ScanCode = userProfile.ScanCode,
                                IsFingerprintExempt = userProfile.FingerprintExempt,
                                IsCardPinExempt = userProfile.IsCardPinExempt,
                                IsTemporary = userProfile.IsTemporary,
                                IsLocked = userProfile.IsAccountLocked,
                                IsActive = userProfile.IsAccountActive,
                                IsSuperUser = userProfile.IsSuperUser,
                                CardSerialId = userProfile.CardSerialId,
                                RFIDCardSerialID = userProfile.RFIDCardSerialID,
                                DirectoryChangePasswordUtcDateTime = userProfile.DirectoryChangePasswordUtc,
                                LastPasswordExpirationNoticeUtcDateTime = userProfile.LastPasswordExpirationNoticeUtc,
                                LastSuccessfulPasswordAuthenticationUtcDateTime = userProfile.LastSuccessfulPasswordAuthenticationUtc,
                                AccountExpirationUtcDate = userProfile.AccountExpirationUtc,
                                UserTypeKey = userProfile.UserTypeKey
                            };

                            Log.Debug($"IDS Authentication - Successfully deserialized user profile for: '{authResult.AuthUserAccount.UserId}'");
                            authResult.UserPasswordExpiresInDays = GetPasswordExpirationDuration(claims);
                            return Task.FromResult(authResult);
                        }
                    }
                }

                Log.Debug($"IDS Authentication - Failed to retrieve user profile.");
                return Task.FromResult(new AuthenticationResult(AuthenticationResultCode.AuthenticationFailed, result.Data.ErrorDescription));
            }

            Log.Debug($"IDS Authentication - Error: User:{credentials.UserId} | Error:{result.Data.Error} | Error Description:{result.Data.ErrorDescription}");

            return Task.FromResult(GenerateAuthenticationResultWithError(result.Data.ErrorDescription));
        }

        public async Task<AuthenticationResult> AuthenticateSmartCardUserAsync(Context context, TokenCredentials credentials, X509Certificate2 smartCardCert, CancellationToken cancellationToken = default)
        {
            var identityServerUrl = GetIdentityServerUrl();

            if (string.IsNullOrEmpty(identityServerUrl))
            {
                Log.Debug($"IDS Authentication: User:{credentials.UserId ?? "Certificate"} | IDS Url: not configured");

                return new AuthenticationResult(AuthenticationResultCode.IdentityServerUrlNotConfigured)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.DomainErrorOrUnreachable,
                    ErrorMessage = ServiceResources.SignInFailure_IdentityServerUrl
                };
            }

            CreateIdentityServerMiddleware(identityServerUrl, cancellationToken);

            if (IdsMiddleware is null)
            {
                return new AuthenticationResult(AuthenticationResultCode.DomainError)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.DomainErrorOrUnreachable,
                    ErrorMessage = ServiceResources.SignInFailure_IdentityServerNotAvailable
                };
            }

            Log.Debug($"IDS Authentication - Sending Request: User:{credentials.UserId} | IDS Url:{identityServerUrl} | Client ID:{credentials.ClientId}");

            var result = await IdsMiddleware.VerifySmartCardUserAsync(credentials.ClientId,
                                                          credentials.ClientSecret,
                                                          credentials.Scope,
                                                          smartCardCert,
                                                          credentials.UserId,
                                                          cancellationToken).ConfigureAwait(false);

            Log.Debug($"IDS User Account Verification - Result Received: User:{credentials.UserId} | HTTPCode:{result.StatusCode} | Status Message:{result.StatusMessage}");

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return new AuthenticationResult(AuthenticationResultCode.NotFound)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.DomainErrorOrUnreachable,
                    ErrorMessage = ServiceResources.SignInFailure_IdentityServerNotAvailable
                };
            }

            if (result.StatusCode == HttpStatusCode.RequestTimeout)
            {
                return new AuthenticationResult(AuthenticationResultCode.RequestTimedOut)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.RequestTimedOut,
                    ErrorMessage = ServiceResources.SignInFailure_IdentityServerRequestTimedOut
                };
            }

            // IIS triggers Microsoft security pin popup at device side to validate pin, once pin is verified then IIS validates smart card certificate from certificate authority
            // Microsoft security pin popup always returns with HttpStatusCode.Forbidden error and aborts request to IDS
            // IIS returns HttpStatusCode.Forbidden error if failed to validate certificate or certificate is invalid
            // If user cancels or closes the pin popup, then status code will be 0
            if (result.StatusCode == HttpStatusCode.Forbidden || (int)result.StatusCode == 0)
            {
                Log.Debug($"AuthenticateSmartCardUserAsync, Failed to validate Smart Card certificate for user {credentials.UserId}. Status: {result.StatusCode}, Error: {result.Data?.Error}");
                return new AuthenticationResult(AuthenticationResultCode.SmartCardCertificateRevokedOrInvalid);
            }

            if (!string.IsNullOrEmpty(result.Data.AccessToken) && string.IsNullOrEmpty(result.Data.Error))
            {
                Log.Debug($"AuthenticateSmartCardUserAsync, IDS User Account Verification - Successfully validated user id {credentials.UserId} from IDS.");
                return new AuthenticationResult(AuthenticationResultCode.Successful)
                {
                   UserPasswordExpiresInDays = GetPasswordExpirationDuration(result.Data.Raw)
                };
            }

            Log.Debug($"AuthenticateSmartCardUserAsync, IDS Authentication - Error: User:{credentials.UserId} | Error:{result.Data.Error} | Error Description:{result.Data.ErrorDescription}");

            return GenerateVerifyUserResultWithError(result.Data.Error);
        }

        public async Task<AuthenticationResult> VerifyUserAccountStatus(Context context, TokenCredentials credentials, CancellationToken cancellationToken = default)
        {
            var identityServerUrl = GetIdentityServerUrl();

            if (string.IsNullOrEmpty(identityServerUrl))
            {
                Log.Debug($"IDS User Account Verification: User:{credentials.UserId} | IDS Url: not configured");

                return new AuthenticationResult(AuthenticationResultCode.IdentityServerUrlNotConfigured)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.DomainErrorOrUnreachable,
                    ErrorMessage = ServiceResources.SignInFailure_IdentityServerUrl
                };
            }

            CreateIdentityServerMiddleware(identityServerUrl, cancellationToken);

            if (IdsMiddleware is null)
            {
                return new AuthenticationResult(AuthenticationResultCode.DomainError)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.DomainErrorOrUnreachable,
                    ErrorMessage = ServiceResources.SignInFailure_IdentityServerNotAvailable
                };
            }

            Log.Debug($"IDS User Account Verification - Sending Request: User:{credentials.UserId} | IDS Url:{identityServerUrl} | Client ID:{credentials.ClientId}");

            //Todo: Can we make Authenticate just take the TokenCredential?
            var result = await IdsMiddleware.VerifyUserIdAsync(credentials.ClientId,
                                                          credentials.ClientSecret,
                                                          credentials.Scope,
                                                          credentials.UserId,
                                                          cancellationToken).ConfigureAwait(false);

            Log.Debug($"IDS User Account Verification - Result Received: User:{credentials.UserId} | HTTPCode:{result.StatusCode} | Status Message:{result.StatusMessage}");

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                return new AuthenticationResult(AuthenticationResultCode.NotFound)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.DomainErrorOrUnreachable,
                    ErrorMessage = ServiceResources.SignInFailure_IdentityServerNotAvailable
                };
            }

            if (result.StatusCode == HttpStatusCode.RequestTimeout)
            {
                return new AuthenticationResult(AuthenticationResultCode.RequestTimedOut)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.RequestTimedOut,
                    ErrorMessage = ServiceResources.SignInFailure_IdentityServerRequestTimedOut
                };
            }

            if (!string.IsNullOrEmpty(result.Data.AccessToken) && string.IsNullOrEmpty(result.Data.Error))
            {
                Log.Debug($"VerifyUserAccountStatus, IDS User Account Verification - Successfully validated user id {credentials.UserId} from IDS.");
                return new AuthenticationResult(AuthenticationResultCode.Successful)
                {
                    UserPasswordExpiresInDays = GetPasswordExpirationDuration(result.Data.Raw)
                };
            }

            Log.Debug($"IDS Authentication - Error: User:{credentials.UserId} | Error:{result.Data.Error}");

            return GenerateVerifyUserResultWithError(result.Data.Error);
        }

        public Task<UserAccountResponse> GetUserAccountAsync(string accessToken, CancellationToken cancellationToken = default)
        {
            if (accessToken is null)
            {
                return default;
            }

            var identityServerUrl = GetIdentityServerUrl();

            if (string.IsNullOrEmpty(identityServerUrl))
            {
                return default;
            }

            CreateIdentityServerMiddleware(identityServerUrl, cancellationToken);

            if (IdsMiddleware is null)
            {
                return default;
            }

            var result = IdsMiddleware.UserInfo(accessToken, cancellationToken).Result;

            return Task.FromResult(result.Data);
        }

        private static AuthenticationResult GenerateAuthenticationResultWithError(string errorDescription)
        {
            var splitMessage = errorDescription?.Split(new[] { ':' }, 2) ?? new[] { string.Empty };

            if (splitMessage[0] == IdentityServerAuthenticationFailureCode.TempPasswordExpired.ToString())
            {
                return new AuthenticationResult(AuthenticationResultCode.TempPasswordExpired)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.TemporaryPasswordExpired,
                    ErrorMessage = ServiceResources.SignInFailure_TempPasswordExpired
                };
            }

            if (splitMessage[0] == IdentityServerAuthenticationFailureCode.PasswordChangeRequired.ToString())
            {
                return new AuthenticationResult(AuthenticationResultCode.ChangePasswordRequired);
            }

            if (splitMessage[0] == IdentityServerAuthenticationFailureCode.NextFailLock.ToString())
            {
                return new AuthenticationResult(AuthenticationResultCode.WarnAccountLockout)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.InvalidCredential,
                    ErrorMessage = ServiceResources.SignInFailure_WarnAccountLockout
                };
            }

            if (splitMessage[0] == IdentityServerAuthenticationFailureCode.AccountLocked.ToString())
            {
                return new AuthenticationResult(AuthenticationResultCode.AccountLocking)
                {
                    AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.MultipleFailedAttemptsLock,
                    ErrorMessage = ServiceResources.SignInFailure_AccountLocked
                };
            }

            return new AuthenticationResult(AuthenticationResultCode.AuthenticationFailed)
            {
                AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.InvalidCredential,
                ErrorMessage = ServiceResources.SignInFailure_InvalidUserIdPassword
            };
        }

        private string GetIdentityServerUrl()
        {
            var systemConfigType = _systemConfigurationRepository.GetSystemConfiguration(SystemConfigTypeInternalCode.IdentityServerURL.ToInternalCode());

            return systemConfigType?.ConfigValue;
        }

        private static AuthenticationResult GenerateVerifyUserResultWithError(string error)
        {
            var authResult = new AuthenticationResult(AuthenticationResultCode.AuthenticationFailed);
            AuthenticationResultCode userDirectoryResult;
            if (Enum.TryParse<AuthenticationResultCode>(error, true, out userDirectoryResult))
            {
                switch (userDirectoryResult)
                {
                    case AuthenticationResultCode.AccountExpired:
                        {
                            authResult.ResultCode = AuthenticationResultCode.AccountExpired;
                            authResult.AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.AccountExpired;
                            authResult.ErrorMessage = ServiceResources.SignInFailure_AccountExpired;
                            return authResult;
                        }
                    case AuthenticationResultCode.AccountInactive:
                        {
                            authResult.ResultCode = AuthenticationResultCode.AccountInactive;
                            authResult.AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.AccountInactive;
                            authResult.ErrorMessage = ServiceResources.SignInFailure_AccountInactive;
                            return authResult;
                        }
                    case AuthenticationResultCode.AccountAlreadyLocked:
                        {
                            authResult.ResultCode = AuthenticationResultCode.AccountLocked;
                            authResult.AuthenticationFailureReason = AuthenticationFailureReasonInternalCode.AccountAlreadyLocked;
                            authResult.ErrorMessage = ServiceResources.SignInFailure_AccountLocked;
                            return authResult;
                        }
                    case AuthenticationResultCode.InactiveDomain:
                        {
                            authResult.ResultCode = AuthenticationResultCode.InactiveDomain;
                            authResult.ErrorMessage = AuthenticationResources.InactiveDomainAuthenticationError;
                            return authResult;
                        }
                    case AuthenticationResultCode.DomainError:
                        {
                            authResult.ResultCode = AuthenticationResultCode.DomainError;
                            authResult.ErrorMessage = ServiceResources.DomainAuthenticationError;
                            return authResult;
                        }
                }
            }
            return authResult;
        }

        private void CreateIdentityServerMiddleware(string idsUrl, CancellationToken cancellationToken)
        {
            if (IdsMiddleware != null) return;

            try
            {
                IdsMiddleware = new IdentityServerMiddleware(idsUrl, cancellationToken);
            }
            catch (Exception e)
            {
                Log.Error("Exception occurred.", e);
            }
        }
        private int GetPasswordExpirationDuration(string result)
        {
           try
           {
             var pwdPolicy = JsonConvert.DeserializeAnonymousType(result, new
             {
                PasswordExpiresSoon = false,
                UserPasswordExpiresInDays = int.MaxValue,
                PasswordExpirationNoticeHasBeenShownWithin24Hours = false
             });
             Log.Debug($"Password Expiration  Notice - UserPasswordExpiresInDays[{pwdPolicy.UserPasswordExpiresInDays}] PasswordExpiresSoon[{pwdPolicy.PasswordExpiresSoon}] PasswordExpirationNoticeHasBeenShownWithin24Hours[{pwdPolicy.PasswordExpirationNoticeHasBeenShownWithin24Hours}]");
             //do not show password expiring soon notice if its shown is last 24 hrs.
             return pwdPolicy.PasswordExpiresSoon && !pwdPolicy.PasswordExpirationNoticeHasBeenShownWithin24Hours ? pwdPolicy.UserPasswordExpiresInDays : int.MaxValue;
           }
           catch (Exception e)
           {
               Log.Error("Exception occurred while extracting password expiration duration from IDS authentication result. Hence returning int.MaxValue for password expiration duration", e);
           }
           return int.MaxValue;
        }
        private int GetPasswordExpirationDuration(IEnumerable<Claim> claims)
        {
           try
           {
             var pwdExpiresInDays = int.Parse(claims.FirstOrDefault(c => c.Type.Equals(UserPasswordExpiresInDays))?.Value);
             var pwdExpiresSoon = bool.Parse(claims.FirstOrDefault(c => c.Type.Equals(PasswordExpiresSoon))?.Value);
             var pwdExpiresNoticeShownWithin24Hrs = bool.Parse(claims.FirstOrDefault(c => c.Type.Equals(PasswordExpirationNoticeHasBeenShownWithin24Hours))?.Value);
             Log.Debug($"Password Expiration  Notice - UserPasswordExpiresInDays[{pwdExpiresInDays}] PasswordExpiresSoon[{pwdExpiresSoon}] PasswordExpirationNoticeHasBeenShownWithin24Hours[{pwdExpiresNoticeShownWithin24Hrs}]");
             //do not show password expiring soon notice if its shown is last 24 hrs.
             return pwdExpiresSoon && !pwdExpiresNoticeShownWithin24Hrs ? pwdExpiresInDays : int.MaxValue;
           }
           catch (Exception e)
           {
               Log.Error("Exception occurred while extracting password expiration duration from IDS authentication result. Hence returning int.MaxValue for password expiration duration", e);
           }
           return int.MaxValue;
        }
    }
}
