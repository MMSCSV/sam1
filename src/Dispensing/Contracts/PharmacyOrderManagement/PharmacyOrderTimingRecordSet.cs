using System;
using System.Collections.Generic;

namespace CareFusion.Dispensing.Contracts
{
    public class PharmacyOrderTimingRecordSet: EntitySet<Guid, PharmacyOrderTimingRecord>
    {
        #region Constructors

        public PharmacyOrderTimingRecordSet()
        {
        }

        public PharmacyOrderTimingRecordSet(Guid key)
            : base(key)
        {
            
        }

        public PharmacyOrderTimingRecordSet(Guid key, IEnumerable<PharmacyOrderTimingRecord> items)
            : base(key, items)
        {
            
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderTimingRecordSet(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderTimingRecordSet FromKey(Guid key)
        {
            return new PharmacyOrderTimingRecordSet(key);
        }

        #endregion
    }
}
