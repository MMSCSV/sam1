using System;
using System.Collections.Generic;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    public class PharmacyOrderRouteSet: EntitySet<Guid, PharmacyOrderRoute>
    {
        #region Constructors

        public PharmacyOrderRouteSet()
        {
        }

        public PharmacyOrderRouteSet(Guid key)
            : base(key)
        {
            
        }

        public PharmacyOrderRouteSet(Guid key, IEnumerable<PharmacyOrderRoute> items)
            : base(key, items)
        {
            
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderRouteSet(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderRouteSet FromKey(Guid key)
        {
            return new PharmacyOrderRouteSet(key);
        }

        #endregion
    }
}
