using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class ReplenishmentScanModeEntity : IContractConvertible<ReplenishmentScanMode>
    {
        public ReplenishmentScanMode ToContract()
        {
            return new ReplenishmentScanMode(InternalCode, DescriptionText)
            {
                SortValue = SortValue
            };
        }
    }
}
