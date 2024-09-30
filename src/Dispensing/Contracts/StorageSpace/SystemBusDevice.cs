using System;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Contracts
{
    public class SystemBusDevice : Entity<Guid>
    {
        public enum DrawerHeights : short { AccessibleDrawer = 1, FullHeight = 2, BinHeight = 4 };

        #region Constructors ...

        public SystemBusDevice()
        {
        }

        public SystemBusDevice(Guid key)
        {
            Key = key;
        }

        public SystemBusDevice(Guid key, byte[] lastModified)
        {
            Key = key;
            LastModified = lastModified;
        }

        #endregion

        #region Operator Overloads

        public static implicit operator SystemBusDevice(Guid key)
        {
            return FromKey(key);
        }

        public static SystemBusDevice FromKey(Guid key)
        {
            return new SystemBusDevice(key);
        }

        #endregion

        #region Public properties ...

        public Guid? ParentSystemBusKey { get; set; }

        public SystemBusDeviceTypeInternalCode SystemBusDeviceType { get; set; }

        public CommunicationCubeTypeInternalCode? CommunicationCubeType { get; set; }

        public long? SerialNumber { get; set; }

        public int? Position { get; set; }

        public short? Width { get; set; }

        public short? Height { get; set; }

        public short? Depth { get; set; }

        public short? Offset { get; set; }

        public Guid? DispensingDeviceKey { get; set; }

        #endregion
    }
}
