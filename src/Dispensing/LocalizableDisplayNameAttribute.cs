using System;
using System.ComponentModel;
using System.Reflection;

namespace CareFusion.Dispensing
{
    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Field)]
    public class LocalizableDisplayNameAttribute : DisplayNameAttribute
    {
        private readonly Type _resourceType;
        private readonly string _resourceName;
        private readonly string _sectionName;

        public LocalizableDisplayNameAttribute()
        {
        }

        public LocalizableDisplayNameAttribute(string displayName)
            : base(displayName)
        {
        }

        public LocalizableDisplayNameAttribute(Type resourceType, string resourceName)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");
            Guard.ArgumentNotNullOrEmptyString(resourceName, "resourceName");

            _resourceType = resourceType;
            _resourceName = resourceName;
        }

        public LocalizableDisplayNameAttribute(Type resourceType, string resourceName, string section)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");
            Guard.ArgumentNotNullOrEmptyString(resourceName, "resourceName");

            _resourceType = resourceType;
            _resourceName = resourceName;
            _sectionName = section;
        }

        public override string DisplayName
        {
            get
            {
                try
                {
                    if (_resourceType != null &&
                        !string.IsNullOrEmpty(_resourceName))
                    {
                        PropertyInfo resourceProperty = _resourceType.GetProperty(_resourceName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
                        if (resourceProperty != null)
                            return resourceProperty.GetValue(null, null).ToString();
                    }

                    return base.DisplayName;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public string Section
        {
            get
            {
                if (_resourceType != null &&
                    !string.IsNullOrEmpty(_sectionName))
                {
                    PropertyInfo resourceProperty = _resourceType.GetProperty(_sectionName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);
                    if (resourceProperty != null)
                        return resourceProperty.GetValue(null, null).ToString();
                }

                return string.Empty;
            }
        }
    }
}
