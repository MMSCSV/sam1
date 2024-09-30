using System;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Services.Business
{
    public interface IDispensingDeviceManager
    {
        DispensingDevice Get(string computerName);

        DispensingDevice Get(Guid dispensingDeviceKey);

        Guid Add(Context context, DispensingDevice dispensingDevice);

        void Update(Context context, DispensingDevice dispensingDevice);

        string GetSerialId(Guid dispensingDeviceKey);
    }
}
