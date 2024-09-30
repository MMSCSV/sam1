using System;
using System.ServiceModel;

namespace CareFusion.Dispensing.Contracts
{
    [ServiceContract(Namespace = ContractConstants.ContractsNamespaceV1)]
    public interface IDataSyncStatusService
    {
        [OperationContract]
        DateTimePair GetLastServerCommunicationTime(Guid dispensingDeviceKey);

        [OperationContract]
        PagedResults<SyncTransfer> GetSyncTransfers(Guid dispensingDeviceKey, int startIndex, int maxResults);

        [OperationContract]
        bool DataUpgradeRequired();
    }
}
