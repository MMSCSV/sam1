using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class MiniDrawerTrayModeEntity : IContractConvertible<MiniDrawerTrayMode>
    {
        #region IContractConvertible<MiniDrawerTrayMode> Members

        public MiniDrawerTrayMode ToContract()
        {
            return new MiniDrawerTrayMode(InternalCode, DescriptionText)
            {
               SortValue = SortValue
            };
        }

        #endregion
    }
}
