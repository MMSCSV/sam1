using System;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Services.Business
{
    internal interface IDataSyncStatusManager
    {
        DateTimePair GetLastServerCommunicationTime(Guid dispensingDeviceKey);

        PagedResults<SyncTransfer> GetSyncTransfers(Guid dispensingDeviceKey, int startIndex, int maxResults);

        bool DataUpgradeRequired();
    }
}
