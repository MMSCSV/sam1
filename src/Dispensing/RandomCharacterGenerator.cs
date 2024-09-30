using System;
using System.Text;

namespace CareFusion.Dispensing
{
    public static class RandomCharacterGenerator
    {
        private static string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string digits = "0123456789";
        private static string specialChars = "~!@#$%^&*+";

        private static Random rand = new Random();

        public static string GenerateRandomString(int numOfChars, 
            bool useAlphabets, bool useDigits, bool useSpecialChars)
        {
            string charsToUse = null;

            if (useAlphabets)
                charsToUse += alphabet;
            if (useDigits)
                charsToUse += digits;
            if (useSpecialChars)
                charsToUse += specialChars;

            StringBuilder r = new StringBuilder();

            for (int index = 0; index < numOfChars; index++)
            {
                r.Append(charsToUse[rand.Next(charsToUse.Length)]);
            }

            return r.ToString();
        }
    }
}
