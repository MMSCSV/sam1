using System;
using System.Text;

namespace CareFusion.Dispensing.Services.Cryptography
{
    internal sealed class PyxisHashTextEncryptor : ITextEncryptor
    {
        #region ITextEncryptor Members

        public string GenerateHash(string text, string salt)
        {
            string converted = "";
            int len = 0;
            foreach (char chr in text)
            {
                if (cva.IndexOf(Char.ToUpper(chr)) >= 0)
                {
                    converted += Char.ToUpper(chr);
                    if (++len == PLAIN_LEN)
                        break;
                }
            }
            byte[] bytesToHash = Encoding.Default.GetBytes(converted);
            byte[] hash = pw_encode(bytesToHash);
            return Encoding.Default.GetString(hash);
        }

        public bool IsMatch(string text, string salt, string hash)
        {
            string generatedHash = GenerateHash(text, salt);
            return hash == generatedHash;
        }

        public bool SupportsSalt { get { return false; } }

        #endregion

        #region Encryption implementation

        private const int PLAIN_LEN = 8;
        private const int ENCODED_LEN = 8;
        private const int BASE1 = 31;
        private const int BASE2 = 37;
        private const int BASE3 = 37 * 37;
        private const string cva = @"QAZW0SX1ED2CR4FV3TG5BY6HN7U J8MI9KOLP";

        internal byte[] pw_encode(byte[] plaintext)
        {
            byte[] cvb = Encoding.Default.GetBytes("WHGTYDFUIRNELPAC");
            byte[] encoded = Encoding.Default.GetBytes("        ");
            UInt16 val = 0;

            int index = 0;
            foreach (byte plain in plaintext)
                encoded[ENCODED_LEN - plaintext.Length + index++] = plain;

            string encodedStr = Encoding.Default.GetString(encoded);

            if (plaintext.Length == 8)
            {
                val += (UInt16)((cva.IndexOf(encodedStr[0]) + 47) * BASE3);

            }
            if (plaintext.Length >= 7)
            {
                val += (UInt16)((cva.IndexOf(encodedStr[1]) + 51) * BASE1);
            }
            val += (UInt16)cva.IndexOf(encodedStr[2]);
            val += (UInt16)(cva.IndexOf(encodedStr[3]) * BASE2);
            val += (UInt16)(cva.IndexOf(encodedStr[4]) * BASE3);

            encoded[0] = cvb[val & 0x000F];
            val >>= 4;
            encoded[2] = cvb[val & 0x000F];
            val >>= 4;
            encoded[1] = cvb[val & 0x000F];
            val >>= 4;
            encoded[4] = cvb[val];

            val = (UInt16)cva.IndexOf(encodedStr[5]);
            val += (UInt16)(cva.IndexOf(encodedStr[6]) * BASE2);
            val += (UInt16)(cva.IndexOf(encodedStr[7]) * BASE3);

            encoded[7] = cvb[val & 0x000F];
            val >>= 4;
            encoded[3] = cvb[val & 0x000F];
            val >>= 4;
            encoded[5] = cvb[val & 0x000F];
            val >>= 4;
            encoded[6] = cvb[val];

            Array.Reverse(encoded);
            return encoded;
        }

        #endregion
    }
}
