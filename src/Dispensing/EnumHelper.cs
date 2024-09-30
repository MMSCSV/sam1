using System;

namespace CareFusion.Dispensing
{
    public static class EnumHelper
    {
        public static T[] GetValues<T>()
        {
            return (T[]) Enum.GetValues(typeof (T));
        }

        public static T EnumParse<T>(this string value)
        {
            return EnumParse<T>(value, true);
        }

        public static T EnumParse<T>(this string value, bool ignoreCase)
        {
            if(String.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            if(!typeof(T).IsEnum)
                throw new ArgumentException("Must provide an enum type");

            return (T) Enum.Parse(typeof (T), value, ignoreCase);
        }
    }
}
