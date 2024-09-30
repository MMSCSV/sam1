using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Services.Business;
using CareFusion.Dispensing.Services;
using Pyxis.Core.Data.InternalCodes;
using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Dispensing.Services.tests;
using System.Linq;
using System.Threading;

namespace Dispensing.Services.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class DeviceAuthenticationServiceUnitTests : AuthenticationServiceTestBase
    {
        private IDeviceAuthenticationService _deviceAuthenticationService;
        private TokenCredentials _tokenCredentials;
        private Mock<IAuthenticationManager> _authenticationManagerMock;

        [SetUp]
        public void Setup()
        {
            _tokenCredentials = new TokenCredentials
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
            _authenticationManagerMock = new Mock<IAuthenticationManager>();
            _deviceAuthenticationService = new DeviceAuthenticationService(_authenticationManagerMock.Object);
        }

        [Test]
        public void AuthenticateUser_MatchScanCode_ADUser()
        {
            //initialize
            var context = new Context()
            {
                Device = new DeviceContextInfo()
                {
                    Key = new Guid()
                }
            };
            var userAccounts = new List<AuthUserAccount>
           {
              new AuthUserAccount
              {
               UserId = "admin",
               IsSupportUser = false,
               Key = Guid.NewGuid(),
               ActiveDirectoryDomainKey = Guid.NewGuid()
              }
           };
            var domain = new ActiveDirectoryDomain
            {
                FullyQualifiedName = "mmspyxis.io"
            };
            var authResult = new AuthenticationResult
            {
                ResultCode = AuthenticationResultCode.Successful,
                AuthUserAccount = userAccounts.First()
            };

            //arrange
            _authenticationManagerMock.Setup(x => x.GetAuthUserAccount(_tokenCredentials.UserId, true)).Returns(userAccounts);
            _authenticationManagerMock.Setup(x => x.AuthenticateUserAsync(context, userAccounts.First(), _tokenCredentials, CancellationToken.None).Result).Returns(authResult);
            _authenticationManagerMock.Setup(x => x.GetActiveDirectoryDomain(userAccounts.First().ActiveDirectoryDomainKey.Value)).Returns(domain);

            //act
            _deviceAuthenticationService.Authenticate(context, _tokenCredentials, true);

            //assert
            Assert.AreEqual("admin@mmspyxis.io", _tokenCredentials.UserId);
        }

        [Test]
        public void AuthenticateUser_MatchScanCode_LocalUser()
        {
            //initialize
            var context = new Context()
            {
                Device = new DeviceContextInfo()
                {
                    Key = new Guid()
                }
            };

            //Set the token credentials as different from the user Id.
            _tokenCredentials.UserId = "1234";

            var userAccounts = new List<AuthUserAccount>
           {
              new AuthUserAccount
              {
               UserId = "admin",
               IsSupportUser = false,
               Key = Guid.NewGuid(),
               ActiveDirectoryDomainKey = null
              }
           };

            var authResult = new AuthenticationResult
            {
                ResultCode = AuthenticationResultCode.Successful,
                AuthUserAccount = userAccounts.First()
            };

            //arrange
            _authenticationManagerMock.Setup(x => x.GetAuthUserAccount(_tokenCredentials.UserId, true)).Returns(userAccounts);
            _authenticationManagerMock.Setup(x => x.AuthenticateUserAsync(context, userAccounts.First(), _tokenCredentials, CancellationToken.None).Result).Returns(authResult);

            //act
            _deviceAuthenticationService.Authenticate(context, _tokenCredentials, true);

            //assert
            Assert.AreEqual("admin", _tokenCredentials.UserId);
        }

        [Test]
        public void AuthenticateUser_TokenCredentials_ADUser()
        {
            //initialize
            var context = new Context()
            {
                Device = new DeviceContextInfo()
                {
                    Key = new Guid()
                }
            };
            var userAccounts = new List<AuthUserAccount>
           {
              new AuthUserAccount
              {
               UserId = "admin",
               IsSupportUser = false,
               Key = Guid.NewGuid(),
               ActiveDirectoryDomainKey = Guid.NewGuid()
              }
           };
            var domain = new ActiveDirectoryDomain
            {
                FullyQualifiedName = "mmspyxis.io"
            };
            var authResult = new AuthenticationResult
            {
                ResultCode = AuthenticationResultCode.Successful,
                AuthUserAccount = userAccounts.First()
            };

            //arrange
            _authenticationManagerMock.Setup(x => x.AuthenticateUserAsync(context, userAccounts.First(), _tokenCredentials, CancellationToken.None).Result).Returns(authResult);
            _authenticationManagerMock.Setup(x => x.GetActiveDirectoryDomain(userAccounts.First().ActiveDirectoryDomainKey.Value)).Returns(domain);

            //act
            _deviceAuthenticationService.Authenticate(context, _tokenCredentials, userAccounts.First());

            //assert
            Assert.AreEqual("admin@mmspyxis.io", _tokenCredentials.UserId);
        }

        [Test]
        public void AuthenticateUser_TokenCredentials_LocalUser()
        {
            //initialize
            var context = new Context()
            {
                Device = new DeviceContextInfo()
                {
                    Key = new Guid()
                }
            };
            var userAccounts = new List<AuthUserAccount>
           {
              new AuthUserAccount
              {
               UserId = "admin",
               IsSupportUser = false,
               Key = Guid.NewGuid(),
               ActiveDirectoryDomainKey = null
              }
           };

            var authResult = new AuthenticationResult
            {
                ResultCode = AuthenticationResultCode.Successful,
                AuthUserAccount = userAccounts.First()
            };

            //arrange
            _authenticationManagerMock.Setup(x => x.AuthenticateUserAsync(context, userAccounts.First(), _tokenCredentials, CancellationToken.None).Result).Returns(authResult);

            //act
            _deviceAuthenticationService.Authenticate(context, _tokenCredentials, userAccounts.First());

            //assert
            Assert.AreEqual("admin", _tokenCredentials.UserId);
        }

        [Test]
        public void AuthenticateUser_MultipleUserID_ActiveADUser_InactiveLocalUser()
        {
            //initialize
            var context = new Context()
            {
                Device = new DeviceContextInfo()
                {
                    Key = new Guid()
                }
            };
            var userAccounts = new List<AuthUserAccount>
           {
              new AuthUserAccount
              {
               UserId = "admin",
               IsSupportUser = false,
               Key = Guid.NewGuid(),
               IsActive = true,
               ActiveDirectoryDomainKey = Guid.NewGuid()
              },
               new AuthUserAccount
              {
               UserId = "admin",
               IsSupportUser = false,
               Key = Guid.NewGuid(),
               IsActive = false,
               ActiveDirectoryDomainKey = null
              }
           };
            var domain = new ActiveDirectoryDomain
            {
                FullyQualifiedName = "mmspyxis.io",
                IsActive = true,
                Key = (Guid)userAccounts.First().ActiveDirectoryDomainKey
            };
            var authResult = new AuthenticationResult
            {
                ResultCode = AuthenticationResultCode.Successful,
                AuthUserAccount = userAccounts.First()
            };

            //arrange
            _authenticationManagerMock.Setup(x => x.GetAuthUserAccount(_tokenCredentials.UserId, false)).Returns(userAccounts);
            _authenticationManagerMock.Setup(x => x.AuthenticateUserAsync(context, userAccounts.First(), _tokenCredentials, CancellationToken.None).Result).Returns(authResult);
            _authenticationManagerMock.Setup(x => x.GetActiveDirectoryDomain(userAccounts.First().ActiveDirectoryDomainKey.Value)).Returns(domain);
            _authenticationManagerMock.Setup(x => x.GetActiveDirectoryDomains()).Returns(new List<ActiveDirectoryDomain> { domain });


            //act
            _deviceAuthenticationService.Authenticate(context, _tokenCredentials, false);

            //assert
            Assert.AreNotEqual(ValidUsername, _tokenCredentials.UserId);
            Assert.AreEqual("admin@mmspyxis.io", _tokenCredentials.UserId);
        }

        [Test]
        public void AuthenticateUser_VerifyUserAccountStatus_MultipleUserID_ActiveADUser_InactiveLocalUser()
        {
            //initialize
            var context = new Context()
            {
                Device = new DeviceContextInfo()
                {
                    Key = new Guid()
                }
            };
            var userAccounts = new List<AuthUserAccount>
           {
              new AuthUserAccount
              {
               UserId = "admin",
               IsSupportUser = false,
               Key = Guid.NewGuid(),
               IsActive = true,
               ActiveDirectoryDomainKey = Guid.NewGuid()
              },
               new AuthUserAccount
              {
               UserId = "admin",
               IsSupportUser = false,
               Key = Guid.NewGuid(),
               IsActive = false,
               ActiveDirectoryDomainKey = null
              }
           };
            var domain = new ActiveDirectoryDomain
            {
                FullyQualifiedName = "mmspyxis.io",
                IsActive = true,
                Key = (Guid)userAccounts.First().ActiveDirectoryDomainKey
            };
            var authResult = new AuthenticationResult
            {
                ResultCode = AuthenticationResultCode.Successful,
                AuthUserAccount = userAccounts.First()
            };

            //arrange
            _authenticationManagerMock.Setup(x => x.GetAuthUserAccount(_tokenCredentials.UserId, false)).Returns(userAccounts);
            _authenticationManagerMock.Setup(x => x.AuthenticateUserAsync(context, userAccounts.First(), _tokenCredentials, CancellationToken.None).Result).Returns(authResult);
            _authenticationManagerMock.Setup(x => x.GetActiveDirectoryDomain(userAccounts.First().ActiveDirectoryDomainKey.Value)).Returns(domain);
            _authenticationManagerMock.Setup(x => x.GetActiveDirectoryDomains()).Returns(new List<ActiveDirectoryDomain> { domain });


            //act
            _deviceAuthenticationService.VerifyUserAccountStatus(context, _tokenCredentials, false);

            //assert
            Assert.AreNotEqual(ValidUsername, _tokenCredentials.UserId);
            Assert.AreEqual("admin@mmspyxis.io", _tokenCredentials.UserId);
        }
    }
}
