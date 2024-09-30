using System.Resources;
using CareFusion.Dispensing.Resources.Common;

namespace CareFusion.Dispensing.Resources
{
    public class ResourceReader
    {
        public readonly static ResourceManager DefaultResourceManager = CommonResources.ResourceManager;

        public static string GetString(string key)
        {
            string value;
            try
            {
                value = DefaultResourceManager.GetString(key);
            }
            catch
            {
                value = string.Empty;
            }
            return value;
        }

        public static string GetString(string key, ResourceManager rm)
        {
            string value;

            if (rm == null)
                return GetString(key);
            try
            {
                value = rm.GetString(key);               
            }
            catch
            {
                value = string.Empty;
            }
            return value;
        }


        public static string GetString(string key, string fileName)
        {
            ResourceManager rm = null;
            try
            {
                string resourceName = typeof(ResourceReader).Namespace + "." + fileName;
                rm = new System.Resources.ResourceManager(resourceName, typeof(ResourceReader).Assembly);
                return GetString(key, rm);
            }
            catch
            {
                return GetString(key);
            }
        }
    }
}
