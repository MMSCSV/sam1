using System;
using System.Collections.Generic;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    public class PharmacyOrderSupplierDispensingInstructionSet : EntitySet<Guid, PharmacyOrderSupplierDispensingInstruction>
    {
        #region Constructors

        public PharmacyOrderSupplierDispensingInstructionSet()
        {
        }

        public PharmacyOrderSupplierDispensingInstructionSet(Guid key)
            : base(key)
        {
            
        }

        public PharmacyOrderSupplierDispensingInstructionSet(Guid key, IEnumerable<PharmacyOrderSupplierDispensingInstruction> items)
            : base(key, items)
        {
            
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderSupplierDispensingInstructionSet(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderSupplierDispensingInstructionSet FromKey(Guid key)
        {
            return new PharmacyOrderSupplierDispensingInstructionSet(key);
        }

        #endregion
    }
}
