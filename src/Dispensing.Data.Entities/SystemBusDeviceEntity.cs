using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data.Entities
{
    public partial class SystemBusDeviceEntity : IContractConvertible<SystemBusDevice>
    {
        #region IContractConvertible<SystemBusDevice> Members

        public SystemBusDevice ToContract()
        {
            return new SystemBusDevice(Key, LastModifiedBinaryValue.ToArray())
            {
                SystemBusDeviceType = SystemBusDeviceTypeInternalCode.FromInternalCode<SystemBusDeviceTypeInternalCode>(),
                CommunicationCubeType = CommunicationCubeTypeInternalCode.FromNullableInternalCode<CommunicationCubeTypeInternalCode>(),
                SerialNumber = DeviceNumber,
                Position = PositionNumber,
                ParentSystemBusKey = ControllingSystemBusDeviceKey,
                Width = WidthQuantity,
                Height = HeightQuantity,
                Depth = DepthQuantity,
                Offset = OffsetQuantity,
                LastModified = LastModifiedBinaryValue.ToArray(),
                DispensingDeviceKey = DispensingDeviceKey
            };
        }

        #endregion

       
    }
}
