using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data.Repositories;
using CareFusion.Dispensing.Resources;
using CareFusion.Dispensing.Services.Business;
using IdentityModel;
using IdentityModel.Client;
using Moq;
using NUnit.Framework;
using Pyxis.Core.Data.InternalCodes;
using Pyxis.Core.Data.Schema.Core.Models;
using Pyxis.IdentityServer.Middleware.Modules;

namespace Dispensing.Services.tests.Business
{
    [TestFixture]
    [Category("Unit")]
    public class IdentityServerAuthenticationManagerUnitTests
    {
        // ReSharper disable once UnusedMember.Local
        private const string RawTokenResponse = @"{""access_token"":""eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Imo4UjhoNnNhMjFPM2E5M1FfZjFKaFl2SVlZMCIsImtpZCI6Imo4UjhoNnNhMjFPM2E5M1FfZjFKaFl2SVlZMCJ9.eyJpc3MiOiJodHRwczovL3d3dy5iZC5jb20vcHl4aXMvaWRlbnRpdHkiLCJhdWQiOiJodHRwczovL3d3dy5iZC5jb20vcHl4aXMvaWRlbnRpdHkvcmVzb3VyY2VzIiwiZXhwIjoxNTQxNzIzMzI1LCJuYmYiOjE1NDE3MTk3MjUsImNsaWVudF9pZCI6Ik1TX2IwMjJkNGM4LWM0ODAtNGQxOS05YmIyLTQxOGY2ODY4ZTVmYSIsInNjb3BlIjpbIm9wZW5pZCIsInByb2ZpbGUiXSwic3ViIjoiY2RjMjU1Y2Y5MzUzNDIxZDk4MTdlYjBhNTJjMGJkNjEiLCJhdXRoX3RpbWUiOjE1NDE3MTk3MjUsImlkcCI6Imlkc3J2IiwibmFtZSI6ImFkbWluIiwiZmFtaWx5X25hbWUiOiJBZG1pbmlzdHJhdG9yIiwiZ2l2ZW5fbmFtZSI6IlN5c3RlbSIsInByb2ZpbGUiOiJ7XCJVc2VyQWNjb3VudEtleVwiOlwiY2RjMjU1Y2YtOTM1My00MjFkLTk4MTctZWIwYTUyYzBiZDYxXCIsXCJVc2VySWRcIjpcImFkbWluXCIsXCJFbWFpbFwiOm51bGwsXCJGaXJzdE5hbWVcIjpcIlN5c3RlbVwiLFwiTWlkZGxlTmFtZVwiOm51bGwsXCJMYXN0TmFtZVwiOlwiQWRtaW5pc3RyYXRvclwiLFwiSXNTdXBwb3J0VXNlclwiOmZhbHNlfSIsImFtciI6WyJwYXNzd29yZCJdfQ.l9Xw2wnYNa3IdDWB2WnOVgRTz1H75IFAoxK6DRDo9jLcAn8yTdaprAUcrC46MG9x5YpAx9tksJXkfMa_YDjkfHhN5p0jV6RLbf5l1L21M0H_1eLazovRoH5SgYtIaE6vfQZ3l3jNTzG4teRk4coDNmM9UBLMRP0-Txn9Xqxzuy0Djwc4cthA-7N2Ywerw0UQYfM9_CRvw4HnTdtF7htvRqsQ79Qi0WEWGnbWzJXp7dmHYUJXW8mkx73uzpEpa-KMdRliqjt7FFuLcQldT3ayVJpK5ssxhdGNb_lsoPHgpzZ1im26GVUljIi6r-LRUoMQkCIJNKRGTbrh11PuNdmyPg"",""expires_in"":90000,""token_type"":""Bearer""}";
        private const string RawErrorResponse = @"{""error"":""invalid_client""}";
        private const string certText = "MIIDozCCAougAwIBAgIQcWrSiHp1nIlHRRh5ENkj2zANBgkqhkiG9w0BAQsFADBRMRMwEQYKCZImiZPyLGQBGRYDY29tMRIwEAYKCZImiZPyLGQBGRYCYmQxFTATBgNVBAsMDFVzZXJBY2NvdW50czEPMA0GA1UEAwwGYmQuY29tMB4XDTIxMDgwNDA1MzE0NFoXDTIyMDgwNDA1NTE0NFowUTETMBEGCgmSJomT8ixkARkWA2NvbTESMBAGCgmSJomT8ixkARkWAmJkMRUwEwYDVQQLDAxVc2VyQWNjb3VudHMxDzANBgNVBAMMBmJkLmNvbTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKAixQgOCrkeKwuWOkkgsfgCawwj2zF6Hk6uEbRgarRg8OahcBn3gbrYYc7wLh6gsnXSeJmAKIJMrQb47hg3T6c517pOZRJ7IR4i4n8LRl5HID5j8LZ3Mm9Y62QajQ4T57jr9F5Su0K6Jp6f077znfZVmAWiVlOAxJkm0Haup10HHcWoC3sYSVf690eR/J7bG8iT6AzkXae18i62Pdcwwkii795pDYtFRyHln4JuH/xtQ1G0prYmzLYoYPIZoC3Pxa16FZd8V3kvSi78ZmJuqs56gAq+lfOYwk4fi9aCsn43OdnXg7E6PrGxF8//HtrXs9rCx3YwyUH6B1awJWaswKECAwEAAaN3MHUwDgYDVR0PAQH/BAQDAgKEMBMGA1UdJQQMMAoGCCsGAQUFBwMCMC8GA1UdEQQoMCagJAYKKwYBBAGCNxQCA6AWDBRtYXJpby5zYWd1Y2lvQGJkLmNvbTAdBgNVHQ4EFgQUny2iQ5474R4Qwk6wvhZnUz4ou9EwDQYJKoZIhvcNAQELBQADggEBAA9Na4WmxI0C8DXJ35mF2N9pwya/PszBDgeI5HaK/vpSrJneC9EmCEdoNntIJKFrMlKXFiO8T8xX2TxHkILgfqr+nKS3suCqtU/1PYqgFqMQblkOryhPdbVQeiY6+apjsSaeQ1TotefTWsWNWUrtFIRuK19+Jn4KShcIK4FLvhfKcfnT+dAlM18FCVDk34pMZUMJgUsMJhR8IOqWVNqTOlAu+50SLMu5bPQ+C/ctkXJ58wyN+YkVMedL7ViCfoCQTu2AD24XWKSGGUXINNvPsnZbLnkTxvxZSLjI0bcKvGG6oCZZAS5HxAlOc2QfCGEri4WN9w9vv7YPFR0jCe3Zcj4=";


