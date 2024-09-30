namespace CareFusion.Dispensing.Encryption
{
    public interface IKeyProvider
    {
        EncryptionKey GetEncryptionKey(
            string keyContext,
            string algorithm);

        void SaveEncryptionKey(
            string keyContext,
            string algorithm,
            EncryptionKey key);
    }
}
