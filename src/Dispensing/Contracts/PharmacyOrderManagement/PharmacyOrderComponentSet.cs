using System;
using System.Collections.Generic;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    public class PharmacyOrderComponentSet: EntitySet<Guid, PharmacyOrderComponent>
    {
        #region Constructors

        public PharmacyOrderComponentSet()
        {
        }

        public PharmacyOrderComponentSet(Guid key)
            : base(key)
        {
            
        }

        public PharmacyOrderComponentSet(Guid key, IEnumerable<PharmacyOrderComponent> items)
            : base(key, items)
        {
            
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderComponentSet(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderComponentSet FromKey(Guid key)
        {
            return new PharmacyOrderComponentSet(key);
        }

        #endregion
    }
}
