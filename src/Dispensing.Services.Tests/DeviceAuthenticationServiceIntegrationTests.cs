using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data.Repositories;
using CareFusion.Dispensing.Services;
using CareFusion.Dispensing.Services.Business;
using Dispensing.Services.tests.Stubs.SystemConfiguration;
using NUnit.Framework;
using Pyxis.Core.Data.InternalCodes;
using Pyxis.Core.Data.Schema.Core.Models;

namespace Dispensing.Services.tests
{
    [TestFixture]
    [Category("Integration")]
    public class DeviceAuthenticationServiceIntegrationTests : AuthenticationServiceTestBase
    {
        private TokenCredentials tokenCredentials;

        private SystemConfigurationStub systemConfigurationStub;
        private IDeviceAuthenticationService authenticationService;
        private List<SystemConfig> systemConfigs;

        [SetUp]
        public void SetUp()
        {
            tokenCredentials = new TokenCredentials
            {
                AuthenticationMethod = AuthenticationMethodInternalCode.UIDPWD,
                AuthenticationPurpose = AuthenticationPurposeInternalCode.SI,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                GrantType = "password",
                Scope = "profile",
                UserId = ValidUsername,
                Password = ValidPassword
            };

            systemConfigs = new List<SystemConfig>
            {
                new SystemConfig
                {
                    SystemConfigTypeInternalCode = SystemConfigTypeInternalCode.IdentityServerURL.ToInternalCode(),
                    ConfigValue = IdentityServerUrl
                }
            };

            systemConfigurationStub = new SystemConfigurationStub(systemConfigs);

            var idsManager = new IdentityServerAuthenticationManager(systemConfigurationStub);

            IAuthenticationManager authManager = new DeviceAuthenticationManager(new DispensingSystemRepository(), new AuthenticationEventRepository(), idsManager);
            authenticationService = new DeviceAuthenticationService(authManager);
        }

        [Test]
        public void AuthenticateUser_VerifyUserId()
        {
            tokenCredentials = new TokenCredentials
            {
                AuthenticationMethod = AuthenticationMethodInternalCode.UIDFP,
                AuthenticationPurpose = AuthenticationPurposeInternalCode.SI,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                GrantType = Pyxis.IdentityServer.Middleware.Models.Enums.AllowedCustomGrants.VerifyUser.ToString(),
                Scope = "profile openid",
                UserId = ValidUsername
            };

            var result = authenticationService.VerifyUserAccountStatusAsync(new Context(), tokenCredentials, false).Result;

            Assert.AreEqual(AuthenticationResultCode.Successful, result.ResultCode);
            Assert.IsTrue(result.IsAuthenticated);
        }

        [Test]
        public void AuthenticateUser_MatchScanCode()
        {
            var result = authenticationService.AuthenticateAsync(new Context(), tokenCredentials, false).Result;

            Assert.AreEqual(AuthenticationResultCode.Successful, result.ResultCode);
            Assert.IsTrue(result.IsAuthenticated);
        }

        [Test]
        public void AuthenticateUser_AuthUserAccount()
        {
            var result = authenticationService.AuthenticateAsync(new Context(), tokenCredentials,
                                                                 new AuthUserAccount
                                                                 {
                                                                    UserId = "admin",
                                                                    IsSupportUser = false,
                                                                    Key = Guid.NewGuid()
                                                                 }).Result;

            Assert.AreEqual(AuthenticationResultCode.Successful, result.ResultCode);
            Assert.IsTrue(result.IsAuthenticated);
        }

        [Test]
        public void AuthenticateUser_IdentityServerUrlNotConfigured_FallbackToOldAuthentication()
        {
            systemConfigs.First().ConfigValue = string.Empty;

            var result = authenticationService.AuthenticateAsync(new Context(), tokenCredentials, false).Result;

            Assert.IsTrue(result.IsAuthenticated);
            Assert.AreEqual(AuthenticationResultCode.Successful, result.ResultCode);
        }

        [Test]
        public void VerifyUser_IdentityServerUrlNotConfigured_FallbackToDatabaseUserVerification()
        {
            systemConfigs.First().ConfigValue = string.Empty;

            var result = authenticationService.AuthenticateAsync(new Context(), tokenCredentials, false).Result;

            Assert.IsTrue(result.IsAuthenticated);
            Assert.AreEqual(AuthenticationResultCode.Successful, result.ResultCode);
        }

       
    }
}
