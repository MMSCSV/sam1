using System.Collections.Generic;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Comparers
{
    public class KeyEqualityComparer<T> : IEqualityComparer<T>
        where T : IEntity
    {
        #region Implementation of IEqualityComparer<in T>

        public bool Equals(T x, T y)
        {
            if (object.ReferenceEquals(x, y))
                return true;

            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return false;

            return x.Key.Equals(y.Key);
        }

        public int GetHashCode(T obj)
        {
            if (object.ReferenceEquals(obj, null))
                return 0;

            return obj.Key.GetHashCode();
        }

        #endregion
    }
}
