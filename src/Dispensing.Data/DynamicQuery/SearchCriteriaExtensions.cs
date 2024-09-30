using System;
using System.Collections.Generic;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.Schema.Core.Models;

namespace CareFusion.Dispensing.Data.DynamicQuery
{
    public static class SearchCriteriaExtensions
    {
        public static IQueryable<TEntity> ApplySearchCriteria<TEntity, TSearchFields>(this SearchCriteria source, IQueryable<TEntity> query, 
            Func<IQueryable<TEntity>, SearchCondition<TSearchFields>, IQueryable<TEntity>> func)
        {
            Guard.ArgumentNotNull(source, "source");

            var composite = source as SearchComposite;
            var condition = source as SearchCondition<TSearchFields>;
            if (composite != null)
            {
                if (composite.Items != null && composite.Items.Length > 0)
                {
                    if (composite.Items.Any(x => x is SearchComposite))
                    {
                        throw new NotSupportedException("Nested composites are not supported.");
                    }

                    foreach(SearchCriteria searchCriteria in composite.Items)
                    {
                        query = func(query, (SearchCondition<TSearchFields>) searchCriteria);
                    }
                }
            }
            else if (condition != null)
            {
                query = func(query, condition);
            }

            return query;
        }

        public static IEnumerable<FindFilter> CreateFindFilters<TSearchFields>(this SearchCriteria source)
        {
            Guard.ArgumentNotNull(source, "source");

            List<FindFilter> filters = new List<FindFilter>();
            var composite = source as SearchComposite;
            var condition = source as SearchCondition<TSearchFields>;

            if (composite != null)
            {
                if (composite.Items != null && composite.Items.Length > 0)
                {
                    if (composite.Items.Any(x => x is SearchComposite))
                    {
                        throw new NotSupportedException("Nested composites are not supported.");
                    }

                    foreach (SearchCriteria searchCriteria in composite.Items)
                    {
                        SearchCondition<TSearchFields> searchCondition = (SearchCondition<TSearchFields>) searchCriteria;

                        filters.Add(new FindFilter(searchCondition.Field.ToString(), searchCondition.Operator.ToString(),
                                                   ConvertFilterValueToString(searchCondition.Value)));
                    }
                }
            }
            else if (condition != null)
            {
                filters.Add(new FindFilter(condition.Field.ToString(), condition.Operator.ToString(),
                                                   ConvertFilterValueToString(condition.Value)));
            }

            return filters;
        }

        private static string ConvertFilterValueToString(object filterValue)
        {
            if (filterValue == null)
                return null;

            if (filterValue is DateTime)
            {
                // Format date/time to the supported format (international) as per article:
                // http://support.microsoft.com/kb/173907?wa=wsignin1.0
                return ((DateTime)filterValue).ToString("yyyy-MM-dd hh:mm:ss");
            }

            return filterValue.ToString();
        }
    }
}
