using System;
using System.Collections.Generic;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    public class PharmacyOrderOrderingPersonSet: EntitySet<Guid, PharmacyOrderOrderingPerson>
    {
        #region Constructors

        public PharmacyOrderOrderingPersonSet()
        {
        }

        public PharmacyOrderOrderingPersonSet(Guid key)
            : base(key)
        {
            
        }

        public PharmacyOrderOrderingPersonSet(Guid key, IEnumerable<PharmacyOrderOrderingPerson> items)
            : base(key, items)
        {
            
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderOrderingPersonSet(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderOrderingPersonSet FromKey(Guid key)
        {
            return new PharmacyOrderOrderingPersonSet(key);
        }

        #endregion
    }
}
