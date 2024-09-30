using System;
using System.Collections.Generic;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    public class DiagnosisSet : EntitySet<Guid, Diagnosis>
    {
        #region Constructors

        public DiagnosisSet()
        {
        }

        public DiagnosisSet(Guid key)
            : base(key)
        {
            
        }

        public DiagnosisSet(Guid key, IEnumerable<Diagnosis> items)
            : base(key, items)
        {
            
        }

        #endregion

        #region Operator Overloads

        public static implicit operator DiagnosisSet(Guid key)
        {
            return FromKey(key);
        }

        public static DiagnosisSet FromKey(Guid key)
        {
            return new DiagnosisSet(key);
        }

        #endregion
    }
}
