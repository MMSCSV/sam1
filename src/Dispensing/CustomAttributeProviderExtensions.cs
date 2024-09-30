using System;
using System.Reflection;

namespace CareFusion.Dispensing
{
    public static class CustomAttributeProviderExtensions
    {
        public static bool HasCustomAttributes<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            return provider.HasCustomAttributes<T>(true);
        }

        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider provider) where T : Attribute
        {
            return GetCustomAttributes<T>(provider, true);
        }

        public static bool HasCustomAttributes<T>(this ICustomAttributeProvider provider, bool inherit) where T : Attribute
        {
            return provider.GetCustomAttributes<T>(inherit).Length > 0;
        }

        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider provider, bool inherit) where T : Attribute
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            T[] attributes = provider.GetCustomAttributes(typeof(T), inherit) as T[];
            
            // WORKAROUND: Due to a bug in the code for retrieving attributes
            // from a dynamic generated parameter, GetCustomAttributes can return
            // an instance of an object[] instead of T[], and hence the cast above
            // will return null.
            return attributes ?? new T[0];
        }
    }
}
