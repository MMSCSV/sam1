using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class EncounterTypeEntity : IContractConvertible<EncounterType>
    {
        #region IContractConvertible<EncounterType> Members

        public EncounterType ToContract()
        {
            return new EncounterType(InternalCode, DescriptionText)
                {
                    SortValue = SortValue
                };
        }

        #endregion
    }
}
