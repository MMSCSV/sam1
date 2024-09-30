using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CareFusion.Dispensing
{
    public static class ConcurrentBagExtensions
    {
        public static void EmptyBag<T>(this ConcurrentBag<T> source)
        {
            if (source == null)
                return;

            T take;
            while (source.TryTake(out take))
            { }
        }

        public static T[] ToArrayAndEmptyBag<T>(this ConcurrentBag<T> source)
        {
            if (source == null)
                return null;

            return ToListAndEmptyBag(source).ToArray();
        }

        public static List<T> ToListAndEmptyBag<T>(this ConcurrentBag<T> source)
        {
            if (source == null)
                return null;

            List<T> items = new List<T>();

            T take;
            while (source.TryTake(out take))
            {
                items.Add(take);
            }

            return items;
        }
    }
}
