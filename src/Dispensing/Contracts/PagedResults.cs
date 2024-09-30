using System;
using System.Collections.Generic;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Generic class used when returning result sets where pagination is used.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    [Serializable]
    public class PagedResults<TResult> : IEnumerable<TResult>
        where TResult : class
    {
        public static PagedResults<TResult> EmptyResults = new PagedResults<TResult>(new TResult[0], 0);

        #region Constructors

        protected PagedResults()
        {

        }

        public PagedResults(IEnumerable<TResult> results, long totalCount)
        {
            Results = results;
            TotalCount = totalCount;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets list of results returned by the search operation.
        /// </summary>
        /// <value>Array of <see cref="TResult"/>.</value>
        public IEnumerable<TResult> Results { get; private set; }

        /// <summary>
        /// Gets or sets the total count that resulted from the search operation.
        /// </summary>
        /// <value>The total user count.</value>
        public long TotalCount { get; private set; }

        #endregion

        #region IEnumerable<TResult> Members

        public IEnumerator<TResult> GetEnumerator()
        {
            return (Results != null) ? Results.GetEnumerator() : EmptyResults.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