        private IdentityServerAuthenticationManager idsManager;
        private TokenCredentials tokenCredentials;

        private Mock<IIdentityServerMiddleware> middlewareMock;
        private Mock<ISystemConfigurationRepository> systemConfigurationMock;
        private const string IdsUrl = "https://localhost/identity";
        private SystemConfig systemConfig;

        [SetUp]
        public void SetUp()
        {
            middlewareMock = new Mock<IIdentityServerMiddleware>();
            systemConfigurationMock = new Mock<ISystemConfigurationRepository>();

            systemConfig = new SystemConfig
            {
                SystemConfigTypeInternalCode = SystemConfigTypeInternalCode.IdentityServerURL.ToInternalCode(),
                ConfigValue = IdsUrl
            };

            tokenCredentials = new TokenCredentials
            {
                AuthenticationMethod = AuthenticationMethodInternalCode.UIDPWD,
                AuthenticationPurpose = AuthenticationPurposeInternalCode.SI,
                ClientId = "es_station",
                ClientSecret = "83CFA7C3-6953-4C63-AC75-C1525A380F98",
                GrantType = "password",
                Scope = "profile openid",
                UserId = "admin",
                Password = "qa"
            };

            idsManager = new TestIdentityServerAuthenticationManager(systemConfigurationMock.Object, middlewareMock.Object);
        }

