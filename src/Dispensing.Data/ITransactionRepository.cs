using System;
using CareFusion.Dispensing.Contracts;

namespace CareFusion.Dispensing.Data
{
    public interface ITransactionRepository : IRepository
    {        
        #region Storage Space Inventory
        /// <summary>
        /// Get the current storageSpaceInventory record for a given StorageSpaceItem record.
        /// </summary>
        /// <param name="storageSpaceItemKey"></param>
        /// <returns></returns>
        StorageSpaceInventory GetStorageSpaceInventory(Guid storageSpaceItemKey);

        /// <summary>
        /// Insert storageSpaceInventory record.
        /// </summary>
        Guid InsertStorageSpaceInventory(Context context, StorageSpaceInventory storageSpaceInventory);

        /// <summary>
        /// Insert storageSpaceInventory record.
        /// </summary>
        Guid InsertStorageSpaceInventory(
            DateTime actionUtcDateTime,
            DateTime actionDateTime,
            Guid? deviceKey,
            Guid? userKey,
            StorageSpaceInventory storageSpaceInventory);

        /// <summary>
        /// Update a storage space inventory item.
        /// </summary>
        Guid UpdateStorageSpaceInventory(Context context, StorageSpaceInventory storageSpaceInventory);

        /// <summary>
        /// Update a storage space inventory item.
        /// </summary>
        Guid UpdateStorageSpaceInventory(
            DateTime actionDateTime,
            DateTime actionUtcDateTime,
            Guid? deviceKey,
            Guid? actorKey,
            StorageSpaceInventory storageSpaceInventory);
        #endregion       

        #region Discrepancy
        /// <summary>
        /// Inserts a new discrepancy
        /// </summary>
        void InsertDiscrepancy(Context context, Guid itemTransactionKey, bool autoResolve);

        /// <summary>
        /// Inserts a new discrepancy
        /// </summary>
        void InsertDiscrepancy(
            DateTime actionUtcDateTime,
            DateTime actionDateTime,
            Guid? deviceKey,
            Guid? userKey,
            Guid itemTransactionKey,
            bool autoResolve);

        /// <summary>
        /// Updates a discrepancy
        /// </summary>
        void UpdateDiscrepancy(
            Context context,
            Guid itemTransactionKey,
            Guid? priorItemTransactionKey,
            Guid? witnessUserAccountKey,
            Guid? resolutionKey,
            string resolutionText,
            Guid? transactionSessionKey,
            byte[] lastModified);

        /// <summary>
        /// Inserts a new discrepancy note
        /// </summary>
        void InsertDiscrepancyNote(Context context, Guid itemTransactionKey, string noteText, bool serverFlag = false);

        #endregion
    }
}
