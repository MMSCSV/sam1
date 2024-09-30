using System;
using System.Collections.Generic;
using System.Linq;

namespace CareFusion.Dispensing.Data
{
    public static class EnumerableExtensions
    {
        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool ascending)
        {
            if (!ascending)
                return source.OrderByDescending(keySelector);

            return source.OrderBy(keySelector);
        }

        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, bool ascending)
        {
            if (!ascending)
                return source.ThenByDescending(keySelector);

            return source.ThenBy(keySelector);
        }
    
    }
}
