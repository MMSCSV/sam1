using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using CareFusion.Dispensing.Contracts;
using Pyxis.IdentityServer.Middleware.Models;

namespace CareFusion.Dispensing.Services.Business
{
    public interface IIdentityServerAuthenticationManager
    {
        Task<AuthenticationResult> AuthenticateAsync(Context context, TokenCredentials credentials, CancellationToken cancellationToken = default);
        Task<UserAccountResponse> GetUserAccountAsync(string accessToken, CancellationToken cancellationToken = default);
        Task<AuthenticationResult> VerifyUserAccountStatus(Context context, TokenCredentials credentials, CancellationToken cancellationToken = default);
        Task<AuthenticationResult> AuthenticateSmartCardUserAsync(Context context, TokenCredentials credentials, X509Certificate2 smartCardCert, CancellationToken cancellationToken = default);
    }
}
