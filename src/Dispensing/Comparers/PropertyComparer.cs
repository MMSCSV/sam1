using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CareFusion.Dispensing.Comparers
{
    /// <summary>
    /// Compares a specific public property on two objects for differences
    /// </summary>
    /// <typeparam name="TClass">The type of object to compare</typeparam>
    public class PropertyComparer<TClass> where TClass : class
    {
        /// <summary>
        /// This copy constructor is only to be used if you want to create a new instance of a
        /// subclass from a PropertyComparer
        /// </summary>
        /// <param name="otherComparer"></param>
        protected PropertyComparer(PropertyComparer<TClass> otherComparer)
            : this(otherComparer.Property, otherComparer.Source, otherComparer.Target)
        {
            
        }

        /// <summary>
        /// Create a new property comparer for the speficied property and objects
        /// </summary>
        /// <param name="property">The property to compare</param>
        /// <param name="source">The source object</param>
        /// <param name="target">The target object</param>
        public PropertyComparer(PropertyInfo property, TClass source, TClass target)
        {
            if (property == null) throw new ArgumentNullException("property");
            if (source == null) throw new ArgumentNullException("source");
            if (target == null) throw new ArgumentNullException("target");

            Property = property;
            Source = source;
            Target = target;

            originalSourceValue = SourceValue;
            originalTargetValue = TargetValue;
        }

        private PropertyInfo Property { get; set; }
        private TClass Source { get; set; }
        private TClass Target { get; set; }
        protected string _displayName;
        protected string _targetDisplayName;

        private readonly object originalSourceValue;
        private readonly object originalTargetValue;

        /// <summary>
        /// Returns whether or not the two objects have different values for the property. Returns
        /// false if the property has been decorated with the <see cref="IgnoreDifferencesAttribute"/>
        /// </summary>
        public bool IsChanged
        {
            get
            {
                // Respect the IgnoreDifferences attribute
                if(Property.HasCustomAttributes<IgnoreDifferencesAttribute>())
                    return false;

                var sourceValue = SourceValue;
                var targetValue = TargetValue;

                if (sourceValue == null && targetValue != null)
                    return true;

                if (sourceValue != null && targetValue == null)
                    return true;

                if (sourceValue != null)
                {
                    return !targetValue.Equals(sourceValue);
                }

                return false;
            }
        }

        /// <summary>
        /// The property name
        /// </summary>
        public string PropertyName
        {
            get
            {
                return Property.Name;
            }
        }

        /// <summary>
        /// The localizable display name of the property. This is retrieved from the <see cref="DisplayAttribute"/>
        /// that decorates the property. Otherwise the <see cref="PropertyName"/> is retrieved.
        /// </summary>
        public string DisplayName
        {
            get
            {
                try
                {
                    if (_displayName != null)
                        return _displayName;

                    return Property.GetDisplayName();
                }
                catch (Exception)
                {
                    return PropertyName;
                }
            }

            set
            {
                _displayName = value;
            }
        }

        /// <summary>
        /// The value of the property in the source object
        /// </summary>
        public object SourceValue
        {
            get
            {
                return Property.GetValue(Source, null);
            }
        }

        public string SourceValueDisplay
        {
            get {
                return GetDisplayName(SourceValue);
            }
        }

        private static string GetDisplayName(object value)
        {
            return value == null ? string.Empty : value.ToString();
        }


        /// <summary>
        /// The value of the property in the target object
        /// </summary>
        public object TargetValue
        {
            get
            {
                return Property.GetValue(Target, null);
            }
        }

        public string TargetValueDisplay
        {
            get
            {
                if (_targetDisplayName != null)
                    return _targetDisplayName;

                return GetDisplayName(TargetValue);
            }
            set
            {
                _targetDisplayName = value;
            }
        }

        /// <summary>
        /// Copy the original source value into the target object
        /// </summary>
        public void AcceptSource()
        {
            Property.SetValue(Target, originalSourceValue, null);
        }

        /// <summary>
        /// Copy the original target value into the target object
        /// </summary>
        public void AcceptTarget()
        {
            Property.SetValue(Target, originalTargetValue, null);
        }

        public override string ToString()
        {
            if (IsChanged)
                return string.Format("{0}: {1} -> {2}", DisplayName, SourceValueDisplay, TargetValueDisplay);
            
            return string.Format("{0}: Unchanged", DisplayName);
        }
    }
}