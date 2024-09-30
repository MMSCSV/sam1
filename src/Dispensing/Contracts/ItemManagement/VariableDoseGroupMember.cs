using System;

namespace CareFusion.Dispensing.Contracts
{
    /// <summary>
    /// Represents the member of a varialbe dose group.
    /// </summary>
    [Serializable]
    public class VariableDoseGroupMember : Entity<Guid>
    {
         #region Constructors

        public VariableDoseGroupMember()
        {
        }

        public VariableDoseGroupMember(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator VariableDoseGroupMember(Guid key)
        {
            return FromKey(key);
        }

        public static VariableDoseGroupMember FromKey(Guid key)
        {
            return new VariableDoseGroupMember(key);
        }

        #endregion

        #region Public Properties

        public Guid MedItemKey { get; set; }

        public string ItemId { get; set; }

        public string DisplayName { get; set; }

        public string PureGenericName { get; set; }

        public string BrandName { get; set; }

        public Guid? DosageFormKey { get; set; }

        public string DosageFormCode { get; set; }

        public Guid? EquivalencyDosageFormGroupKey { get; set; }

        public string EquivalencyDosageFormGroupDisplayCode { get; set; }

        public int? Rank { get; set; }

        #endregion
    }
}
