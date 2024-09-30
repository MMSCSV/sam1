using System.Security.Cryptography;
using System.Text;

namespace CareFusion.Dispensing.Services.Cryptography
{
    internal sealed class SHA256TextEncryptor : ITextEncryptor
    {
        #region ITextEncryptor Members

        public string GenerateHash(string text, string salt)
        {
            /*Note: Using SHA256CryptoServiceProvider instead of SHA256Managed since it uses the FIPS 140-2 validated (FIPS = Federal Information Processing Standards) Crypto Service Provider (CSP) 
             * while c does not. SHA256Managed is a pure managed implementation while SHA256CryptoServiceProvider does presumably the same thing but wraps the CryptoAPI.
             * Both will generate same hash.
             * SHA256CryptoServiceProvider uses Operating system cryptographic service providers and even if you have .NET 3.5 installed it will need Windows XP with SP3, 7, or 2008 to work.
             */
            //Bug 380777 (User Authentication - Users can not login to client/server with FIPS validated cryptographic algorithm enabled on system.)
            var cryptoServiceProvider = new SHA256CryptoServiceProvider();

            var bytes = Encoding.UTF8.GetBytes(text + salt);
            var hash = cryptoServiceProvider.ComputeHash(bytes);
            return Encoding.UTF8.GetString(hash);
        }

        public bool IsMatch(string text, string salt, string hash)
        {
            return hash == GenerateHash(text, salt);
        }

        public bool SupportsSalt
        {
            get { return true; }
        }

        #endregion
    }
}
