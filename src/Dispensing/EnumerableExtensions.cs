using System;
using System.Collections.Generic;
using System.Linq;

namespace CareFusion.Dispensing
{
    public static class EnumerableExtensions
    {
        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source)
        {
            Guard.ArgumentNotNull(source, "source");

            return new HashSet<TSource>(source);
        }

        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            Guard.ArgumentNotNull(source, "source");

            return new HashSet<TSource>(source, comparer);
        }

        public static MultiKeyDictionary<TKey, TSubKey, TSource> ToMultiKeyDictionary<TSource, TKey, TSubKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TSubKey> subKeySelector)
        {
            Guard.ArgumentNotNull(source, "source");
            Guard.ArgumentNotNull(keySelector, "keySelector");
            Guard.ArgumentNotNull(subKeySelector, "subKeySelector");

            MultiKeyDictionary<TKey, TSubKey, TSource> multiKeyDictionary = new MultiKeyDictionary<TKey, TSubKey, TSource>();
            foreach (TSource element in source) multiKeyDictionary.Add(keySelector(element), subKeySelector(element), element);
            return multiKeyDictionary;
        }

        public static bool IsOrdered<T>(this IEnumerable<T> source)
        {
            Guard.ArgumentNotNull(source, "source");

            var comparer = Comparer<T>.Default;
            using(var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    return true;

                var previous = enumerator.Current;
                while(enumerator.MoveNext())
                {
                    if (comparer.Compare(previous, enumerator.Current) > 0)
                        return false;

                    previous = enumerator.Current;
                }

                return true;
            }
        }

        public static IEnumerable<T> PageByIndex<T>(this IEnumerable<T> source, int startIndex, int maxResults, out long totalCount)
        {
            Guard.ArgumentNotNull(source, "source");

            // A maxResults of less than 0 is equal to int.MaxValue
            if (maxResults < 0 || maxResults == int.MaxValue)
            {
                // If the starting index is greater than 0 then make sure that
                // the MaxValue is substracted by the index to avoid arithmetic 
                // issues.
                maxResults = startIndex == 0 ? int.MaxValue : int.MaxValue - startIndex;
            }

            totalCount = source.LongCount();
            return source.Skip(startIndex).Take(maxResults);
        }

        public static IEnumerable<T> Page<T>(this IEnumerable<T> source, int page, int pageSize, out long totalCount)
        {
            Guard.ArgumentNotNull(source, "source");

            // Calculate the start index.
            int startIndex = (page - 1) * pageSize;

            return PageByIndex(source, startIndex, pageSize, out totalCount);
        }

        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            Guard.ArgumentNotNull(source, "source");

            for (int i = 0; i < source.Count(); i += chunkSize)
                yield return source.Skip(i).Take(chunkSize);
        }
    }
}
