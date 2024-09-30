using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents a pharmacy order.
    /// </summary>
    [Serializable]
    public class PharmacyOrderAdminInstruction : Entity<Guid>
    {
        #region Constructors

        public PharmacyOrderAdminInstruction()
        {
        }

        public PharmacyOrderAdminInstruction(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator PharmacyOrderAdminInstruction(Guid key)
        {
            return FromKey(key);
        }

        public static PharmacyOrderAdminInstruction FromKey(Guid key)
        {
            return new PharmacyOrderAdminInstruction(key);
        }

        #endregion

        #region Public Properties

        public int MemberNumber { get; set; }

        public string Description { get; set; }

        public bool Truncated { get; set; }

        #endregion
    }
}
