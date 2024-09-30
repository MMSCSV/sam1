using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class DispensingDeviceTypeEntity: IContractConvertible<DispensingDeviceType>
    {
        public DispensingDeviceType ToContract()
        {
            return new DispensingDeviceType(InternalCode, DescriptionText);
        }
    }
}
