using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data.Models;
using CareFusion.Dispensing.Models;
using Dapper;
using Pyxis.Core.Data;
using Pyxis.Core.Data.InternalCodes;
using Pyxis.Core.Data.Schema.TableTypes;
using HcOrderDAL = Pyxis.Core.Data.Schema.HcOrder;

namespace CareFusion.Dispensing.Data.Repositories
{
    #region IPharmacyOrderRepository

    public interface IPharmacyOrderRepository
    {
        IReadOnlyCollection<Guid> ListOrderedItemKeys(Guid dispensingDeviceKey);

        IReadOnlyCollection<Guid> ListActivePharmacyOrderKeysForEncounters(
            IEnumerable<Guid> encounterKeys,
            DateTime startUtcDateTime,
            DateTime endUtcDateTime);

        IDictionary<Guid, int> ListPharmacyOrderActivity(IEnumerable<Guid> pharmacyOrderKeys);

        IReadOnlyCollection<PharmacyOrder> GetPharmacyOrders(IEnumerable<Guid> pharmacyOrderKeys = null, Guid? externalSystemKey = null,
            Guid? encounterKey = null, string pharmacyOrderId = null);

        PharmacyOrder GetPharmacyOrder(Guid pharmacyOrderKey);

        Guid InsertPharmacyOrder(Context context, PharmacyOrder order);

        void UpdatePharmacyOrder(Context context, PharmacyOrder order);

        void UpdatePharmacyOrderActivity(Guid pharmacyOrderKey, Guid? dispensingDeviceKey = null);

        void UpdatePharmacyOrderActivityForItemTransaction(Guid itemTransactionKey, Guid? dispensingDeviceKey = null);
    }

    #endregion

    public class PharmacyOrderRepository : IPharmacyOrderRepository
    {
        static PharmacyOrderRepository()
        {
            // Dapper custom mappings
        }

        IReadOnlyCollection<Guid> IPharmacyOrderRepository.ListOrderedItemKeys(Guid dispensingDeviceKey)
        {
            IReadOnlyCollection<Guid> itemKeys = new List<Guid>();

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    itemKeys = connectionScope.Connection.Query(
                        "HcOrder.bsp_ListOrderedItemKeysForDevice",
                        new {DispensingDeviceKey = dispensingDeviceKey},
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure)
                        .Select(result => (Guid)result.ItemKey)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return itemKeys;
        }

