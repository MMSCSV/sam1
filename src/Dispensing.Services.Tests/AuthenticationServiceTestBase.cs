using System.Configuration;
using Pyxis.Core.Data;
using Pyxis.Core.Data.Configuration;

namespace Dispensing.Services.tests
{
    public class AuthenticationServiceTestBase
    {
        public readonly string IdentityServerUrl;
        public readonly string ClientId;
        public readonly string ClientSecret;
        public readonly string ValidUsername;
        public readonly string ValidPassword;

        public AuthenticationServiceTestBase()
        {
            IdentityServerUrl = ConfigurationManager.AppSettings["IdentityServerUrl"];

            ClientId = ConfigurationManager.AppSettings["ClientId"];
            ClientSecret = ConfigurationManager.AppSettings["ClientSecret"];

            ValidUsername = ConfigurationManager.AppSettings["ValidUsername"];
            ValidPassword = ConfigurationManager.AppSettings["ValidPassword"];

            ConnectionScopeFactory.Configure(DataConfigurationSectionReader.ReadConnectionScopeConfiguration());
        }
    }
}
