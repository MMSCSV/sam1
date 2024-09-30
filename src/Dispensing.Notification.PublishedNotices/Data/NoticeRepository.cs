using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Pyxis.Core.Data;
using Pyxis.Dispensing.Notification.PublishedNotices.Models;

namespace Pyxis.Dispensing.Notification.PublishedNotices.Data
{
    internal interface INoticeRepository
    {
        #region OMNL Notice Members

        EncounterUnit GetEncounterUnit(Guid encounterKey);

        IReadOnlyCollection<Guid> GetEquivalentItems(Guid itemKey);

        IReadOnlyCollection<Guid> GetVariableDoseGroupMedItems(Guid variableDoseGroupKey);

        IReadOnlyCollection<Guid> GetComboComponentMedItems(Guid comboMedFacilityItemKey);

        IReadOnlyCollection<Guid> GetDevicesWhereItemNotLoaded(Guid itemKey, Guid unitKey);

        #endregion

        #region Dispensing Device Inventory Notice Members

        IReadOnlyCollection<InventoryItem> GetStockedOutItems();

        IReadOnlyCollection<InventoryItem> GetCriticalLowItems();

        void UpdateDispensingDeviceInventoryStates(out int updatedRecordCount);

        #endregion
    }

    internal class NoticeRepository : INoticeRepository
    {
        #region #region OMNL Notice Members

        public EncounterUnit GetEncounterUnit(Guid encounterKey)
        {
            EncounterUnit encounterUnit = null;

            try
            {
                SqlBuilder query = new SqlBuilder();
                query.SELECT()
                    ._("epl.EncounterKey")
                    ._("us.UnitKey")
                    ._("us.OMNLNoticePrinterName")
                    ._("us.FacilityKey")
                    ._("fs.OMNLToPrintEquivalenciesFlag")
                    .FROM("ADT.EncounterPatientLocation epl")
                    .INNER_JOIN("Location.UnitSnapshot us ON us.UnitKey = epl.AssignedUnitKey")
                    .INNER_JOIN("Location.FacilitySnapshot fs ON fs.FacilityKey = us.FacilityKey")
                    .WHERE("epl.EndUTCDateTime IS NULL")
                    .WHERE("epl.EncounterKey = @EncounterKey")
                    .WHERE("us.EndUTCDateTime IS NULL")
                    .WHERE("us.DeleteFlag = 0")
                    .WHERE("fs.EndUTCDateTime IS NULL");

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    encounterUnit = connectionScope.Connection.Query<EncounterUnit>(
                        query.ToString(),
                        new { EncounterKey = encounterKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return encounterUnit;
        }

        public IReadOnlyCollection<Guid> GetEquivalentItems(Guid itemKey)
        {
            List<Guid> equivalentItemKeys = new List<Guid>();

            try
            {
                SqlBuilder query = new SqlBuilder();
                query.SELECT("DISTINCT ie.EquivalentItemKey")
                    .FROM("Item.ItemEquivalencySet ies")
                    .INNER_JOIN("Item.ItemEquivalency ie ON ie.ItemEquivalencySetKey = ies.ItemEquivalencySetKey")
                    .WHERE("ies.EndUTCDateTime IS NULL")
                    .WHERE("ies.ItemKey = @ItemKey")
                    .WHERE("ies.ApprovedFlag = 1");

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    equivalentItemKeys = connectionScope.Connection.Query(
                        query.ToString(),
                        new { ItemKey = itemKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Select(result => (Guid)result.EquivalentItemKey)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return equivalentItemKeys;
        }

        public IReadOnlyCollection<Guid> GetVariableDoseGroupMedItems(Guid variableDoseGroupKey)
        {
            List<Guid> equivalentItemKeys = new List<Guid>();

            try
            {
                SqlBuilder query = new SqlBuilder();
                query.SELECT("DISTINCT vdgms.MedItemKey")
                    .FROM("Item.VariableDoseGroupMemberSnapshot vdgms")
                    .WHERE("vdgms.EndUTCDateTime IS NULL")
                    .WHERE("vdgms.DeleteFlag = 0")
                    .WHERE("vdgms.VariableDoseGroupKey = @VariableDoseGroupKey");

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    equivalentItemKeys = connectionScope.Connection.Query(
                        query.ToString(),
                        new { VariableDoseGroupKey = variableDoseGroupKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Select(result => (Guid)result.MedItemKey)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return equivalentItemKeys;
        }

        public IReadOnlyCollection<Guid> GetComboComponentMedItems(Guid comboMedFacilityItemKey)
        {
            List<Guid> comboMedItemKeys = new List<Guid>();

            try
            {
                SqlBuilder query = new SqlBuilder();
                query.SELECT("DISTINCT cfi.ItemKey")
                    .FROM("Item.ComboComponentSnapshot ccs")
                    .INNER_JOIN(" Item.FacilityItem cfi ON cfi.FacilityItemKey = ccs.ComponentFacilityItemKey")
                    .WHERE("ccs.EndUTCDateTime IS NULL")
                    .WHERE("ccs.DeleteFlag = 0")
                    .WHERE("ccs.ComboFacilityItemKey = @ComboFacilityItemKey");

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    comboMedItemKeys = connectionScope.Connection.Query(
                        query.ToString(),
                        new { ComboFacilityItemKey = comboMedFacilityItemKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Select(result => (Guid)result.ItemKey)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return comboMedItemKeys;
        }

        public IReadOnlyCollection<Guid> GetDevicesWhereItemNotLoaded(Guid itemKey, Guid unitKey)
        {
            List<Guid> dispensingDeviceKeys = new List<Guid>();

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    dispensingDeviceKeys = connectionScope.Query(
                        "Notice.bsp_ListDevicesWhereItemNotLoaded",
                        new
                        {
                            ItemKey = itemKey,
                            UnitKey = unitKey
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure)
                        .Select(x => (Guid)x.DispensingDeviceKey)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return dispensingDeviceKeys;
        }

        #endregion

        #region Dispensing Device Inventory Notice Members

        public IReadOnlyCollection<InventoryItem> GetStockedOutItems()
        {
            List<InventoryItem> noticeInventoryItems = new List<InventoryItem>();

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    noticeInventoryItems = connectionScope.Query<InventoryItem>(
                        "Notice.bsp_ListStockedOutItems",
                        null,
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return noticeInventoryItems;
        }

        public IReadOnlyCollection<InventoryItem> GetCriticalLowItems()
        {
            List<InventoryItem> noticeInventoryItems = new List<InventoryItem>();

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    noticeInventoryItems = connectionScope.Query<InventoryItem>(
                        "Notice.bsp_ListCriticalLowItems",
                        null,
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return noticeInventoryItems;
        }

        public void UpdateDispensingDeviceInventoryStates(out int updatedRecordCount)
        {
            updatedRecordCount = 0;

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@UpdatedRowCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Strg.bsp_DispensingDeviceInventoryStateInsert",
                        parameters,
                        null,
                        connectionScope.DefaultCommandTimeout,
                        CommandType.StoredProcedure);
                }

                updatedRecordCount = parameters.Get<int>("@UpdatedRowCount");
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #endregion
    }
}
