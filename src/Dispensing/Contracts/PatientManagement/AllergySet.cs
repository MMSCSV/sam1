using System;
using System.Collections.Generic;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a collection of patient allergies for a given patient.
    /// </summary>
    [Serializable]
    public class AllergySet : EntitySet<Guid, Allergy>
    {
        #region Constructors

        public AllergySet()
        {
        }

        public AllergySet(Guid key)
            : base(key)
        {
            
        }

        public AllergySet(Guid key, IEnumerable<Allergy> items)
            : base(key, items)
        {
            
        }

        public AllergySet(IEnumerable<Allergy> items)
            : base(items)
        { }

        #endregion

        #region Operator Overloads

        public static implicit operator AllergySet(Guid key)
        {
            return FromKey(key);
        }

        public static AllergySet FromKey(Guid key)
        {
            return new AllergySet(key);
        }

        #endregion
    }
}
