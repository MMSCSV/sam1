using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class UnitOfMeasureRoleEntity : IContractConvertible<UOMRole>
    {
        #region Implementation of IContractConvertible<UnitOfMeasureRole>

        public UOMRole ToContract()
        {
            return new UOMRole(InternalCode, DescriptionText)
                {
                    SortValue = SortValue.GetValueOrDefault()
                };
        }

        #endregion
    }
}
