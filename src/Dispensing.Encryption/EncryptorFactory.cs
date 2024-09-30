using CareFusion.Dispensing.Encryption.Aes;

namespace CareFusion.Dispensing.Encryption
{
    public sealed class EncryptorFactory
    {
        public static IEncryptor CreateEncryptor()
        {
            return CreateEncryptor(new DatabaseKeyProvider());
        }

        public static IEncryptor CreateEncryptor(IKeyProvider keyProvider)
        {
            return new AesEncryptor(keyProvider);
        }
    }
}
