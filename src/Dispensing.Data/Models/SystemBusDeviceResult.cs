using Pyxis.Core.Data.Schema.Strg.Models;
using System;

namespace CareFusion.Dispensing.Data.Models
{
    public class SystemBusDeviceResult : SystemBusDevice
    {
        public Guid DispensingDeviceKey { get; set; }
    }
}
