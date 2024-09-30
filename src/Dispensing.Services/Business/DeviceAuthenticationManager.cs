using System;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data.Repositories;
using CareFusion.Dispensing.Models;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Server.Contracts;
using Pyxis.Core.Data.InternalCodes;
using Mms.Logging;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace CareFusion.Dispensing.Services.Business
{
    internal class DeviceAuthenticationManager : BaseAuthenticationManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public DeviceAuthenticationManager()
        {
        }

        public DeviceAuthenticationManager(IDispensingSystemRepository dispensingSystemRepository,IAuthenticationEventRepository authenticationEventRepository,
            IIdentityServerAuthenticationManager identityServerAuthenticationManager)
            : base(dispensingSystemRepository, authenticationEventRepository, identityServerAuthenticationManager)
        {
        }

        protected override Task<AuthenticationResult> AuthenticateUserAsync(Context context, AuthUserAccount userAccount, TokenCredentials credentials, CancellationToken cancellationToken)
        {
            var authResult = base.AuthenticateUserAsync(context, userAccount, credentials, cancellationToken).Result;

            if(authResult.AuthUserAccount == null || authResult.ResultCode != AuthenticationResultCode.Successful)
            {
                return Task.FromResult(authResult);
            }

            var lockClinicalResult = LockAndClinicalUserCheck(userAccount, context);

            return lockClinicalResult.ResultCode != AuthenticationResultCode.Successful ? Task.FromResult(lockClinicalResult) : Task.FromResult(authResult);
        }

        //Bug# 117465 - User Authentication: Non Support BioId User can login to station when station is out of service and in BioId mode
        protected override AuthenticationResult VerifyUserAccountStatus(Context context, AuthUserAccount userAccount)
        {
            var authResult = LockAndClinicalUserCheck(userAccount, context);

            if (authResult.ResultCode != AuthenticationResultCode.Successful)
            {
                return authResult;
            }

            return base.VerifyUserAccountStatus(context, userAccount);
        }

        protected override Task<AuthenticationResult> VerifyUserAccountStatusAsync(Context context, AuthUserAccount userAccount, TokenCredentials credentials, CancellationToken cancellationToken)
        {
            var authResult = LockAndClinicalUserCheck(userAccount, context);

            if (authResult.ResultCode != AuthenticationResultCode.Successful)
            {
                return Task.FromResult(authResult);
            }

            return base.VerifyUserAccountStatusAsync(context, userAccount, credentials, cancellationToken);
        }

        protected override Task<AuthenticationResult> AuthenticateSmartCardUserAsync(Context context, TokenCredentials credentials, AuthUserAccount userAccount, X509Certificate2 smartCardCert, CancellationToken cancellationToken)
        {
            var authResult = LockAndClinicalUserCheck(userAccount, context);

            if (authResult.ResultCode != AuthenticationResultCode.Successful)
            {
                return Task.FromResult(authResult);
            }

            return base.AuthenticateSmartCardUserAsync(context, credentials, userAccount, smartCardCert, cancellationToken);
        }

        protected override bool IsSystemDoD()
        {
            DispensingSystem dispensingSystem = GetDispensingSystem();
            if (dispensingSystem == null) //if device is not configured then return false
                return false;
            return dispensingSystem.IsDoD;
        }

        protected override void SignOutViaSwitchUser(Context context, Guid signInEventKey)
        {
            Guard.ArgumentNotNull(context, "context");

            AuthenticationEventRepository.UpdateAuthenticationEventSwitchUser(context, signInEventKey, SignOutReasonInternalCode.MSO);
        }

        private AuthenticationResult ClinicalUserCheck(AuthUserAccount userAccount, Context context)
        {
            //Note: 1.4.1 onwards device authentication will not check User-Facility association
            //if (!IsUserAssociatedWithFacility(context, userAccount))
            //{
            //    return new AuthenticationResult(AuthenticationResultCode.NotFound)
            //    {
            //        AuthUserAccount = userAccount,
            //        ErrorMessage = ServiceResources.SignInFailure_InvalidUserIdPassword
            //    };
            //}

            return IsDeviceOutOfService(context, userAccount);
        }

        private AuthenticationResult IsDeviceOutOfService(Context context, AuthUserAccount currentUserAccount)
        {
            //Is device out of service??
            if (context.Device != null &&
                context.Device.IsOutOfService &&
                currentUserAccount != null &&
                !currentUserAccount.IsSupportUser)
            {
                return new AuthenticationResult(AuthenticationResultCode.DeviceOutOfService)
                {
                    AuthUserAccount = currentUserAccount,
                    ErrorMessage = ServiceResources.SignInFailure_DeviceOutOfService
                };
            }

            return new AuthenticationResult
            {
                ResultCode = AuthenticationResultCode.Successful
            };
        }

        private AuthenticationEventData GetLastSuccessfulAuthenticationEvent(Context context, Guid userKey)
        {
            AuthenticationEventData data = null;
            // get from server
            using (var client = new DispensingServiceClient<IDispensingServerService>("DispensingServerService", context.Device.ServerAddress))
            {
                try
                {
                    // call server to get information
                    data = client.Proxy.GetRecentUserAccountSuccessfulAttempt(userKey);
                }
                catch (Exception ex)
                {

                    Log.Warn("Could not communicate with server to retrieve last sign in information", ex);
                }
            }
            return data;
        }

        private AuthenticationResult LockAndClinicalUserCheck(AuthUserAccount userAccount, Context context)
        {
            DispensingSystem dispensingSystem = GetDispensingSystem();
            if (dispensingSystem != null &&
                dispensingSystem.LockedNoAuthenticationDuration != null &&
                dispensingSystem.LockedNoAuthenticationDuration > 0)
            {
                //Lock user account if no activity for certain days
                AuthenticationEventData authenticationDetails =
                    GetLastSuccessfulAuthenticationEvent(context, userAccount.Key);

                if (authenticationDetails != null)
                {
                    AuthenticationResult lockedUserResult = LockNoAuthenticationUserAccount(context, userAccount,
                        authenticationDetails.AuthenticationUtcDateTime,
                        dispensingSystem.LockedNoAuthenticationDuration);

                    if (lockedUserResult.ResultCode != AuthenticationResultCode.Successful)
                        return lockedUserResult;
                }
            }

            return ClinicalUserCheck(userAccount, context);
        }


    }
}
