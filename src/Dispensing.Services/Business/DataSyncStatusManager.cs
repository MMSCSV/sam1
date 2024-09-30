using System;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data;
using CareFusion.Dispensing.Data.Entities;

namespace CareFusion.Dispensing.Services.Business
{
    internal class DataSyncStatusManager : IDataSyncStatusManager
    {
        public DateTimePair GetLastServerCommunicationTime(Guid dispensingDeviceKey)
        {
            using (ISyncRepository repository = RepositoryFactory.Create<ISyncRepository>())
            {
                return repository.GetLastServerCommunicationTime(dispensingDeviceKey);
            }
        }

        public PagedResults<SyncTransfer> GetSyncTransfers(Guid dispensingDeviceKey, int startIndex, int maxResults)
        {
            using (ISyncRepository repository = RepositoryFactory.Create<ISyncRepository>())
            {
                IQueryable<SyncTransferEntity> query = repository.GetQueryableEntity<SyncTransferEntity>();
                query =
                    query
                    .Where(st => st.DispensingDeviceKey == dispensingDeviceKey)
                    .OrderByDescending(st => st.TransferStartUTCDateTime);

                long totalCount = query.LongCount();
                SyncTransfer[] transfers = query.Skip(startIndex)
                    .Take(maxResults)
                    .Select(ua => ua.ToContract())
                    .ToArray();

                return new PagedResults<SyncTransfer>(transfers, totalCount);
            }
        }

        public bool DataUpgradeRequired()
        {
            using (ISyncRepository repository = RepositoryFactory.Create<ISyncRepository>())
            {
                return repository.DataUpgradeRequired();
            }
        }
    }
}
