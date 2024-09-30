using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CareFusion.Dispensing
{
    public static class StringExtensions
    {
        #region Internationalized String Comparisons

        /// <summary>
        /// Returns whether source starts with searchText, ignoring accent marks. 
        /// e.g. 'e' and 'é' are treated the same.
        /// </summary>
        public static bool StartsWith_AI(this string source, string searchText)
        {
            if (source == null)
                return false;

            const CompareOptions compareOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreWidth;
            string s = source.Length > searchText.Length ? source.Substring(0, searchText.Length) : source;

            return (CultureInfo.CurrentCulture.CompareInfo.Compare(s, searchText, compareOptions) == 0);
        }

        /// <summary>
        /// Returns whether source contains searchText, ignoring accent marks. 
        /// e.g. 'e' and 'é' are treated the same.
        /// </summary>
        public static bool Contains_AI(this string source, string searchText)
        {
            if (source == null)
                return false;

            const CompareOptions compareOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreWidth;

            return (CultureInfo.CurrentCulture.CompareInfo.IndexOf(source, searchText, compareOptions) >= 0);
        }

        /// <summary>
        /// Uses the current culture.
        /// </summary>
        public static bool LocalizedStartsWith(this string source, string value, bool ignoreCase = false)
        {
            if (source == null)
                return false;

            return CultureInfo.CurrentCulture.CompareInfo.IsPrefix(source, value, ignoreCase ? CompareOptions.IgnoreCase | CompareOptions.IgnoreWidth : CompareOptions.IgnoreWidth);
        }

        /// <summary>
        /// Uses the current culture.
        /// </summary>
        public static bool LocalizedEndsWith(this string source, string value, bool ignoreCase = false)
        {
            if (source == null)
                return false;

            return CultureInfo.CurrentCulture.CompareInfo.IsSuffix(source, value,
                ignoreCase ? CompareOptions.IgnoreCase | CompareOptions.IgnoreWidth : CompareOptions.IgnoreWidth);
        }

        /// <summary>
        /// Uses the current culture.
        /// </summary>
        public static bool LocalizedContains(this string source, string value, bool ignoreCase = false)
        {
            if (source == null)
                return false;

            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(source, value, ignoreCase ? CompareOptions.IgnoreCase | CompareOptions.IgnoreWidth : CompareOptions.IgnoreWidth) >= 0;
        }

        /// <summary>
        /// Uses the current culture.
        /// </summary>
        public static bool LocalizedEquals(this string source, string value, bool ignoreCase = false)
        {
            if (source == null)
                return false;
            
            return CultureInfo.CurrentCulture.CompareInfo.Compare(source, value, ignoreCase ? CompareOptions.IgnoreCase | CompareOptions.IgnoreWidth : CompareOptions.IgnoreWidth) == 0;
        }

        #endregion

        public static string Truncate(this string value, int maxLength)
        {
            return string.IsNullOrEmpty(value) ? value : value.Substring(0, Math.Min(value.Length, maxLength));
        }

        public static string TrimControlCodes(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            return Regex.Replace(source, @"\p{Cc}", string.Empty);

            //StringBuilder sb = new StringBuilder();
            //foreach (char c in source)
            //{
            //    if (!char.IsControl(c))
            //    {
            //        sb.Append(c);
            //    }
            //}

            //return sb.ToString();
        }

        public static bool IsEmpty(this string source)
        {
            if (source == null)
                return false;

            return source.Length == 0;
        }
    }
}
