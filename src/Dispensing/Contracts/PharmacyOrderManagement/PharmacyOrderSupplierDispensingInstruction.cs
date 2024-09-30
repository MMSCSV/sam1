using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a pharmacy order.
    /// </summary>
    [Serializable]
    public class PharmacyOrderSupplierDispensingInstruction : Entity<Guid>
    {
        #region Constructors

        public PharmacyOrderSupplierDispensingInstruction()
        {
        }

        public PharmacyOrderSupplierDispensingInstruction(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderSupplierDispensingInstruction(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderSupplierDispensingInstruction FromKey(Guid key)
        {
            return new PharmacyOrderSupplierDispensingInstruction(key);
        }

        #endregion

        #region Public Properties

        public int MemberNumber { get; set; }

        public string Description { get; set; }

        public bool Truncated { get; set; }

        #endregion
    }
}
