namespace CareFusion.Dispensing.Encryption
{
    public interface IEncryptor
    {
        /// <summary>
        /// Encrypt data using selected algorithm
        /// </summary>
        /// <param name="stringToEncrypt">string which should be encrypted</param>
        /// <param name="keyContext">Key context/usage</param>
        /// <returns>encrypted string</returns>
        string Encrypt(string stringToEncrypt, string keyContext);

        /// <summary>
        /// Decrypt data using selected algorithm
        /// </summary>
        /// <param name="stringToDecrypt">string which should be decrypted</param>
        /// <param name="keyContext">Key context/usage</param>
        /// <returns>decrypted string</returns>
        string Decrypt(string stringToDecrypt, string keyContext);
    }
}