        private void Initialize(Pyxis.IdentityServer.Middleware.Models.Response<TokenResponse> tr)
        {
            systemConfigurationMock.Setup(s => s.GetSystemConfiguration(It.IsAny<string>())).Returns(systemConfig);
            middlewareMock.Setup(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None))
                          .ReturnsAsync(tr);
        }
        private void Initialize_SmartCard(Pyxis.IdentityServer.Middleware.Models.Response<TokenResponse> tr)
        {
            systemConfigurationMock.Setup(s => s.GetSystemConfiguration(It.IsAny<string>())).Returns(systemConfig);
            middlewareMock.Setup(m => m.VerifySmartCardUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<X509Certificate2>(), It.IsAny<string>(), CancellationToken.None))
                          .ReturnsAsync(tr);
        }

        [Test]
        public async Task AuthenticateTest_Successful()
        {
            // Arrange
            Initialize(new Pyxis.IdentityServer.Middleware.Models.Response<TokenResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Data = await GenerateTokenResponseAsync(HttpStatusCode.OK, RawTokenResponse)
            });

            // Act
            var authenticationResult = await idsManager.AuthenticateAsync(new Context(), tokenCredentials);

            // Assert
            Assert.AreEqual(AuthenticationResultCode.Successful, authenticationResult.ResultCode);
        }

        [Test]
        public async Task AuthenticateTest_InvalidClient()
        {
            // Arrange
            Initialize(new Pyxis.IdentityServer.Middleware.Models.Response<TokenResponse>
            {
                StatusCode = HttpStatusCode.BadRequest,
                StatusMessage = OidcConstants.TokenErrors.InvalidClient,
                Data = await GenerateTokenResponseAsync(HttpStatusCode.BadRequest, RawErrorResponse, OidcConstants.TokenErrors.InvalidClient)
            });

            // Act
            var authenticationResult = await idsManager.AuthenticateAsync(new Context(), tokenCredentials);

            // Assert
            Assert.AreEqual(AuthenticationResultCode.AuthenticationFailed, authenticationResult.ResultCode);
            Assert.AreEqual(ServiceResources.SignInFailure_InvalidUserIdPassword, authenticationResult.ErrorMessage);
        }

        [Test]
        public async Task AuthenticateTest_IDSNotConfigured()
        {
            systemConfig.ConfigValue = string.Empty;

            var authenticationResult = await idsManager.AuthenticateAsync(new Context(), tokenCredentials);

            Assert.AreEqual(AuthenticationResultCode.IdentityServerUrlNotConfigured, authenticationResult.ResultCode);
            Assert.AreEqual(ServiceResources.SignInFailure_IdentityServerUrl, authenticationResult.ErrorMessage);
        }

        [Test]
        public async Task AuthenticateTest_SmartCardUser_ValidUser()
        {
            var smartCardCert = new X509Certificate2(Convert.FromBase64String(certText));
            tokenCredentials = new TokenCredentials
            {
                AuthenticationMethod = AuthenticationMethodInternalCode.CardPIN,
                AuthenticationPurpose = AuthenticationPurposeInternalCode.SI,
                ClientId = "es_station",
                ClientSecret = "83CFA7C3-6953-4C63-AC75-C1525A380F98",
                GrantType = "password",
                Scope = "profile openid",
                UserId = "admin",
            };
            // Arrange
            Initialize_SmartCard(new Pyxis.IdentityServer.Middleware.Models.Response<TokenResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Data = await GenerateTokenResponseAsync(HttpStatusCode.OK, RawTokenResponse)
            });
           var authenticationResult = await idsManager.AuthenticateSmartCardUserAsync(new Context(), tokenCredentials, smartCardCert);
           Assert.AreEqual(AuthenticationResultCode.Successful, authenticationResult.ResultCode);
        }

        [Test]
        public async Task AuthenticateTest_SmartCardUser_InvalidUser()
        {
            var smartCardCert = new X509Certificate2(Convert.FromBase64String(certText));
            tokenCredentials = new TokenCredentials
            {
                AuthenticationMethod = AuthenticationMethodInternalCode.CardPIN,
                AuthenticationPurpose = AuthenticationPurposeInternalCode.SI,
                ClientId = "es_station",
                ClientSecret = "83CFA7C3-6953-4C63-AC75-C1525A380F98",
                GrantType = "password",
                Scope = "profile openid",
                UserId = "admin",
            };
            // Arrange
            Initialize_SmartCard(new Pyxis.IdentityServer.Middleware.Models.Response<TokenResponse>
            {
                StatusCode = HttpStatusCode.Forbidden,
                StatusMessage = OidcConstants.TokenErrors.InvalidClient,
                Data = await GenerateTokenResponseAsync(HttpStatusCode.Forbidden, RawErrorResponse, OidcConstants.TokenErrors.InvalidClient)
            });
           var authenticationResult = await idsManager.AuthenticateSmartCardUserAsync(new Context(), tokenCredentials, smartCardCert);
           Assert.AreEqual(AuthenticationResultCode.SmartCardCertificateRevokedOrInvalid, authenticationResult.ResultCode);
        }
       [Test]
        public async Task AuthenticateTest_SmartCard_Certificate()
        {
            var smartCardCert = new X509Certificate2(Convert.FromBase64String(certText));
            tokenCredentials = new TokenCredentials
            {
                AuthenticationMethod = AuthenticationMethodInternalCode.CardPIN,
                AuthenticationPurpose = AuthenticationPurposeInternalCode.SI,
                ClientId = "es_station",
                ClientSecret = "83CFA7C3-6953-4C63-AC75-C1525A380F98",
                GrantType = "password",
                Scope = "profile openid",
            };
            // Arrange
            Initialize_SmartCard(new Pyxis.IdentityServer.Middleware.Models.Response<TokenResponse>
            {
               StatusCode = HttpStatusCode.OK,
               Data = await GenerateTokenResponseAsync(HttpStatusCode.OK, RawTokenResponse)
            });
            var authenticationResult = await idsManager.AuthenticateSmartCardUserAsync(new Context(), tokenCredentials, smartCardCert);
            Assert.AreEqual(AuthenticationResultCode.Successful, authenticationResult.ResultCode);
        }

        private async Task<TokenResponse> GenerateTokenResponseAsync(HttpStatusCode statusCode, string rawContent, string reasonPhrase = null)
        {
            TokenResponse tokenResponse = default;

            using (var httpResponseMessage = new HttpResponseMessage())
            {
                httpResponseMessage.StatusCode = statusCode;
                httpResponseMessage.Content = new StringContent(rawContent);
                httpResponseMessage.ReasonPhrase = reasonPhrase ?? string.Empty;
                tokenResponse = await ProtocolResponse.FromHttpResponseAsync<TokenResponse>(httpResponseMessage);
            }

            return tokenResponse;
        }
    }

    public class TestIdentityServerAuthenticationManager : IdentityServerAuthenticationManager
    {
        public TestIdentityServerAuthenticationManager(ISystemConfigurationRepository systemConfigurationRepository, IIdentityServerMiddleware idsMiddleware)
            : base(systemConfigurationRepository)
        {
            IdsMiddleware = idsMiddleware;
        }
    }
}
