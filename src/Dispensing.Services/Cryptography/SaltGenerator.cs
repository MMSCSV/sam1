using System;
using System.Security.Cryptography;

namespace CareFusion.Dispensing.Services.Cryptography
{
    public static class SaltGenerator
    {
        public static string GenerateSalt()
        {
            var salt = new byte[4]; // 128 random bits

            var cryptoRNG = new RNGCryptoServiceProvider();
            cryptoRNG.GetBytes(salt);

            return Convert.ToBase64String(salt);
        }
    }
}
