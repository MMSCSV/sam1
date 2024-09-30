using System.Collections.Generic;
using System.Linq;

namespace CareFusion.Dispensing.Comparers
{
    /// <summary>
    /// Compares two objects for differences in their public properties
    /// </summary>
    /// <typeparam name="TClass">The type of objects to be compared</typeparam>
    public class ClassComparer<TClass> where TClass : class
    {
        /// <summary>
        /// Create a comparer for two objects of the same type
        /// </summary>
        /// <param name="source">The source object</param>
        /// <param name="target">The target object</param>
        public ClassComparer(TClass source, TClass target)
        {
            this.source = source;
            this.target = target;

            // Don't use lazy-eval on this. Properties won't change throughout the class' lifetime
            Properties = typeof (TClass).GetProperties()
                .Select(prop => new PropertyComparer<TClass>(prop, source, target))
                .ToList();
        }

        private readonly TClass source;
        private readonly TClass target;

        /// <summary>
        /// Retrieves the PropertyComparers for all public properties
        /// </summary>
        public IEnumerable<PropertyComparer<TClass>> Properties { get; private set;}
        
        /// <summary>
        /// Retrieves only the PropertyComparers for changed properties
        /// </summary>
        public IEnumerable<PropertyComparer<TClass>> Changes
        {
            get
            {
                return Properties.Where(property => property.IsChanged);
            }
        }

        /// <summary>
        /// Returns whether or not the source and target items are different
        /// </summary>
        public bool HasChanges
        {
            get
            {
                return Properties.Any(property => property.IsChanged);
            }
        }

        /// <summary>
        /// Copy the original source values of all the properties into the target properties
        /// </summary>
        public void AcceptSource()
        {
            foreach(var change in Changes)
            {
                change.AcceptSource();
            }
        }

        /// <summary>
        /// Copy the original values of all the target properties back into the target properties
        /// </summary>
        public void AcceptTarget()
        {
            foreach(var property in Properties)
            {
                property.AcceptTarget();
            }
        }
    }
}