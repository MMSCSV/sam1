using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CareFusion.Dispensing
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this Collection<T> collection, IEnumerable<T> values)
        {
            foreach (var item in values)
            {
                collection.Add(item);
            }
        }

        public static T Find<T>(this Collection<T> collection, Predicate<T> predicate)
        {
            foreach (var item in collection)
            {
                if (predicate(item))
                    return item;
            }
            return default(T);
        }

        public static Collection<T> FindAll<T>(this Collection<T> collection, Predicate<T> predicate)
        {
            Collection<T> all = new Collection<T>();
            foreach (var item in collection)
            {
                if (predicate(item))
                    all.Add(item);
            }
            return all;
        }

        public static int FindIndex<T>(this Collection<T> collection, Predicate<T> predicate)
        {
            return FindIndex(collection, 0, predicate);
        }

        public static int FindIndex<T>(this Collection<T> collection, int startIndex, Predicate<T> predicate)
        {
            return FindIndex(collection, startIndex, collection.Count, predicate);
        }

        public static int FindIndex<T>(this Collection<T> collection, int startIndex, int count, Predicate<T> predicate)
        {
            for (int i = startIndex; i < count; i++)
            {
                if (predicate(collection[i]))
                    return i;
            }
            return -1;
        }

        public static T FindLast<T>(this Collection<T> collection, Predicate<T> predicate)
        {
            for (int i = collection.Count - 1; i >= 0; i--)
            {
                if (predicate(collection[i]))
                    return collection[i];
            }
            return default(T);
        }

        public static int FindLastIndex<T>(this Collection<T> collection, Predicate<T> predicate)
        {
            return FindLastIndex(collection, collection.Count - 1, predicate);
        }

        public static int FindLastIndex<T>(this Collection<T> collection, int startIndex, Predicate<T> predicate)
        {
            return FindLastIndex(collection, startIndex, startIndex + 1, predicate);

        }

        public static int FindLastIndex<T>(this Collection<T> collection, int startIndex, int count, Predicate<T> predicate)
        {
            for (int i = startIndex; i >= startIndex - count; i--)
            {
                if (predicate(collection[i]))
                    return i;
            }
            return -1;
        }

        public static void ForEach<T>(this Collection<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static int RemoveAll<T>(this Collection<T> collection, Predicate<T> match)
        {
            int count = 0;
            for (int i = 0; i < collection.Count; i++)
            {
                if (match(collection[i]))
                {
                    collection.Remove(collection[i]);
                    count++;
                    i--;
                }
            }
            return count;
        }

        public static bool TrueForAll<T>(this Collection<T> collection, Predicate<T> match)
        {
            foreach (var item in collection)
            {
                if (!match(item))
                    return false;
            }
            return true;
        }
    }
}
