using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class VerifyCountModeEntity : IContractConvertible<VerifyCountMode>
    {
        #region IContractConvertible<VerifyCountMode> Members

        public VerifyCountMode ToContract()
        {
            return new VerifyCountMode(InternalCode, DescriptionText)
                {
                    SortValue = SortValue
                };
        }

        #endregion
    }
}
