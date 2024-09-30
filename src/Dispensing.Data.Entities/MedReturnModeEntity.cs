using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class MedReturnModeEntity : IContractConvertible<MedReturnMode>
    {
        #region IContractConvertible<MedReturnMode> Members

        public MedReturnMode ToContract()
        {
            return new MedReturnMode(InternalCode, DescriptionText)
                {
                    SortValue = SortValue
                };
        }

        #endregion
    }
}
