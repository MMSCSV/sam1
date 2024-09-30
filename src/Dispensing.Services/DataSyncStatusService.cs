using System;
using System.ServiceModel;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Services.Business;

namespace CareFusion.Dispensing.Services
{
    [ServiceBehavior(ConfigurationName = "DispensingDataSyncStatusService",
        InstanceContextMode = InstanceContextMode.PerCall)]
    public class DataSyncStatusService : DispensingServiceBase, IDataSyncStatusService
    {
        private readonly IDataSyncStatusManager _dataSyncManager;

        public DataSyncStatusService()
             : this(new DataSyncStatusManager())
        {
        }

        internal DataSyncStatusService(IDataSyncStatusManager dataSyncManager)
        {
            Guard.ArgumentNotNull(dataSyncManager, "dataSyncManager");

            _dataSyncManager = dataSyncManager;
        }

        public DateTimePair GetLastServerCommunicationTime(Guid dispensingDeviceKey)
        {
            DateTimePair lastCommunication = null;

            try
            {
                lastCommunication = _dataSyncManager.GetLastServerCommunicationTime(dispensingDeviceKey);
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }

            return lastCommunication;
        }

        public PagedResults<SyncTransfer> GetSyncTransfers(Guid dispensingDeviceKey, int startIndex, int maxResults)
        {
            PagedResults<SyncTransfer> transfers = null;

            try
            {
                transfers = _dataSyncManager.GetSyncTransfers(dispensingDeviceKey, startIndex, maxResults);
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }

            return transfers;
        }

        public bool DataUpgradeRequired()
        {
            try
            {
                return _dataSyncManager.DataUpgradeRequired();
            }
            catch (Exception e)
            {
                if (ServiceExceptionHandler.HandleException(e))
                    throw;
            }
            return false;
        }
    }
}
