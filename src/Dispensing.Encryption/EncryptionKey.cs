namespace CareFusion.Dispensing.Encryption
{
    public class EncryptionKey
    {
        public EncryptionKey(byte[] value)
        {
            Value = value;
        }

        public byte[] Value { get; }
    }
}
