using System;
using System.Collections.Generic;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    public class PatientIdentificationSet : EntitySet<Guid, PatientIdentification>
    {
        #region Constructors

        public PatientIdentificationSet()
        { }

        public PatientIdentificationSet(Guid key)
            : base(key)
        { }

        public PatientIdentificationSet(Guid key, IEnumerable<PatientIdentification> items)
            : base(key, items)
        { }

        public PatientIdentificationSet(IEnumerable<PatientIdentification> items)
            : base(items)
        { }

        #endregion

        #region Operator Overloads

        public static implicit operator PatientIdentificationSet(Guid key)
        {
            return FromKey(key);
        }

        public static PatientIdentificationSet FromKey(Guid key)
        {
            return new PatientIdentificationSet(key);
        }

        #endregion
    }
}
