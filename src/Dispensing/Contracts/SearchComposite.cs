using System;
using System.Linq;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    public class SearchComposite : SearchCriteria
    {
        #region Constructors

        public SearchComposite()
        {
            Items = null;
        }

        public SearchComposite(params SearchCriteria[] criteria)
        {
            Guard.ArgumentNotNull(criteria, "criteria");

            Items = criteria;
        }

        #endregion

        #region Public Properties

        public SearchCriteria[] Items { get; set; }

        #endregion

        public override string ToString()
        {
            return Items.Select(sc => sc.ToString())
                .Aggregate((sc1, sc2) => string.Format("{0} And {1}", sc1, sc2));
        }
    }
}
