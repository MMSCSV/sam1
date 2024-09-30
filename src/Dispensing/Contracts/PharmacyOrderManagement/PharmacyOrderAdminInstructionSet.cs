using System;
using System.Collections.Generic;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    public class PharmacyOrderAdminInstructionSet: EntitySet<Guid, PharmacyOrderAdminInstruction>
    {
        #region Constructors

        public PharmacyOrderAdminInstructionSet()
        {
        }

        public PharmacyOrderAdminInstructionSet(Guid key)
            : base(key)
        {
            
        }

        public PharmacyOrderAdminInstructionSet(Guid key, IEnumerable<PharmacyOrderAdminInstruction> items)
            : base(key, items)
        {
            
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderAdminInstructionSet(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderAdminInstructionSet FromKey(Guid key)
        {
            return new PharmacyOrderAdminInstructionSet(key);
        }

        #endregion
    }
}
