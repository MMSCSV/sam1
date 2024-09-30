using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Services.Cryptography
{
    public static class TextEncryptorFactory
    {
        public static ITextEncryptor GetEncryptor(EncryptionAlgorithmInternalCode encryptionAlgorithm)
        {
            switch(encryptionAlgorithm)
            {
                case EncryptionAlgorithmInternalCode.SHA_1:
                    return new SHA1TextEncryptor();
                case EncryptionAlgorithmInternalCode.SHA_256:
                    return new SHA256TextEncryptor();
                case EncryptionAlgorithmInternalCode.PHA:
                    return new PyxisHashTextEncryptor();
            }

            throw new Exception(
                string.Format(
                "The enryption algorithm {0} is not supported.", encryptionAlgorithm.ToString()));
        }
    }
}
