using System;
using System.Collections.Generic;

namespace CareFusion.Dispensing.Comparers
{
    public class GenericEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        private readonly Func<T, T, bool> _comparisonFunction;
        private readonly Func<T, int> _hashCodeFunction;

        public GenericEqualityComparer(Func<T, T, bool> comparisonFunction,
            Func<T, int> hashCodeFunction = null)
        {
            Guard.ArgumentNotNull(comparisonFunction, "comparisonFunction");

            _comparisonFunction = comparisonFunction;
            _hashCodeFunction = hashCodeFunction;
        }

        public GenericEqualityComparer(Func<T, object> projection)
        {
            Guard.ArgumentNotNull(projection, "projection");

            _comparisonFunction = (t1, t2) => projection(t1).Equals(projection(t2));
            _hashCodeFunction = t => projection(t).GetHashCode();

        }

        public bool Equals(T x, T y)
        {
            return _comparisonFunction(x, y);
        }

        public int GetHashCode(T obj)
        {
            if (obj == null)
                return 0;

            return _hashCodeFunction != null
                ? _hashCodeFunction(obj)
                : obj.GetHashCode();
        }
    }
}
