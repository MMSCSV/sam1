using System;

namespace CareFusion.Dispensing.Contracts
{
    [Serializable]
    public class ComboComponent : Entity<Guid>
    {
        #region Constructors

        public ComboComponent()
        {

        }

        public ComboComponent(Guid key)
        {
            Key = key;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator ComboComponent(Guid key)
        {
            return FromKey(key);
        }

        public static ComboComponent FromKey(Guid key)
        {
            return new ComboComponent(key);
        }

        #endregion
        
        #region Public Properties

        public Guid FacilityItemKey { get; set; }

        public string DisplayName { get; set; }

        public short? Quantity { get; set; }

        public bool Charge { get; set; }

        public bool Multiplier { get; set; }

        public Guid[] AdministrationRoutes { get; set; }

        #endregion
    }
}
