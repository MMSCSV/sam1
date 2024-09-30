using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CareFusion.Dispensing.Data
{
    public static class QueryableExtensions
    {
        public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, bool ascending)
        {
            if (!ascending)
                return source.OrderByDescending(keySelector);

            return source.OrderBy(keySelector);
        }

        public static IOrderedQueryable<TSource> ThenBy<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, bool ascending)
        {
            if (!ascending)
                return source.ThenByDescending(keySelector);

            return source.ThenBy(keySelector);
        }

        public static IQueryable<T> PageByIndex<T>(this IQueryable<T> source, int startIndex, int maxResults)
        {
            // A maxResults of less than 0 is equal to int.MaxValue
            if (maxResults < 0 || maxResults == int.MaxValue)
            {
                // If the starting index is greater than 0 then make sure that
                // the MaxValue is substracted by the index to avoid arithmetic 
                // issues.
                maxResults = startIndex == 0 ? int.MaxValue : int.MaxValue - startIndex;
            }

            return source.Skip(startIndex).Take(maxResults);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> source, int page, int pageSize)
        {
            // Calculate the start index.
            int startIndex = (page - 1) * pageSize;

            return PageByIndex(source, startIndex, pageSize);
        }

        public static IEnumerable<TResult> PageByIndex<T, TResult>(this IQueryable<T> source, int startIndex, int maxResults, Expression<Func<T, TResult>> selector, out long totalCount)
        {
            if (maxResults < 0)
            {
                maxResults = int.MaxValue;
            }

            // If the caller is not asking for all results then we need
            // 2 database calls, one for count and the other for the results.
            if (maxResults != int.MaxValue)
            {
                totalCount = source.LongCount();
                return source.PageByIndex(startIndex, maxResults)
                    .Select(selector)
                    .ToArray();
            }

            // Optimization: At this point the caller is asking for all results,
            // therefore we just need one database call for the results.
            IEnumerable<TResult> results = source.PageByIndex(startIndex, maxResults)
                .Select(selector)
                .ToArray();

            // Get the total count from the in-memory results.
            totalCount = results.Count();

            return results;
        }

        public static IEnumerable<TResult> Page<T, TResult>(this IQueryable<T> source, int page, int pageSize, Expression<Func<T, TResult>> selector, out long totalCount)
        {
            // Calculate the start index.
            int startIndex = (page - 1)*pageSize;

            return PageByIndex(source, startIndex, pageSize, selector, out totalCount);
        }
    }
}
