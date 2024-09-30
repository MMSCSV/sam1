using System.Collections.Generic;
using System.Linq;
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
    public class WebAuthenticationServiceIntegrationTests : AuthenticationServiceTestBase
    {
        private TokenCredentials tokenCredentials;

        private SystemConfigurationStub systemConfigurationStub;


        private IWebAuthenticationService authenticationService;
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

            IAuthenticationManager authManager = new WebAuthenticationManager(new DispensingSystemRepository(), new AuthenticationEventRepository(), idsManager);
            authenticationService = new WebAuthenticationService(authManager);
        }

        [Test]
        public void AuthenticateUser()
        {
            var result = authenticationService.Authenticate(new Context(), tokenCredentials);
            
            Assert.AreEqual(AuthenticationResultCode.Successful, result.ResultCode);
            Assert.IsTrue(result.IsAuthenticated);
        }

        [Test]
        public void AuthenticateUser_IdentityServerUrlNotConfigured_FallbackToOldAuthentication()
        {
            systemConfigs.First().ConfigValue = string.Empty;

            var result = authenticationService.Authenticate(new Context(), tokenCredentials);

            Assert.IsTrue(result.IsAuthenticated);
            Assert.AreEqual(AuthenticationResultCode.Successful, result.ResultCode);
        }

        [Test]
        public void AuthenticateUser_Fail()
        {
            tokenCredentials.Password = "bad_password";

            var result = authenticationService.Authenticate(new Context(), tokenCredentials);

            Assert.IsFalse(result.IsAuthenticated);
            Assert.AreEqual(AuthenticationResultCode.AuthenticationFailed, result.ResultCode);
        }

        [Test]
        public void AuthenticateUser_UserNotFound()
        {
            tokenCredentials.UserId = "not_found_user";

            var result = authenticationService.Authenticate(new Context(), tokenCredentials);

            Assert.IsFalse(result.IsAuthenticated);
            Assert.AreEqual(AuthenticationResultCode.AuthenticationFailed, result.ResultCode);
        }
    }
}
