using System;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data
{
    public interface ISyncRepository : IRepository
    {
        DateTimePair GetLastServerCommunicationTime(Guid dispensingDeviceKey);
        bool DataUpgradeRequired();
    }
}
