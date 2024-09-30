using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Encryption.Aes
{
    internal class AesEncryptor : IEncryptor
    {
        // AES IV size is always 16 bytes
        private const int IVSize = 16;

        private readonly IKeyProvider _keyProvider;

        public AesEncryptor(IKeyProvider keyProvider)
        {
            _keyProvider = keyProvider;
        }

        public string Encrypt(string stringToEncrypt, string keyContext)
        {
            // validate args
            if (string.IsNullOrEmpty(stringToEncrypt))
                throw new ArgumentException("stringToEncrypt");

            // get encryption key
            var key = _keyProvider.GetEncryptionKey(keyContext, EncryptionAlgorithmInternalCodes.AES_256);

            // create if key doesn't exist
            if (key == null)
                key = CreateEncryptionKey(keyContext);

            // get bytes
            var dataToEncrypt = Encoding.UTF8.GetBytes(stringToEncrypt);

            byte[] encryptedData;
            using (var provider = new AesManaged())
            {
                // set key
                provider.Key = key.Value;

                using (var ms = new MemoryStream())
                {
                    // write IV to start of stream
                    ms.Write(provider.IV, 0, IVSize);

                    // encrypt
                    using (var cs = new CryptoStream(ms, provider.CreateEncryptor(provider.Key, provider.IV), CryptoStreamMode.Write))
                    {
                        cs.Write(dataToEncrypt, 0, dataToEncrypt.Length);
                    }

                    encryptedData = ms.ToArray();
                }
            }

            // base-64 representation of encrypted data
            return Convert.ToBase64String(encryptedData);
        }

        public string Decrypt(string stringToDecrypt, string keyContext)
        {
            // validate args
            if (string.IsNullOrEmpty(stringToDecrypt))
                throw new ArgumentException("stringToDecrypt");

            // get encryption key
            var key = _keyProvider.GetEncryptionKey(keyContext, EncryptionAlgorithmInternalCodes.AES_256);

            // if none, cannot decrypt
            if (key == null)
                throw new Exception($"Encryption key not found for keyContext: {keyContext}, cannot decrypt data");

            // get bytes
            var encryptedData = Convert.FromBase64String(stringToDecrypt);

            string results;
            using (var provider = new AesManaged())
            {
                // set key
                provider.Key = key.Value;

                using (var msEncrypted = new MemoryStream(encryptedData))
                {
                    // read IV from start of stream
                    var iv = new byte[IVSize];
                    msEncrypted.Read(iv, 0, IVSize);
                    provider.IV = iv;

                    // decrypt
                    using (var cs = new CryptoStream(msEncrypted, provider.CreateDecryptor(), CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs, Encoding.UTF8))
                    {
                        results = sr.ReadToEnd();
                    }
                }
            }

            return results;
        }

        private EncryptionKey CreateEncryptionKey(string keyContext)
        {
            // create new encryption key
            EncryptionKey key;
            using (var provider = new AesManaged())
            {
                key = new EncryptionKey(provider.Key);
            }

            // save
            _keyProvider.SaveEncryptionKey(keyContext, EncryptionAlgorithmInternalCodes.AES_256, key);
            return key;
        }
    }
}
