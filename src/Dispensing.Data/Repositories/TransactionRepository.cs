using System;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data;
using Pyxis.Core.Data.Schema;
using TxDAL = Pyxis.Core.Data.Schema.Tx;

namespace CareFusion.Dispensing.Data.Repositories
{
    internal class TransactionRepository : LinqBaseRepository, ITransactionRepository
    {
        private readonly TxDAL.IStorageSpaceInventoryRepository _storageSpaceInventoryRepository;
        private readonly TxDAL.IItemTransactionRepository _itemTransactionRepository;

        public TransactionRepository()
        {
            _storageSpaceInventoryRepository = new TxDAL.StorageSpaceInventoryRepository();
            _itemTransactionRepository = new TxDAL.ItemTransactionRepository();
        }

        #region Storage Space Inventory
        StorageSpaceInventory ITransactionRepository.GetStorageSpaceInventory(Guid storageSpaceItemKey)
        {
            StorageSpaceInventory storageSpaceInventory = null;

            try
            {
                var result = _storageSpaceInventoryRepository.GetStorageSpaceInventoryByStorageSpaceItem(storageSpaceItemKey);
                if (result != null)
                {
                    storageSpaceInventory = new StorageSpaceInventory
                    {
                        EarliestNextExpirationDate = result.EarliestNextExpirationDate,
                        InventoryQuantity = result.InventoryQuantity,
                        Key = result.StorageSpaceInventoryKey,
                        LastModified = result.LastModifiedBinaryValue,
                        StorageSpaceItemKey = result.StorageSpaceItemKey,
                        StrengthInventoryQuantity = result.StrengthInventoryQuantity,
                        StrengthUnitOfMeasureKey = result.StrengthUOMKey,
                        VolumeInventoryQuantity = result.VolumeInventoryQuantity,
                        VolumeUnitOfMeasureKey = result.VolumeUOMKey
                    };
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }

            return storageSpaceInventory;
        }

        public Guid InsertStorageSpaceInventory(Context context, StorageSpaceInventory storageSpaceInventory)
        {
            return InsertStorageSpaceInventory(context.ActionUtcDateTime, context.ActionDateTime, (Guid?)context.Device,
                (Guid?)context.Actor, storageSpaceInventory);
        }

        public Guid InsertStorageSpaceInventory(DateTime actionUtcDateTime, DateTime actionDateTime, Guid? deviceKey,
            Guid? userKey, StorageSpaceInventory storageSpaceInventory)
        {
            Guid? storageSpaceInventoryKey = storageSpaceInventory.IsTransient() ? default(Guid?) : storageSpaceInventory.Key;

            try
            {
                var actionContext = new ActionContext
                {
                    ActionDateTime = actionDateTime,
                    ActionLocalDateTime = actionDateTime,
                    ActionUtcDateTime = actionUtcDateTime,
                    ActorKey = userKey,
                    DispensingDeviceKey = deviceKey,
                    UserAccountKey = userKey
                };
                var storageSpaceInventoryModel = new TxDAL.Models.StorageSpaceInventory
                {
                    EarliestNextExpirationDate = storageSpaceInventory.EarliestNextExpirationDate,
                    InventoryQuantity = storageSpaceInventory.InventoryQuantity,
                    LastModifiedBinaryValue = storageSpaceInventory.LastModified,
                    StorageSpaceInventoryKey = storageSpaceInventory.Key,
                    StorageSpaceItemKey = storageSpaceInventory.StorageSpaceItemKey,
                    StrengthInventoryQuantity = storageSpaceInventory.StrengthInventoryQuantity,
                    StrengthUOMKey = storageSpaceInventory.StrengthUnitOfMeasureKey,
                    VolumeInventoryQuantity = storageSpaceInventory.VolumeInventoryQuantity,
                    VolumeUOMKey = storageSpaceInventory.VolumeUnitOfMeasureKey
                };
                storageSpaceInventoryKey = _storageSpaceInventoryRepository.InsertStorageSpaceInventory(actionContext, storageSpaceInventoryModel);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }

            return storageSpaceInventoryKey.GetValueOrDefault();
        }

        public Guid UpdateStorageSpaceInventory(Context context, StorageSpaceInventory storageSpaceInventory)
        {
            return UpdateStorageSpaceInventory(context.ActionDateTime, context.ActionUtcDateTime, (Guid?)context.Device,
                (Guid?)context.Actor, storageSpaceInventory);
        }

        public Guid UpdateStorageSpaceInventory(DateTime actionDateTime, DateTime actionUtcDateTime, Guid? deviceKey,
            Guid? actorKey, StorageSpaceInventory storageSpaceInventory)
        {
            Guid? storageSpaceInventoryKey = null;

            try
            {
                var actionContext = new ActionContext
                {
                    ActionDateTime = actionDateTime,
                    ActionLocalDateTime = actionDateTime,
                    ActionUtcDateTime = actionUtcDateTime,
                    ActorKey = actorKey,
                    DispensingDeviceKey = deviceKey,
                    UserAccountKey = actorKey
                };

                var storageSpaceInventoryModel = new TxDAL.Models.StorageSpaceInventory
                {
                    EarliestNextExpirationDate = storageSpaceInventory.EarliestNextExpirationDate,
                    InventoryQuantity = storageSpaceInventory.InventoryQuantity,
                    LastModifiedBinaryValue = storageSpaceInventory.LastModified,
                    StorageSpaceInventoryKey = storageSpaceInventory.Key,
                    StorageSpaceItemKey = storageSpaceInventory.StorageSpaceItemKey,
                    StrengthInventoryQuantity = storageSpaceInventory.StrengthInventoryQuantity,
                    StrengthUOMKey = storageSpaceInventory.StrengthUnitOfMeasureKey,
                    VolumeInventoryQuantity = storageSpaceInventory.VolumeInventoryQuantity,
                    VolumeUOMKey = storageSpaceInventory.VolumeUnitOfMeasureKey
                };

                storageSpaceInventoryKey = _storageSpaceInventoryRepository.UpdateStorageSpaceInventory(actionContext, storageSpaceInventoryModel);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }

            return storageSpaceInventoryKey.GetValueOrDefault();
        }
        #endregion

        #region Discrepancy

        public void InsertDiscrepancy(Context context, Guid itemTransactionKey, bool autoResolve)
        {
            InsertDiscrepancy(context.ActionUtcDateTime, context.ActionDateTime, (Guid?)context.Device,
                (Guid?)context.Actor, itemTransactionKey, autoResolve);
        }

        public void InsertDiscrepancy(DateTime actionUtcDateTime, DateTime actionDateTime, Guid? deviceKey,
            Guid? userKey, Guid itemTransactionKey, bool autoResolve)
        {
            try
            {
                var actionContext = new ActionContext
                {
                    ActionDateTime = actionDateTime,
                    ActionLocalDateTime = actionDateTime,
                    ActionUtcDateTime = actionUtcDateTime,
                    ActorKey = userKey,
                    DispensingDeviceKey = deviceKey,
                    UserAccountKey = userKey
                };

                var discrepancy = new TxDAL.Models.Discrepancy
                {
                    AutoResolvedFlag = autoResolve,
                    ItemTransactionKey = itemTransactionKey,
                    ResolvedUtcDateTime = autoResolve ? actionUtcDateTime : default(DateTime?),
                    ResolvedLocalDateTime = autoResolve ? actionDateTime : default(DateTime?),
                    ResolvedUserAccountKey = null,
                    ResolvedTransactionSessionKey = null,
                    WitnessUserAccountKey = null,
                    DiscrepancyResolutionKey = null,
                    ResolutionText = null
                };
                _itemTransactionRepository.InsertDiscrepancy(actionContext, discrepancy);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }
        }

        public void UpdateDiscrepancy(Context context, Guid itemTransactionKey, Guid? priorItemTransactionKey,
            Guid? witnessUserAccountKey, Guid? resolutionKey, string resolutionText, Guid? transactionSessionKey,
            byte[] lastModified)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                var discrepancy = new TxDAL.Models.Discrepancy
                {
                    ItemTransactionKey = itemTransactionKey,
                    ResolvedUtcDateTime = context.ActionUtcDateTime,
                    ResolvedLocalDateTime = context.ActionDateTime,
                    ResolvedUserAccountKey = context.User.Key,
                    ResolvedTransactionSessionKey = transactionSessionKey,
                    WitnessUserAccountKey = witnessUserAccountKey,
                    DiscrepancyResolutionKey = resolutionKey,
                    ResolutionText = resolutionText,
                    PriorItemTransactionKey = priorItemTransactionKey,
                    LastModifiedBinaryValue = lastModified
                };
                _itemTransactionRepository.UpdateDiscrepancy(context.ToActionContext(), discrepancy);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }
        }

        public void InsertDiscrepancyNote(Context context, Guid itemTransactionKey, string noteText, bool serverFlag = false)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                var discrepancyNote = new TxDAL.Models.DiscrepancyNote
                {
                    ItemTransactionKey = itemTransactionKey,
                    NoteText = noteText,
                    ServerFlag = serverFlag
                };
                _itemTransactionRepository.InsertDiscrepancyNote(context.ToActionContext(), discrepancyNote);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                {
                    throw;
                }
            }
        }

        #endregion
    }
}
