using System;
using System.Security.Cryptography;
using System.Text;

namespace CareFusion.Dispensing.Services.Cryptography
{
    internal sealed class SHA1TextEncryptor : ITextEncryptor
    {
        #region ITextEncryptor Members

        public string GenerateHash(string text, string salt)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] bytesToHash = Encoding.Default.GetBytes(text + salt);
            byte[] hash = sha1.ComputeHash(bytesToHash);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public bool IsMatch(string text, string salt, string hash)
        {
            string generatedHash = GenerateHash(text, salt);
            return hash == generatedHash;
        }

        public bool SupportsSalt { get { return true; } }

        #endregion
    }
}
