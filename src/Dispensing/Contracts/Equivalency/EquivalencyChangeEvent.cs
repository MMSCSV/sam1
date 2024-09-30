using System;

namespace CareFusion.Dispensing.Contracts
{
    public class EquivalencyChangeEvent : Entity<Guid>
    {
        public EquivalencyChangeEvent(Guid key)
        {
            Key = key;
        }

        public MedItem MedItem { get; set; }
    }
}