        IReadOnlyCollection<Guid> IPharmacyOrderRepository.ListActivePharmacyOrderKeysForEncounters(IEnumerable<Guid> encounterKeys, DateTime startUtcDateTime,
            DateTime endUtcDateTime)
        {
            IReadOnlyCollection<Guid> pharmacyOrderKeys = new List<Guid>();
            if(encounterKeys != null && !encounterKeys.Any())
                return pharmacyOrderKeys; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable(encounterKeys.Distinct());

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    pharmacyOrderKeys = connectionScope.Connection.Query(
                        "HcOrder.bsp_ListPharmacyOrderKeysForEncounters",
                        new
                        {
                            SelectedKeys = selectedKeys.AsTableValuedParameter(),
                            OrderStartUtcDateTime = startUtcDateTime,
                            OrderEndUtcDateTime = endUtcDateTime
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure)
                        .Select(result => (Guid)result.PharmacyOrderKey)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return pharmacyOrderKeys;
        }

        IDictionary<Guid, int> IPharmacyOrderRepository.ListPharmacyOrderActivity(IEnumerable<Guid> pharmacyOrderKeys)
        {
            Dictionary<Guid, int> orderRemoveCount = new Dictionary<Guid, int>();
            if (pharmacyOrderKeys != null && !pharmacyOrderKeys.Any())
                return orderRemoveCount; // Empty results

            try
            {
                GuidKeyTable orderKeys = new GuidKeyTable();
                if (pharmacyOrderKeys != null)
                    orderKeys = new GuidKeyTable(pharmacyOrderKeys.Distinct());

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var results = connectionScope.Connection.Query(
                        "HcOrder.bsp_ListPharmacyOrderActivity",
                        new
                        {
                            OrderKeys = orderKeys.AsTableValuedParameter()
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure)
                        .ToList();

                    // populate dictionary
                    foreach (var result in results)
                    {
                        orderRemoveCount.Add((Guid)result.PharmacyOrderKey, (int)result.NetRemoveOccurrenceQuantity);
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return orderRemoveCount;
        }

        IReadOnlyCollection<PharmacyOrder> IPharmacyOrderRepository.GetPharmacyOrders(IEnumerable<Guid> pharmacyOrderKeys, Guid? externalSystemKey,
            Guid? encounterKey, string pharmacyOrderId)
        {
            List<PharmacyOrder> pharmacyOrders = new List<PharmacyOrder>();
            if (pharmacyOrderKeys != null && !pharmacyOrderKeys.Any())
                return pharmacyOrders; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (pharmacyOrderKeys != null)
                {
                    selectedKeys = new GuidKeyTable(pharmacyOrderKeys.Distinct());
                }

                using (var connectionScope = ConnectionScopeFactory.Create())
                using (var multi = connectionScope.QueryMultiple(
                    "HcOrder.bsp_GetPharmacyOrders",
                    new
                    {
                        SelectedKeys = selectedKeys.AsTableValuedParameter(),
                        ExternalSystemKey = externalSystemKey,
                        EncounterKey = encounterKey,
                        PharmacyOrderID = pharmacyOrderId
                    },
                    commandTimeout: connectionScope.DefaultCommandTimeout,
                    commandType: CommandType.StoredProcedure))
                {
                    var pharmacyOrderResults = multi.Read<PharmacyOrderResult>();
                    var routeSetResults = multi.Read<PharmacyOrderRouteSetResult>();
                    var routeResults = multi.Read<PharmacyOrderRouteResult>();
                    var componentSetResults = multi.Read<PharmacyOrderComponentSetResult>();
                    var componentResults = multi.Read<PharmacyOrderComponentResult>();
                    var orderingPersonSetResults = multi.Read<PharmacyOrderOrderingPersonSetResult>();
                    var orderingPersonResults = multi.Read<PharmacyOrderOrderingPersonResult>();
                    var adminInstructionSetResults = multi.Read<PharmacyOrderAdminInstructionSetResult>();
                    var adminInstructionResults = multi.Read<PharmacyOrderAdminInstructionResult>();
                    var dispensingInstructionSetResults = multi.Read<PharmacyOrderSupplierDispensingInstructionSetResult>();
                    var dispensingInstructionResults = multi.Read<PharmacyOrderSupplierDispensingInstructionResult>();
                    var timingRecordSetResults = multi.Read<PharmacyOrderTimingRecordSetResult>();
                    var timingRecordResults = multi.Read<PharmacyOrderTimingRecordResult>();
                    var repeatPatternResults = multi.Read<PharmacyOrderRepeatPatternResult>();
                    var explicitTimeResults = multi.Read<PharmacyOrderExplicitTimeResult>();
                    var priorityResults = multi.Read<PharmacyOrderPriorityResult>();
                    var deliveryStateResults = multi.Read<PharmacyOrderDeliveryStateResult>();

                    // Create route sets
                    IDictionary<Guid, PharmacyOrderRouteSet> routeSets = CreatePharmacyOrderRouteSets(
                        routeSetResults, routeResults);

                    // Create component sets
                    IDictionary<Guid, PharmacyOrderComponentSet> componentSets = CreatePharmacyOrderComponentSets(
                        componentSetResults, componentResults);

                    // Create ordering person sets
                    IDictionary<Guid, PharmacyOrderOrderingPersonSet> orderingPersonSets = CreatePharmacyOrderOrderingPersonSets(
                            orderingPersonSetResults, orderingPersonResults);

                    // Create supplier dispensing instruction sets
                    IDictionary<Guid, PharmacyOrderSupplierDispensingInstructionSet> dispensingInstructionSets = CreatePharmacyOrderSupplierDispensingInstructionSets(
                        dispensingInstructionSetResults, dispensingInstructionResults);

                    // Create administration instruction sets
                    IDictionary<Guid, PharmacyOrderAdminInstructionSet> adminInstructionSets = CreatePharmacyOrderAdminInstructionSets(
                            adminInstructionSetResults, adminInstructionResults);

                    // Create timing record sets
                    IDictionary<Guid, PharmacyOrderTimingRecordSet> timingRecordSets = CreatePharmacyOrderTimingRecordSets(
                        timingRecordSetResults, timingRecordResults, repeatPatternResults, explicitTimeResults, priorityResults);

                    // Create Delivery States
                    IDictionary<Guid, PharmacyOrderDeliveryState> deliveryStates = CreateDeliveryStateInfo(deliveryStateResults);

                    // Create pharmacy orders
                    foreach (var pharmacyOrderResult in pharmacyOrderResults)
                    {
                        PharmacyOrder pharmacyOrder = new PharmacyOrder(pharmacyOrderResult.PharmacyOrderKey, pharmacyOrderResult.PharmacyOrderSnapshotKey)
                        {
                            EffectiveUtcDateTime = pharmacyOrderResult.EffectiveUtcDateTime,
                            EffectiveDateTime = pharmacyOrderResult.EffectiveLocalDateTime,
                            IsEffectiveDateOnly = pharmacyOrderResult.EffectiveDateOnlyFlag,
                            ExpirationUtcDateTime = pharmacyOrderResult.ExpirationUtcDateTime,
                            ExpirationDateTime = pharmacyOrderResult.ExpirationLocalDateTime,
                            IsExpirationDateOnly = pharmacyOrderResult.ExpirationDateOnlyFlag,
                            TotalOccurrenceCount = pharmacyOrderResult.TotalOccurrenceQuantity,
                            NetRemoveOccurrenceCount = pharmacyOrderResult.NetRemoveOccurrenceQuantity,
                            Completed = pharmacyOrderResult.CompletedFlag,
                            ActiveNow = pharmacyOrderResult.ActiveNowFlag,
                            OnHoldNow = pharmacyOrderResult.OnHoldNowFlag,
                            CreatedUtcDateTime = pharmacyOrderResult.CreatedUtcDateTime.GetValueOrDefault(), // Nullable in database temporarily due to upgrades
                            CreatedDateTime = pharmacyOrderResult.CreatedLocalDateTime.GetValueOrDefault(), // Nullable in database temporarily due to upgrades
                            ExternalSystemKey = pharmacyOrderResult.ExternalSystemKey,
                            EncounterKey = pharmacyOrderResult.EncounterKey,
                            PharmacyOrderId = pharmacyOrderResult.PharmacyOrderId,
                            IsDiscontinued = pharmacyOrderResult.DiscontinuedFlag,
                            IsCancelled = pharmacyOrderResult.CancelledFlag,
                            HoldEffectiveDateTime = pharmacyOrderResult.HoldEffectiveLocalDateTime,
                            HoldEffectiveUtcDateTime = pharmacyOrderResult.HoldEffectiveUtcDateTime,
                            IsHoldEffectiveDateOnly = pharmacyOrderResult.HoldEffectiveDateOnlyFlag,
                            ReleaseHoldEffectiveDateTime = pharmacyOrderResult.ReleaseHoldEffectiveLocalDateTime,
                            ReleaseHoldEffectiveUtcDateTime = pharmacyOrderResult.ReleaseHoldEffectiveUtcDateTime,
                            IsReleaseHoldEffectiveDateOnly = pharmacyOrderResult.ReleaseHoldEffectiveDateOnlyFlag,
                            CancelledUtcDateTime = pharmacyOrderResult.CancelledUtcDateTime,
                            CancelledDateTime = pharmacyOrderResult.CancelledLocalDateTime,
                            GiveItemKey = pharmacyOrderResult.GiveItemKey,
                            GiveId = pharmacyOrderResult.GiveId,
                            GiveDescription = pharmacyOrderResult.GiveDescriptionText,
                            GiveAmount = pharmacyOrderResult.GiveAmount,
                            MaximumGiveAmount = pharmacyOrderResult.MaximumGiveAmount,
                            GiveAmountUnitOfMeasure = pharmacyOrderResult.GiveUOMKey != null
                                    ? new UnitOfMeasure(pharmacyOrderResult.GiveUOMKey.Value)
                                    {
                                        DisplayCode = pharmacyOrderResult.GiveUOMDisplayCode,
                                        BaseUnitOfMeasureKey = pharmacyOrderResult.GiveUOMBaseUOMKey,
                                        Conversion = pharmacyOrderResult.GiveUOMConversionAmount
                                    } : null,
                            GiveAmountExternalUnitOfMeasureKey = pharmacyOrderResult.GiveExternalUOMKey,
                            DispenseQuantity = pharmacyOrderResult.DispenseQuantity,
                            PharmacyOrderType = pharmacyOrderResult.PharmacyOrderTypeInternalCode.FromNullableInternalCode<PharmacyOrderTypeInternalCode>(),
                            InboundWarning = pharmacyOrderResult.InboundWarningFlag,
                            SchedulePersistable = pharmacyOrderResult.SchedulePersistableFlag,
                            DispenseNotUsingDispenseDevice = pharmacyOrderResult.DispenseNotUsingDispensingDeviceFlag,
                            AdminInstructionSet = adminInstructionSets.ContainsKey(pharmacyOrderResult.PharmacyOrderKey)
                                    ? adminInstructionSets[pharmacyOrderResult.PharmacyOrderKey] : null,
                            ComponentSet = componentSets.ContainsKey(pharmacyOrderResult.PharmacyOrderKey)
                                    ? componentSets[pharmacyOrderResult.PharmacyOrderKey] : null,
                            OrderingPersonSet = orderingPersonSets.ContainsKey(pharmacyOrderResult.PharmacyOrderKey)
                                    ? orderingPersonSets[pharmacyOrderResult.PharmacyOrderKey] : null,
                            RouteSet = routeSets.ContainsKey(pharmacyOrderResult.PharmacyOrderKey)
                                    ? routeSets[pharmacyOrderResult.PharmacyOrderKey] : null,
                            SupplierDispensingInstructionSet = dispensingInstructionSets.ContainsKey(pharmacyOrderResult.PharmacyOrderKey)
                                    ? dispensingInstructionSets[pharmacyOrderResult.PharmacyOrderKey] : null,
                            TimingRecordSet = timingRecordSets.ContainsKey(pharmacyOrderResult.PharmacyOrderKey)
                                    ? timingRecordSets[pharmacyOrderResult.PharmacyOrderKey] : null,
                            LastModified = pharmacyOrderResult.LastModifiedBinaryValue.ToArray(),
                            DeliveryStateInfo = deliveryStates.ContainsKey(pharmacyOrderResult.PharmacyOrderKey)
                                    ? deliveryStates[pharmacyOrderResult.PharmacyOrderKey] : null,
                        };

                        pharmacyOrders.Add(pharmacyOrder);
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return pharmacyOrders;
        }

        PharmacyOrder IPharmacyOrderRepository.GetPharmacyOrder(Guid pharmacyOrderKey)
        {
            IReadOnlyCollection<PharmacyOrder> pharmacyOrders = ((IPharmacyOrderRepository)this).GetPharmacyOrders(
                new[] { pharmacyOrderKey });

            return pharmacyOrders.FirstOrDefault();
        }

        Guid IPharmacyOrderRepository.InsertPharmacyOrder(Context context, PharmacyOrder order)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(order, "order");
            Guid pharmacyOrderKey = Guid.Empty;

            try
            {
                HcOrderDAL.IPharmacyOrderRepository pharmacyOrderRepository = new HcOrderDAL.PharmacyOrderRepository();

                using (ITransactionScope tx = TransactionScopeFactory.Create())
                using (ConnectionScopeFactory.Create())
                {
                    pharmacyOrderKey = pharmacyOrderRepository.InsertPharmacyOrder(context.ToActionContext(), 
                        new HcOrderDAL.Models.PharmacyOrder
                    {
                        EffectiveUtcDateTime = order.EffectiveUtcDateTime,
                        EffectiveLocalDateTime = order.EffectiveDateTime,
                        EffectiveDateOnlyFlag = order.IsEffectiveDateOnly,
                        ExpirationUtcDateTime = order.ExpirationUtcDateTime,
                        ExpirationLocalDateTime = order.ExpirationDateTime,
                        ExpirationDateOnlyFlag = order.IsExpirationDateOnly,
                        TotalOccurrenceQuantity = order.TotalOccurrenceCount,
                        ExternalSystemKey = order.ExternalSystemKey,
                        EncounterKey = order.EncounterKey,
                        PharmacyOrderId = order.PharmacyOrderId,
                        DiscontinuedFlag = order.IsDiscontinued,
                        CancelledFlag = order.IsCancelled,
                        HoldEffectiveUtcDateTime = order.HoldEffectiveUtcDateTime,
                        HoldEffectiveLocalDateTime = order.HoldEffectiveDateTime,
                        HoldEffectiveDateOnlyFlag = order.IsHoldEffectiveDateOnly,
                        ReleaseHoldEffectiveUtcDateTime = order.ReleaseHoldEffectiveUtcDateTime,
                        ReleaseHoldEffectiveLocalDateTime = order.ReleaseHoldEffectiveDateTime,
                        ReleaseHoldEffectiveDateOnlyFlag = order.IsReleaseHoldEffectiveDateOnly,
                        CancelledUtcDateTime = order.CancelledUtcDateTime,
                        CancelledLocalDateTime = order.CancelledDateTime,
                        GiveItemKey = order.GiveItemKey,
                        GiveId = order.GiveId,
                        GiveDescriptionText = order.GiveDescription,
                        GiveAmount = order.GiveAmount,
                        MaximumGiveAmount = order.MaximumGiveAmount,
                        GiveUOMKey = order.GiveAmountUnitOfMeasure != null ? order.GiveAmountUnitOfMeasure.Key : default(Guid?),
                        GiveExternalUOMKey = order.GiveAmountExternalUnitOfMeasureKey,
                        DispenseQuantity = order.DispenseQuantity,
                        PharmacyOrderTypeInternalCode = order.PharmacyOrderType.ToInternalCode(),
                        InboundWarningFlag = order.InboundWarning,
                        SchedulePersistableFlag = order.SchedulePersistable,
                        DispenseNotUsingDispensingDeviceFlag = order.DispenseNotUsingDispenseDevice
                    });

                    // add routes
                    if (order.RouteSet != null &&
                        order.RouteSet.Any())
                    {
                        InsertPharmacyOrderRoutes(pharmacyOrderRepository, context, pharmacyOrderKey, order.RouteSet);
                    }

                    // add components
                    if (order.ComponentSet != null &&
                        order.ComponentSet.Any())
                    {
                        InsertOrUpdatePharmacyOrderComponentSet(pharmacyOrderRepository, context, pharmacyOrderKey, order.ComponentSet);
                    }

                    // add ordering person
                    if (order.OrderingPersonSet != null &&
                        order.OrderingPersonSet.Any())
                    {
                        InsertPharmacyOrderOrderingPersons(pharmacyOrderRepository, context, pharmacyOrderKey, order.OrderingPersonSet);
                    }

                    // add admin instructions
                    if (order.AdminInstructionSet != null &&
                        order.AdminInstructionSet.Any())
                    {
                        InsertPharmacyOrderAdminInstructions(pharmacyOrderRepository, context, pharmacyOrderKey, order.AdminInstructionSet);
                    }

                    // add dispensing instructions
                    if (order.SupplierDispensingInstructionSet != null &&
                        order.SupplierDispensingInstructionSet.Any())
                    {
                        InsertPharmacyOrderSupplierDispensingInstructions(pharmacyOrderRepository, context, pharmacyOrderKey, order.SupplierDispensingInstructionSet);
                    }

                    // add timing record set
                    if (order.TimingRecordSet != null &&
                        order.TimingRecordSet.Any())
                    {
                        InsertPharmacyOrderTimingRecords(pharmacyOrderRepository, context, pharmacyOrderKey, order.TimingRecordSet);
                    }

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return pharmacyOrderKey;
        }

        void IPharmacyOrderRepository.UpdatePharmacyOrder(Context context, PharmacyOrder order)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(order, "order");

            try
            {
                HcOrderDAL.IPharmacyOrderRepository pharmacyOrderRepository = new HcOrderDAL.PharmacyOrderRepository();

                using (ITransactionScope tx = TransactionScopeFactory.Create())
                using (ConnectionScopeFactory.Create())
                {
                    pharmacyOrderRepository.UpdatePharmacyOrder(context.ToActionContext(), new HcOrderDAL.Models.PharmacyOrder
                    {
                        EffectiveUtcDateTime = order.EffectiveUtcDateTime,
                        EffectiveLocalDateTime = order.EffectiveDateTime,
                        EffectiveDateOnlyFlag = order.IsEffectiveDateOnly,
                        ExpirationUtcDateTime = order.ExpirationUtcDateTime,
                        ExpirationLocalDateTime = order.ExpirationDateTime,
                        ExpirationDateOnlyFlag = order.IsExpirationDateOnly,
                        TotalOccurrenceQuantity = order.TotalOccurrenceCount,
                        ExternalSystemKey = order.ExternalSystemKey,
                        EncounterKey = order.EncounterKey,
                        PharmacyOrderId = order.PharmacyOrderId,
                        DiscontinuedFlag = order.IsDiscontinued,
                        CancelledFlag = order.IsCancelled,
                        HoldEffectiveUtcDateTime = order.HoldEffectiveUtcDateTime,
                        HoldEffectiveLocalDateTime = order.HoldEffectiveDateTime,
                        HoldEffectiveDateOnlyFlag = order.IsHoldEffectiveDateOnly,
                        ReleaseHoldEffectiveUtcDateTime = order.ReleaseHoldEffectiveUtcDateTime,
                        ReleaseHoldEffectiveLocalDateTime = order.ReleaseHoldEffectiveDateTime,
                        ReleaseHoldEffectiveDateOnlyFlag = order.IsReleaseHoldEffectiveDateOnly,
                        CancelledUtcDateTime = order.CancelledUtcDateTime,
                        CancelledLocalDateTime = order.CancelledDateTime,
                        GiveItemKey = order.GiveItemKey,
                        GiveId = order.GiveId,
                        GiveDescriptionText = order.GiveDescription,
                        GiveAmount = order.GiveAmount,
                        MaximumGiveAmount = order.MaximumGiveAmount,
                        GiveUOMKey = order.GiveAmountUnitOfMeasure != null ? order.GiveAmountUnitOfMeasure.Key : default(Guid?),
                        GiveExternalUOMKey = order.GiveAmountExternalUnitOfMeasureKey,
                        DispenseQuantity = order.DispenseQuantity,
                        PharmacyOrderTypeInternalCode = order.PharmacyOrderType.ToInternalCode(),
                        InboundWarningFlag = order.InboundWarning,
                        SchedulePersistableFlag = order.SchedulePersistable,
                        DispenseNotUsingDispensingDeviceFlag = order.DispenseNotUsingDispenseDevice,
                        LastModifiedBinaryValue = order.LastModified,
                        PharmacyOrderKey = order.Key
                    });

                    //update Routeset
                    if (order.RouteSet != null)
                    {
                        if (order.RouteSet.IsTransient() || order.RouteSet.Any())
                            InsertPharmacyOrderRoutes(pharmacyOrderRepository, context, order.Key, order.RouteSet);
                        else
                            DeletePharmacyOrderRoutes(pharmacyOrderRepository, context, order.Key);
                    }

                    //update componentset
                    if (order.ComponentSet != null)
                    {
                        if (order.ComponentSet.IsTransient() || order.ComponentSet.Any())
                            InsertOrUpdatePharmacyOrderComponentSet(pharmacyOrderRepository, context, order.Key, order.ComponentSet);
                        else
                            DeletePharmacyOrderComponentSet(pharmacyOrderRepository, context, order.Key);
                    }

                    //update ordering person set
                    if (order.OrderingPersonSet != null)
                    {
                        if (order.OrderingPersonSet.IsTransient() || order.OrderingPersonSet.Any())
                            InsertPharmacyOrderOrderingPersons(pharmacyOrderRepository, context, order.Key, order.OrderingPersonSet);
                        else
                            DeleteOrderingPersonSet(pharmacyOrderRepository, context, order.Key);
                    }

                    //update Admin instructions
                    if (order.AdminInstructionSet != null)
                    {
                        if (order.AdminInstructionSet.IsTransient() || order.AdminInstructionSet.Any())
                            InsertPharmacyOrderAdminInstructions(pharmacyOrderRepository, context, order.Key, order.AdminInstructionSet);
                        else
                            DeleteAdminInstructionSet(pharmacyOrderRepository, context, order.Key);
                    }

                    //update dispensing instructions
                    if (order.SupplierDispensingInstructionSet != null)
                    {
                        if (order.SupplierDispensingInstructionSet.IsTransient() || order.SupplierDispensingInstructionSet.Any())
                            InsertPharmacyOrderSupplierDispensingInstructions(pharmacyOrderRepository, context, order.Key, order.SupplierDispensingInstructionSet);
                        else
                            DeleteSupplierDispensingInstructionSet(pharmacyOrderRepository, context, order.Key);
                    }

                    //update timing records
                    if (order.TimingRecordSet != null)
                    {
                        if (order.TimingRecordSet.IsTransient() || order.TimingRecordSet.Any())
                            InsertPharmacyOrderTimingRecords(pharmacyOrderRepository, context, order.Key, order.TimingRecordSet);
                        else
                            DeleteTimingRecordSet(pharmacyOrderRepository, context, order.Key);
                    }

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IPharmacyOrderRepository.UpdatePharmacyOrderActivity(Guid pharmacyOrderKey, Guid? dispensingDeviceKey)
        {
            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Connection.Execute(
                        "HcOrder.bsp_PharmacyOrderActivityUpdate",
                        new
                        {
                            PharmacyOrderKey = pharmacyOrderKey,
                            LastModifiedDispensingDeviceKey = dispensingDeviceKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IPharmacyOrderRepository.UpdatePharmacyOrderActivityForItemTransaction(Guid itemTransactionKey, Guid? dispensingDeviceKey)
        {
            try
            {
                SqlBuilder query = new SqlBuilder();
                query.SELECT("it.PharmacyOrderKey")
                    .FROM("Tx.ItemTransaction it")
                    .WHERE("it.ItemTransactionKey = @ItemTransactionKey");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var pharmacyOrderKey = connectionScope.Query(
                            query.ToString(),
                            new {ItemTransactionKey = itemTransactionKey},
                            commandTimeout: connectionScope.DefaultCommandTimeout,
                            commandType: CommandType.Text)
                        .Select(result => (Guid?) result.PharmacyOrderKey)
                        .FirstOrDefault();

                    // skip if no order key
                    if (!pharmacyOrderKey.HasValue)
                        return;

                    // update activity
                    ((IPharmacyOrderRepository)this).UpdatePharmacyOrderActivity(pharmacyOrderKey.Value, dispensingDeviceKey);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #region Create PharmacyOrder Data Contracts

        private IDictionary<Guid, PharmacyOrderRouteSet> CreatePharmacyOrderRouteSets(
            IEnumerable<PharmacyOrderRouteSetResult> routeSetResults,
            IEnumerable<PharmacyOrderRouteResult> routeResults)
        {
            Dictionary<Guid, PharmacyOrderRouteSet> routeSets = new Dictionary<Guid, PharmacyOrderRouteSet>();
            foreach (var routeSetResult in routeSetResults)
            {
                var routes = routeResults
                    .Where(por => por.PharmacyOrderRouteSetKey == routeSetResult.PharmacyOrderRouteSetKey)
                    .OrderBy(por => por.MemberNumber)
                    .Select(por => new PharmacyOrderRoute(por.PharmacyOrderRouteKey)
                    {
                        MemberNumber = por.MemberNumber,
                        AdministrationRouteCode = por.AdminRouteCode,
                        AdministrationRouteDescription = por.AdminRouteDescriptionText,
                        LastModified = por.LastModifiedBinaryValue.ToArray()
                    })
                    .ToArray();

                PharmacyOrderRouteSet orderOrderingPersonSet = new PharmacyOrderRouteSet(
                    routeSetResult.PharmacyOrderRouteSetKey, routes)
                {
                    LastModified = routeSetResult.LastModifiedBinaryValue.ToArray()
                };

                routeSets[routeSetResult.PharmacyOrderKey] = orderOrderingPersonSet;
            }

            return routeSets;
        }

        private IDictionary<Guid, PharmacyOrderComponentSet> CreatePharmacyOrderComponentSets(
            IEnumerable<PharmacyOrderComponentSetResult> componentSetResults,
            IEnumerable<PharmacyOrderComponentResult> componentResults)
        {
            Dictionary<Guid, PharmacyOrderComponentSet> componentSets = new Dictionary<Guid, PharmacyOrderComponentSet>();
            foreach (var componentSetResult in componentSetResults)
            {
                var components = componentResults
                    .Where(poc => poc.PharmacyOrderComponentSetKey == componentSetResult.PharmacyOrderComponentSetKey)
                    .OrderBy(poc => poc.MemberNumber)
                    .Select(poc => new PharmacyOrderComponent(poc.PharmacyOrderComponentKey)
                    {
                        MemberNumber = poc.MemberNumber,
                        Amount = poc.ComponentAmount,
                        AmountExternalUnitOfMeasureKey = poc.ComponentExternalUOMKey,
                        AmountUnitOfMeasure = poc.ComponentUOMKey != null
                            ? new UnitOfMeasure(poc.ComponentUOMKey.Value)
                            {
                                DisplayCode = poc.ComponentUOMDisplayCode,
                                Description = poc.ComponentUOMDescriptionText,
                                UseDosageForm = poc.ComponentUOMUseDosageFormFlag,
                                IsActive = poc.ComponentUOMActiveFlag,
                                BaseUnitOfMeasureKey = poc.ComponentUOMBaseUOMKey,
                                Conversion = poc.ComponentUOMConversionAmount
                            } : null,
                        ComponentId = poc.ComponentId,
                        ComponentType = poc.PharmacyOrderComponentTypeInternalCode.FromInternalCode<PharmacyOrderComponentTypeInternalCode>(),
                        Description = poc.ComponentDescriptionText,
                        MedItemKey = poc.MedItemKey,
                        NetRemoveOccurrenceQuantity = poc.NetRemoveOccurrenceQuantity,
                        LinkedDateTime = poc.LinkedLocalDateTime,
                        LinkedUtcDateTime = poc.LinkedUtcDateTime,
                        LastModified = poc.LastModifiedBinaryValue.ToArray()
                    })
                    .ToArray();

                PharmacyOrderComponentSet componentSet = new PharmacyOrderComponentSet(
                    componentSetResult.PharmacyOrderComponentSetKey, components)
                {
                    LastModified = componentSetResult.LastModifiedBinaryValue.ToArray()
                };

                componentSets[componentSetResult.PharmacyOrderKey] = componentSet;
            }

            return componentSets;
        }

        private IDictionary<Guid, PharmacyOrderOrderingPersonSet> CreatePharmacyOrderOrderingPersonSets(
            IEnumerable<PharmacyOrderOrderingPersonSetResult> orderingPersonSetResults,
            IEnumerable<PharmacyOrderOrderingPersonResult> orderingPersonResults)
        {
            Dictionary<Guid, PharmacyOrderOrderingPersonSet> orderingPersonSets = new Dictionary<Guid, PharmacyOrderOrderingPersonSet>();
            foreach (var orderingPersonSetResult in orderingPersonSetResults)
            {
                var persons = orderingPersonResults
                    .Where(poop => poop.PharmacyOrderOrderingPersonSetKey == orderingPersonSetResult.PharmacyOrderOrderingPersonSetKey)
                    .OrderBy(poop => poop.MemberNumber)
                    .Select(poop => new PharmacyOrderOrderingPerson(poop.PharmacyOrderOrderingPersonKey)
                    {
                        FirstName = poop.FirstName,
                        LastName = poop.LastName,
                        MiddleName = poop.MiddleName,
                        MemberNumber = poop.MemberNumber,
                        PersonId = poop.PersonId,
                        Prefix = poop.PrefixText,
                        Suffix = poop.SuffixText,
                        LastModified = poop.LastModifiedBinaryValue.ToArray()
                    })
                    .ToArray();

                PharmacyOrderOrderingPersonSet orderOrderingPersonSet = new PharmacyOrderOrderingPersonSet(
                    orderingPersonSetResult.PharmacyOrderOrderingPersonSetKey, persons)
                {
                    LastModified = orderingPersonSetResult.LastModifiedBinaryValue.ToArray()
                };

                orderingPersonSets[orderingPersonSetResult.PharmacyOrderKey] = orderOrderingPersonSet;
            }

            return orderingPersonSets;
        }

        private Dictionary<Guid, PharmacyOrderDeliveryState> CreateDeliveryStateInfo(IEnumerable<PharmacyOrderDeliveryStateResult> deliveryStateResults)
        {
            Dictionary<Guid, PharmacyOrderDeliveryState> deliveryStates = new Dictionary<Guid, PharmacyOrderDeliveryState>();
            foreach (var dsr in deliveryStateResults)
            {
                var deliveryState =
                    new PharmacyOrderDeliveryState()
                    {
                        ContactInformation = dsr.ContactInformation,
                        DeliveryLocationName = dsr.DeliveryLocationName,
                        DeliveryStateEnteredUtcDateTime = dsr.StartUtcDateTime,
                        DeliveryTrackingStatusInternalCode = dsr.ItemDeliveryTrackingStatusInternalCode.FromInternalCode<ItemDeliveryTrackingStatusInternalCode>(),
                        FirstName = dsr.FirstName,
                        Key = dsr.PharmacyOrderKey,
                        LastName = dsr.LastName,
                        MiddleName = dsr.MiddleName,
                        PharmacyOrderKey = dsr.PharmacyOrderKey
                    };

                deliveryStates[dsr.PharmacyOrderKey] = deliveryState;
            }

            return deliveryStates;
        }

        private IDictionary<Guid, PharmacyOrderAdminInstructionSet> CreatePharmacyOrderAdminInstructionSets(
            IEnumerable<PharmacyOrderAdminInstructionSetResult> adminInstructionSetResults,
            IEnumerable<PharmacyOrderAdminInstructionResult> adminInstructionResults)
        {
            Dictionary<Guid, PharmacyOrderAdminInstructionSet> adminInstructionSets = new Dictionary<Guid, PharmacyOrderAdminInstructionSet>();
            foreach (var adminInstructionSetResult in adminInstructionSetResults)
            {
                var instructions = adminInstructionResults
                    .Where(poai => poai.PharmacyOrderAdminInstructionSetKey == adminInstructionSetResult.PharmacyOrderAdminInstructionSetKey)
                    .OrderBy(poai => poai.MemberNumber)
                    .Select(poai => new PharmacyOrderAdminInstruction(poai.PharmacyOrderAdminInstructionKey)
                    {
                        MemberNumber = poai.MemberNumber,
                        Description = poai.DescriptionText,
                        Truncated = poai.TruncatedFlag,
                        LastModified = poai.LastModifiedBinaryValue.ToArray()
                    })
                    .ToArray();

                PharmacyOrderAdminInstructionSet adminInstructionSet = new PharmacyOrderAdminInstructionSet(
                    adminInstructionSetResult.PharmacyOrderAdminInstructionSetKey, instructions)
                {
                    LastModified = adminInstructionSetResult.LastModifiedBinaryValue.ToArray()
                };

                adminInstructionSets[adminInstructionSetResult.PharmacyOrderKey] = adminInstructionSet;
            }

            return adminInstructionSets;
        }

        private IDictionary<Guid, PharmacyOrderSupplierDispensingInstructionSet> CreatePharmacyOrderSupplierDispensingInstructionSets(
            IEnumerable<PharmacyOrderSupplierDispensingInstructionSetResult> dispensingInstructionSetResults,
            IEnumerable<PharmacyOrderSupplierDispensingInstructionResult> dispensingInstructionResults)
        {
            Dictionary<Guid, PharmacyOrderSupplierDispensingInstructionSet> dispensingInstructionSets = new Dictionary<Guid, PharmacyOrderSupplierDispensingInstructionSet>();
            foreach (var dispensingInstructionSetResult in dispensingInstructionSetResults)
            {
                var instructions = dispensingInstructionResults
                    .Where(posdi => posdi.PharmacyOrderSupplierDispensingInstructionSetKey == dispensingInstructionSetResult.PharmacyOrderSupplierDispensingInstructionSetKey)
                    .OrderBy(posdi => posdi.MemberNumber)
                    .Select(posdi => new PharmacyOrderSupplierDispensingInstruction(posdi.PharmacyOrderSupplierDispensingInstructionKey)
                    {
                        MemberNumber = posdi.MemberNumber,
                        Description = posdi.DescriptionText,
                        Truncated = posdi.TruncatedFlag,
                        LastModified = posdi.LastModifiedBinaryValue.ToArray()
                    })
                    .ToArray();

                PharmacyOrderSupplierDispensingInstructionSet orderOrderingPersonSet = new PharmacyOrderSupplierDispensingInstructionSet(
                    dispensingInstructionSetResult.PharmacyOrderSupplierDispensingInstructionSetKey, instructions)
                {
                    LastModified = dispensingInstructionSetResult.LastModifiedBinaryValue.ToArray()
                };

                dispensingInstructionSets[dispensingInstructionSetResult.PharmacyOrderKey] = orderOrderingPersonSet;
            }

            return dispensingInstructionSets;
        }

        private IDictionary<Guid, PharmacyOrderTimingRecordSet> CreatePharmacyOrderTimingRecordSets(
            IEnumerable<PharmacyOrderTimingRecordSetResult> timingRecordSetResults,
            IEnumerable<PharmacyOrderTimingRecordResult> timingRecordResults,
            IEnumerable<PharmacyOrderRepeatPatternResult> repeatPatternResults,
            IEnumerable<PharmacyOrderExplicitTimeResult> explicitTimeResults,
            IEnumerable<PharmacyOrderPriorityResult> priorityResults)
        {
            Dictionary<Guid, PharmacyOrderTimingRecordSet> timingRecordSets = new Dictionary<Guid, PharmacyOrderTimingRecordSet>();
            foreach (var timingRecordSetResult in timingRecordSetResults)
            {
                List<PharmacyOrderTimingRecord> timingRecords = new List<PharmacyOrderTimingRecord>();
                foreach (var timingRecordResult in timingRecordResults.Where(tr =>
                    tr.PharmacyOrderTimingRecordSetKey == timingRecordSetResult.PharmacyOrderTimingRecordSetKey))
                {
                    var repeatPatterns = repeatPatternResults
                        .Where(rp => rp.PharmacyOrderTimingRecordKey == timingRecordResult.PharmacyOrderTimingRecordKey)
                        .OrderBy(rp => rp.MemberNumber)
                        .Select(rp => new PharmacyOrderRepeatPattern(rp.PharmacyOrderRepeatPatternKey)
                        {
                            MemberNumber = rp.MemberNumber,
                            RepeatPattern = new RepeatPattern(rp.RepeatPatternKey)
                            {
                                ExternalSystemKey = rp.RepeatPatternExternalSystemKey,
                                Description = rp.RepeatPatternDescriptionText,
                                Code = rp.RepeatPatternCode,
                                StandardRepeatPatternInternalCode = rp.StandardRepeatPatternInternalCode,
                                StandardRepeatPatternDisplayCode = rp.StandardRepeatPatternDisplayCode,
                                PeriodAmount = rp.RepeatPatternPeriodAmount,
                                Monday = rp.RepeatPatternMondayFlag,
                                Tuesday = rp.RepeatPatternTuesdayFlag,
                                Wednesday = rp.RepeatPatternWednesdayFlag,
                                Thursday = rp.RepeatPatternThursdayFlag,
                                Friday = rp.RepeatPatternFridayFlag,
                                Saturday = rp.RepeatPatternSaturdayFlag,
                                Sunday = rp.RepeatPatternSundayFlag,
                            },
                            LastModified = rp.LastModifiedBinaryValue.ToArray()
                        })
                        .ToArray();

                    var explicitTimes = explicitTimeResults
                        .Where(et => et.PharmacyOrderTimingRecordKey == timingRecordResult.PharmacyOrderTimingRecordKey)
                        .OrderBy(et => et.MemberNumber)
                        .Select(et => new PharmacyOrderExplicitTime(et.PharmacyOrderExplicitTimeKey)
                        {
                            MemberNumber = et.MemberNumber,
                            TimeOfDay = et.ExplicitTimeOfDayValue,
                            LastModified = et.LastModifiedBinaryValue.ToArray()
                        })
                        .ToArray();

                    var priorities = priorityResults
                        .Where(pri => pri.PharmacyOrderTimingRecordKey == timingRecordResult.PharmacyOrderTimingRecordKey)
                        .OrderBy(pri => pri.MemberNumber)
                        .Select(pri => new PharmacyOrderPriority(pri.PharmacyOrderPriorityKey)
                        {
                            MemberNumber = pri.MemberNumber,
                            TimingRecordPriority = new TimingRecordPriority(pri.TimingRecordPriorityKey)
                            {
                                ExternalSystemKey = pri.TimingRecordPriorityExternalSystemKey,
                                Description = pri.TimingRecordDescriptionText,
                                Code = pri.TimingRecordPriorityCode,
                                StandardTimingRecordPriorityInternalCode = pri.StandardTimingRecordPriorityInternalCode,
                                StandardTimingRecordPriorityDisplayCode = pri.StandardTimingRecordPriorityDisplayCode
                            },
                            LastModified = pri.LastModifiedBinaryValue.ToArray()
                        })
                        .ToArray();

                    PharmacyOrderTimingRecord timingRecord = new PharmacyOrderTimingRecord(timingRecordResult.PharmacyOrderTimingRecordKey)
                    {
                        MemberNumber = timingRecordResult.MemberNumber,
                        ServiceDurationAmount = timingRecordResult.ServiceDurationAmount,
                        ServiceUnitOfDuration = timingRecordResult.ServiceUnitOfDurationInternalCode.FromNullableInternalCode<UnitOfDurationInternalCode>(),
                        EffectiveUtcDateTime = timingRecordResult.EffectiveUtcDateTime,
                        EffectiveLocalDateTime = timingRecordResult.EffectiveLocalDateTime,
                        IsEffectiveDateOnly = timingRecordResult.EffectiveDateOnlyFlag,
                        ExpirationUtcDateTime = timingRecordResult.ExpirationUtcDateTime,
                        ExpirationLocalDateTime = timingRecordResult.ExpirationLocalDateTime,
                        IsExpirationDateOnly = timingRecordResult.ExpirationDateOnlyFlag,
                        ConditionText = timingRecordResult.ConditionText,
                        Conjuction = timingRecordResult.TimingRecordConjunctionInternalCode.FromNullableInternalCode<TimingRecordConjunctionInternalCode>(),
                        TotalOccurrenceQuantity = timingRecordResult.TotalOccurrenceQuantity,
                        RepeatPatterns = repeatPatterns,
                        ExplicitTimes = explicitTimes,
                        Priorities = priorities,
                        LastModified = timingRecordResult.LastModifiedBinaryValue.ToArray()
                    };

                    timingRecords.Add(timingRecord);
                }

                PharmacyOrderTimingRecordSet timingRecordSet = new PharmacyOrderTimingRecordSet(
                    timingRecordSetResult.PharmacyOrderTimingRecordSetKey, timingRecords)
                {
                    LastModified = timingRecordSetResult.LastModifiedBinaryValue.ToArray()
                };

                timingRecordSets[timingRecordSetResult.PharmacyOrderKey] = timingRecordSet;
            }

            return timingRecordSets;
        }

        #endregion

        #region Private Members

        private void InsertPharmacyOrderRoutes(HcOrderDAL.IPharmacyOrderRepository pharmacyOrderRepository, Context context, Guid orderKey, PharmacyOrderRouteSet routeSet)
        {
            Guard.ArgumentNotNull(pharmacyOrderRepository, "pharmacyOrderRepository");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(routeSet, "routeSet");

            HcOrderDAL.Models.PharmacyOrderRouteSet set = new HcOrderDAL.Models.PharmacyOrderRouteSet
            {
                PharmacyOrderKey = orderKey,
                LastModifiedBinaryValue = routeSet.LastModified
            };

            routeSet.ForEach(rte => set.Add(new HcOrderDAL.Models.PharmacyOrderRoute
            {
                MemberNumber = rte.MemberNumber,
                AdminRouteCode = rte.AdministrationRouteCode,
                AdminRouteDescriptionText = rte.AdministrationRouteDescription
            }));

            pharmacyOrderRepository.InsertOrUpdatePharmacyOrderRouteSet(context.ToActionContext(), set);
        }

        private void DeletePharmacyOrderRoutes(HcOrderDAL.IPharmacyOrderRepository pharmacyOrderRepository, Context context, Guid orderKey)
        {
            Guard.ArgumentNotNull(pharmacyOrderRepository, "pharmacyOrderRepository");
            Guard.ArgumentNotNull(context, "context");

            pharmacyOrderRepository.DeletePharmacyOrderRouteSet(
                context.ToActionContext(),
                orderKey);
        }

        private void InsertPharmacyOrderTimingRecords(HcOrderDAL.IPharmacyOrderRepository pharmacyOrderRepository, Context context, Guid orderKey, PharmacyOrderTimingRecordSet timingRecordSet)
        {
            Guard.ArgumentNotNull(pharmacyOrderRepository, "pharmacyOrderRepository");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(timingRecordSet, "timingRecordSet");

            HcOrderDAL.Models.PharmacyOrderTimingRecordSet set = new HcOrderDAL.Models.PharmacyOrderTimingRecordSet
            {
                PharmacyOrderKey = orderKey,
                LastModifiedBinaryValue = timingRecordSet.LastModified
            };

            foreach (PharmacyOrderTimingRecord timingRecord in timingRecordSet)
            {
                // Explicit Times
                HcOrderDAL.Models.PharmacyOrderExplicitTime[] explicitTimes = null;
                if (timingRecord.ExplicitTimes != null)
                {
                    explicitTimes = timingRecord.ExplicitTimes.Select(et => new HcOrderDAL.Models.PharmacyOrderExplicitTime
                    {
                        MemberNumber = et.MemberNumber,
                        ExplicitTimeOfDayValue = et.TimeOfDay
                    }).ToArray();
                }

                // Priorities
                HcOrderDAL.Models.PharmacyOrderPriority[] priorities = null;
                if (timingRecord.Priorities != null)
                {
                    priorities = timingRecord.Priorities.Select(pr => new HcOrderDAL.Models.PharmacyOrderPriority
                    {
                        MemberNumber = pr.MemberNumber,
                        TimingRecordPriorityKey = pr.TimingRecordPriority.Key
                    }).ToArray();
                }

                // Repeat Patterns
                HcOrderDAL.Models.PharmacyOrderRepeatPattern[] repeatPatterns = null;
                if (timingRecord.RepeatPatterns != null)
                {
                    repeatPatterns = timingRecord.RepeatPatterns.Select(rp => new HcOrderDAL.Models.PharmacyOrderRepeatPattern
                    {
                        MemberNumber = rp.MemberNumber,
                        RepeatPatternKey = rp.RepeatPattern.Key
                    }).ToArray();
                }

                // Timing Record
                set.Add(new HcOrderDAL.Models.PharmacyOrderTimingRecord
                {
                    MemberNumber = timingRecord.MemberNumber,
                    ServiceDurationAmount = timingRecord.ServiceDurationAmount,
                    ServiceUnitOfDurationInternalCode = timingRecord.ServiceUnitOfDuration.ToInternalCode(),
                    EffectiveUtcDateTime = timingRecord.EffectiveUtcDateTime,
                    EffectiveLocalDateTime = timingRecord.EffectiveLocalDateTime,
                    EffectiveDateOnlyFlag = timingRecord.IsEffectiveDateOnly,
                    ExpirationUtcDateTime = timingRecord.ExpirationUtcDateTime,
                    ExpirationLocalDateTime = timingRecord.ExpirationLocalDateTime,
                    ExpirationDateOnlyFlag = timingRecord.IsExpirationDateOnly,
                    ConditionText = timingRecord.ConditionText,
                    TimingRecordConjunctionInternalCode = timingRecord.Conjuction.ToInternalCode(),
                    TotalOccurrenceQuantity = timingRecord.TotalOccurrenceQuantity,
                    PharmacyOrderExplicitTimes = explicitTimes,
                    PharmacyOrderPriorities = priorities,
                    PharmacyOrderRepeatPatterns = repeatPatterns
                });
            }

            pharmacyOrderRepository.InsertOrUpdatePharmacyOrderTimingRecordSet(context.ToActionContext(), set);
        }

        private void DeleteTimingRecordSet(HcOrderDAL.IPharmacyOrderRepository pharmacyOrderRepository, Context context, Guid orderKey)
        {
            Guard.ArgumentNotNull(pharmacyOrderRepository, "pharmacyOrderRepository");
            Guard.ArgumentNotNull(context, "context");

            pharmacyOrderRepository.DeletePharmacyOrderTimingRecordSet(
                context.ToActionContext(),
                orderKey);
        }

        private void InsertOrUpdatePharmacyOrderComponentSet(HcOrderDAL.IPharmacyOrderRepository pharmacyOrderRepository, Context context, Guid orderKey, PharmacyOrderComponentSet componentSet)
        {
            Guard.ArgumentNotNull(pharmacyOrderRepository, "pharmacyOrderRepository");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(componentSet, "componentSet");

            HcOrderDAL.Models.PharmacyOrderComponentSet set = new HcOrderDAL.Models.PharmacyOrderComponentSet
            {
                PharmacyOrderKey = orderKey,
                LastModifiedBinaryValue = componentSet.LastModified
            };

            componentSet.ForEach(poc => set.Add(new HcOrderDAL.Models.PharmacyOrderComponent
            {
                MemberNumber = poc.MemberNumber,
                PharmacyOrderComponentTypeInternalCode = poc.ComponentType.ToInternalCode(),
                MedItemKey = poc.MedItemKey,
                ComponentId = poc.ComponentId,
                ComponentDescriptionText = poc.Description,
                ComponentAmount = poc.Amount,
                ComponentUOMKey = poc.AmountUnitOfMeasure != null ? poc.AmountUnitOfMeasure.Key : default(Guid?),
                ComponentExternalUOMKey = poc.AmountExternalUnitOfMeasureKey,
                NetRemoveOccurrenceQuantity = poc.NetRemoveOccurrenceQuantity,
                LinkedUtcDateTime = poc.LinkedUtcDateTime,
                LinkedLocalDateTime = poc.LinkedDateTime
            }));

            pharmacyOrderRepository.InsertOrUpdatePharmacyOrderComponentSet(context.ToActionContext(), set);
        }

        private void DeletePharmacyOrderComponentSet(HcOrderDAL.IPharmacyOrderRepository pharmacyOrderRepository, Context context, Guid orderKey)
        {
            Guard.ArgumentNotNull(pharmacyOrderRepository, "pharmacyOrderRepository");
            Guard.ArgumentNotNull(context, "context");

            pharmacyOrderRepository.DeletePharmacyOrderComponentSet(
                context.ToActionContext(),
                orderKey);
        }

        private void InsertPharmacyOrderOrderingPersons(HcOrderDAL.IPharmacyOrderRepository pharmacyOrderRepository, Context context, Guid orderKey, PharmacyOrderOrderingPersonSet orderingPersonSet)
        {
            Guard.ArgumentNotNull(pharmacyOrderRepository, "pharmacyOrderRepository");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(orderingPersonSet, "orderingPersonSet");

            HcOrderDAL.Models.PharmacyOrderOrderingPersonSet set = new HcOrderDAL.Models.PharmacyOrderOrderingPersonSet
            {
                PharmacyOrderKey = orderKey,
                LastModifiedBinaryValue = orderingPersonSet.LastModified
            };

            orderingPersonSet.ForEach(poop => set.Add(new HcOrderDAL.Models.PharmacyOrderOrderingPerson
            {
                MemberNumber = poop.MemberNumber,
                PersonId = poop.PersonId,
                PrefixText = poop.Prefix,
                FirstName = poop.FirstName,
                MiddleName = poop.MiddleName,
                LastName = poop.LastName,
                SuffixText = poop.Suffix
            }));

            pharmacyOrderRepository.InsertOrUpdatePharmacyOrderOrderingPersonSet(context.ToActionContext(), set);
        }

        private void DeleteOrderingPersonSet(HcOrderDAL.IPharmacyOrderRepository pharmacyOrderRepository, Context context, Guid orderKey)
        {
            Guard.ArgumentNotNull(pharmacyOrderRepository, "pharmacyOrderRepository");
            Guard.ArgumentNotNull(context, "context");

            pharmacyOrderRepository.DeletePharmacyOrderOrderingPersonSet(
                context.ToActionContext(),
                orderKey);
        }

        private void InsertPharmacyOrderAdminInstructions(HcOrderDAL.IPharmacyOrderRepository pharmacyOrderRepository, Context context, Guid orderKey, PharmacyOrderAdminInstructionSet instructionSet)
        {
            Guard.ArgumentNotNull(pharmacyOrderRepository, "pharmacyOrderRepository");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(instructionSet, "instructionSet");

            HcOrderDAL.Models.PharmacyOrderAdminInstructionSet set = new HcOrderDAL.Models.PharmacyOrderAdminInstructionSet
            {
                PharmacyOrderKey = orderKey,
                LastModifiedBinaryValue = instructionSet.LastModified
            };

            instructionSet.ForEach(poai => set.Add(new HcOrderDAL.Models.PharmacyOrderAdminInstruction
            {
                MemberNumber = poai.MemberNumber,
                DescriptionText = poai.Description,
                TruncatedFlag = poai.Truncated
            }));

            pharmacyOrderRepository.InsertOrUpdatePharmacyOrderAdminInstructionSet(context.ToActionContext(), set);
        }

        private void DeleteAdminInstructionSet(HcOrderDAL.IPharmacyOrderRepository pharmacyOrderRepository, Context context, Guid orderKey)
        {
            Guard.ArgumentNotNull(pharmacyOrderRepository, "pharmacyOrderRepository");
            Guard.ArgumentNotNull(context, "context");

            pharmacyOrderRepository.DeletePharmacyOrderAdminInstructionSet(
                context.ToActionContext(),
                orderKey);
        }

        private void InsertPharmacyOrderSupplierDispensingInstructions(HcOrderDAL.IPharmacyOrderRepository pharmacyOrderRepository, Context context, Guid orderKey, PharmacyOrderSupplierDispensingInstructionSet instructionSet)
        {
            Guard.ArgumentNotNull(pharmacyOrderRepository, "pharmacyOrderRepository");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(instructionSet, "instructionSet");

            HcOrderDAL.Models.PharmacyOrderSupplierDispensingInstructionSet set = new HcOrderDAL.Models.
                PharmacyOrderSupplierDispensingInstructionSet
            {
                PharmacyOrderKey = orderKey,
                LastModifiedBinaryValue = instructionSet.LastModified
            };

            instructionSet.ForEach(podi => set.Add(new HcOrderDAL.Models.PharmacyOrderSupplierDispensingInstruction
            {
                MemberNumber = podi.MemberNumber,
                DescriptionText = podi.Description,
                TruncatedFlag = podi.Truncated
            }));

            pharmacyOrderRepository.InsertOrUpdatePharmacyOrderSupplierDispensingInstructionSet(context.ToActionContext(), set);
        }

        private void DeleteSupplierDispensingInstructionSet(HcOrderDAL.IPharmacyOrderRepository pharmacyOrderRepository, Context context, Guid orderKey)
        {
            Guard.ArgumentNotNull(pharmacyOrderRepository, "pharmacyOrderRepository");
            Guard.ArgumentNotNull(context, "context");

            pharmacyOrderRepository.DeletePharmacyOrderSupplierDispensingInstructionSet(
                context.ToActionContext(),
                orderKey);
        }

        #endregion
    }
}
