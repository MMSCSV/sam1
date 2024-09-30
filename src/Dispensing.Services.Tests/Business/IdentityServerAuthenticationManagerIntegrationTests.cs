using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Services.Business;
using Dispensing.Services.tests.Stubs.SystemConfiguration;
using NUnit.Framework;
using Pyxis.Core.Data.InternalCodes;
using Pyxis.Core.Data.Schema.Core.Models;

namespace Dispensing.Services.tests.Business
{
    [TestFixture]
    [Category("Integration")]
    public class IdentityServerAuthenticationManagerIntegrationTests : AuthenticationServiceTestBase
    {
        private IdentityServerAuthenticationManager idsManager;
        private TokenCredentials tokenCredentials;
        private List<SystemConfig> systemConfigs;

        [SetUp]
        public void SetUp()
        {
            systemConfigs = new List<SystemConfig>
            {
                new SystemConfig
                {
                    SystemConfigTypeInternalCode = SystemConfigTypeInternalCode.IdentityServerURL.ToInternalCode(),
                    ConfigValue = IdentityServerUrl
                }
            };

            tokenCredentials = new TokenCredentials
            {
                AuthenticationMethod = AuthenticationMethodInternalCode.UIDPWD,
                AuthenticationPurpose = AuthenticationPurposeInternalCode.SI,
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                GrantType = "password",
                Scope = "profile openid",
                UserId = ValidUsername,
                Password = ValidPassword
            };
        }

        private void Initialize()
        {
            var systemConfigurationStub = new SystemConfigurationStub(systemConfigs);
            idsManager = new IdentityServerAuthenticationManager(systemConfigurationStub);
        }

        [Test]
        public async Task AuthenticateTest_Successful()
        {
            Initialize();

            var authenticationResult = await idsManager.AuthenticateAsync(new Context(), tokenCredentials);

            Assert.AreEqual(AuthenticationResultCode.Successful, authenticationResult.ResultCode);

            var response = await idsManager.GetUserAccountAsync(authenticationResult.AccessToken);
            Assert.AreNotEqual(response.UserProfile.UserAccountKey, default(Guid));
        }

        [Test]
        public async Task AuthenticateTest_InvalidClientId()
        {
            tokenCredentials.ClientId = "invalid";
            Initialize();

            var authenticationResult = await idsManager.AuthenticateAsync(new Context(), tokenCredentials);

            Assert.AreEqual(AuthenticationResultCode.AuthenticationFailed, authenticationResult.ResultCode);
        }

        [Test]
        public async Task AuthenticateTest_InvalidClientSecret()
        {
            tokenCredentials.ClientSecret = "invalid";
            Initialize();

            var authenticationResult = await idsManager.AuthenticateAsync(new Context(), tokenCredentials);

            Assert.AreEqual(AuthenticationResultCode.AuthenticationFailed, authenticationResult.ResultCode);
            Assert.AreEqual(ServiceResources.SignInFailure_InvalidUserIdPassword, authenticationResult.ErrorMessage);
        }

        [Test]
        public async Task AuthenticateTest_InvalidGrantType()
        {
            tokenCredentials.ClientSecret = "invalid";
            Initialize();

            var authenticationResult = await idsManager.AuthenticateAsync(new Context(), tokenCredentials);

            Assert.AreEqual(AuthenticationResultCode.AuthenticationFailed, authenticationResult.ResultCode);
        }

        [Test]
        public async Task AuthenticateTest_InvalidScope()
        {
            tokenCredentials.Scope = "invalid";
            Initialize();

            var authenticationResult = await idsManager.AuthenticateAsync(new Context(), tokenCredentials);

            Assert.AreEqual(AuthenticationResultCode.AuthenticationFailed, authenticationResult.ResultCode);
        }

        [Test]
        public async Task AuthenticateTest_InvalidUser()
        {
            tokenCredentials.UserId = "invalid";
            Initialize();

            var authenticationResult = await idsManager.AuthenticateAsync(new Context(), tokenCredentials);

            Assert.AreEqual(AuthenticationResultCode.AuthenticationFailed, authenticationResult.ResultCode);
        }

        [Test]
        public async Task AuthenticateTest_InvalidPassword()
        {
            tokenCredentials.Password = "invalid";
            Initialize();

            var authenticationResult = await idsManager.AuthenticateAsync(new Context(), tokenCredentials);

            Assert.AreEqual(AuthenticationResultCode.AuthenticationFailed, authenticationResult.ResultCode);
        }

        [Test]
        public async Task AuthenticateTest_IDSNotConfigured()
        {
            systemConfigs.First().ConfigValue = string.Empty;
            Initialize();

            var authenticationResult = await idsManager.AuthenticateAsync(new Context(), tokenCredentials);

            Assert.AreEqual(AuthenticationResultCode.IdentityServerUrlNotConfigured, authenticationResult.ResultCode);
        }
    }
}