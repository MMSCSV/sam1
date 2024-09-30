using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class AutoMedLabelModeEntity : IContractConvertible<AutoMedLabelMode>
    {
        #region IContractConvertible<MedReturnMode> Members

        public AutoMedLabelMode ToContract()
        {
            return new AutoMedLabelMode(InternalCode, DescriptionText)
            {
                SortValue = SortValue
            };
        }

        #endregion
    }
}
