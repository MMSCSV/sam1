using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Models;
using Dapper;
using Pyxis.Core.Data;
using Pyxis.Core.Data.Schema;
using Pyxis.Core.Data.Schema.Core.Models;
using Pyxis.Core.Data.Schema.TableTypes;

namespace CareFusion.Dispensing.Data.Repositories
{
    #region IFacilityKitRepository Interface

    public interface IFacilityKitRepository
    {
        /// <summary>
        /// Retrieves a collection of <see cref="FacilityKit"/> by criteria.
        /// </summary>
        /// <param name="findCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>An IPagedCollection(T) object, where the generic parameter T is <see cref="FacilityKit"/>.</returns>
        IPagedCollection<FacilityKit> GetFacilityKits(FindCriteria findCriteria, int page = 1, int pageSize = Int32.MaxValue);

        /// <summary>
        /// Retrieves a collection of <see cref="FacilityKit"/> by external system.
        /// </summary>
        /// <param name="facilityKitKey"></param>
        /// <returns></returns>
        FacilityKit GetFacilityKit(Guid facilityKitKey);

        /// <summary>
        /// Checks to see if there is an existing facility kit name within facility and external system.
        /// </summary>
        /// <param name="facilityKey"></param>
        /// <param name="externalSystemKey"></param>
        /// <param name="name"></param>
        /// <param name="excludeFacilityKitFilter"></param>
        /// <returns></returns>
        bool FacilityKitNameExists(Guid facilityKey, Guid externalSystemKey, string name, Filter<Guid> excludeFacilityKitFilter = default(Filter<Guid>));

        /// <summary>
        /// Persists the <see cref="FacilityKit"/> to the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="facilityKit">The repeat pattern to save.</param>
        /// <returns>
        /// The repeat pattern surrogate key, which uniquely identifies the facility kit in the database.
        /// </returns>
        Guid InsertFacilityKit(Context context, FacilityKit facilityKit);

        /// <summary>
        /// Updates an existing <see cref="FacilityKit"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="facilityKit">The facility kit to update.</param>
        void UpdateFacilityKit(Context context, FacilityKit facilityKit);

        /// <summary>
        /// Deletes an existing <see cref="FacilityKit"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="facilityKitKey">The facility kit to delete.</param>
        void DeleteFacilityKit(Context context, Guid facilityKitKey);
    }

    #endregion

    public class FacilityKitRepository : RepositoryBase, IFacilityKitRepository
    {
        static FacilityKitRepository()
        {
            // Dapper custom mappings
            SqlMapper.SetTypeMap(
                typeof(FacilityKit),
                new ColumnAttributeTypeMapper<FacilityKit>());

            SqlMapper.SetTypeMap(
                typeof(FacilityKitItem),
                new ColumnAttributeTypeMapper<FacilityKitItem>());
        }

        IPagedCollection<FacilityKit> IFacilityKitRepository.GetFacilityKits(FindCriteria findCriteria, int page, int pageSize)
        {
            Guard.ArgumentNotNull(findCriteria, "findCriteria");

            // Make sure paging is within bounds.
            if (page < 1) page = 1;
            if (pageSize < 0) pageSize = 0;

            IPagedCollection<FacilityKit> facilityKits = PagedCollection<FacilityKit>.EmptyResults;

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable(findCriteria.SelectedKeys);
                GuidKeyTable excludedKeys = new GuidKeyTable(findCriteria.ExcludedKeys);
                GuidKeyTable facilityKeys = new GuidKeyTable(findCriteria.FacilityKeys);
                FilterCriteriaTable filterCriteria = new FilterCriteriaTable(findCriteria.Filters);

                var parameters = new DynamicParameters(
                    new Dictionary<string, object>
                        {
                            {"@PageNumber", page},
                            {"@RowsPerPage", pageSize},
                            {"@SelectedKeys", selectedKeys.AsTableValuedParameter()},
                            {"@ExcludedKeys", excludedKeys.AsTableValuedParameter()},
                            {"@FacilityKeys", facilityKeys.AsTableValuedParameter()},
                            {"@SearchText", findCriteria.SearchText},
                            {"@OrderByField", findCriteria.OrderBy},
                            {"@AscendingFlag", findCriteria.Ascending},
                            {"@FilterCriteria", filterCriteria.AsTableValuedParameter()}
                        });
                parameters.Add("@TotalRows", dbType: DbType.Int64, direction: ParameterDirection.Output);

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    using (var multi = connectionScope.QueryMultiple(
                        "Item.bsp_GetFacilityKits",
                        parameters,
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure))
                    {
                        var results = multi.Read<FacilityKit>().ToList();
                        var results2 = multi.Read<FacilityKitItem>()
                            .ToLookup(fki => fki.FacilityKitKey);
                        var results3 = multi.Read()
                            .ToLookup(x => (Guid)x.FacilityKitKey, x => (Guid)x.DispensingDeviceKey);
                        var totalRows = parameters.Get<long?>("@TotalRows");

                        foreach (var facilityKit in results)
                        {
                            if (results2.Contains(facilityKit.Key))
                            {
                                facilityKit.Items = results2[facilityKit.Key].ToArray();
                            }

                            if (results3.Contains(facilityKit.Key))
                            {
                                facilityKit.AssociatedDevices = results3[facilityKit.Key].ToArray();
                            }
                        }

                        facilityKits = new PagedCollection<FacilityKit>(results, totalRows ?? 0);
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return facilityKits;
        }

        FacilityKit IFacilityKitRepository.GetFacilityKit(Guid facilityKitKey)
        {
            FindCriteria findCriteria = new FindCriteria();
            findCriteria.SelectedKeys.Add(facilityKitKey);

            IPagedCollection<FacilityKit> facilityKits =
                ((IFacilityKitRepository)this).GetFacilityKits(findCriteria, 1, 1);

            return facilityKits.FirstOrDefault();
        }

        bool IFacilityKitRepository.FacilityKitNameExists(Guid facilityKey, Guid externalSystemKey, string name, Filter<Guid> excludeFacilityKitFilter)
        {
            bool exists = false;

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    SqlBuilder query = new SqlBuilder();
                    query.SELECT()
                        ._("FacilityKitKey")
                        .FROM("Item.FacilityKitSnapshot")
                        .WHERE("EndUTCDateTime IS NULL")
                        .WHERE("DeleteFlag = 0")
                        .WHERE("FacilityKey = @FacilityKey")
                        .WHERE("ExternalSystemKey = @ExternalSystemKey")
                        .WHERE("FacilityKitName = @FacilityKitName");

                    if (excludeFacilityKitFilter.HasValue)
                    {
                        query.WHERE("FacilityKitKey <> @ExcludeFacilityKitKey");
                    }

                    exists = connectionScope.Query(
                        query.ToString(),
                        new
                        {
                            FacilityKey = facilityKey,
                            ExternalSystemKey = externalSystemKey,
                            FacilityKitName = name,
                            ExcludeFacilityKitKey = excludeFacilityKitFilter.GetValueOrDefault()
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Any();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return exists;
        }

        Guid IFacilityKitRepository.InsertFacilityKit(Context context, FacilityKit facilityKit)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (facilityKit == null) throw new ArgumentNullException("facilityKit");

            Guid? facilityKitKey = null;

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@StartUTCDateTime", context.ActionUtcDateTime},
                             {"@StartLocalDateTime", context.ActionDateTime},
                             {"@FacilityKey", facilityKit.FacilityKey},
                             {"@FacilityKitName", facilityKit.Name},
                             {"@DescriptionText", facilityKit.Description},
                             {"@LastModifiedActorKey", (Guid?)context.Actor}
                         });
                    parameters.Add("@FacilityKitKey", dbType: DbType.Guid, direction: ParameterDirection.Output);

                    connectionScope.Execute(
                        "Item.usp_FacilityKitInsert",
                        parameters,
                        null,
                        connectionScope.DefaultCommandTimeout,
                        CommandType.StoredProcedure);

                    facilityKitKey = parameters.Get<Guid?>("@FacilityKitKey");

                    if (facilityKitKey.HasValue)
                    {
                        if (facilityKit.Items != null)
                        {
                            InsertFacilityKitItems(
                                connectionScope,
                                context,
                                facilityKitKey.Value,
                                facilityKit.Items);
                        }
                    }

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return facilityKitKey.GetValueOrDefault();
        }

        void IFacilityKitRepository.UpdateFacilityKit(Context context, FacilityKit facilityKit)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (facilityKit == null) throw new ArgumentNullException("facilityKit");

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@StartUTCDateTime", context.ActionUtcDateTime},
                             {"@StartLocalDateTime", context.ActionDateTime},
                             {"@FacilityKitName", facilityKit.Name},
                             {"@DescriptionText", facilityKit.Description},
                             {"@LastModifiedActorKey", (Guid?)context.Actor},
                             {"@LastModifiedBinaryValue", facilityKit.LastModified},
                             {"@FacilityKitKey", facilityKit.Key}
                         });

                    connectionScope.Execute(
                        "Item.usp_FacilityKitUpdate",
                        parameters,
                        null,
                        connectionScope.DefaultCommandTimeout,
                        CommandType.StoredProcedure);

                    UpdateFacilityKitItems(
                        connectionScope,
                        context,
                        facilityKit.Key,
                        facilityKit.Items ?? new FacilityKitItem[0]);

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IFacilityKitRepository.DeleteFacilityKit(Context context, Guid facilityKitKey)
        {
            if (context == null) throw new ArgumentNullException("context");

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@StartUTCDateTime", context.ActionUtcDateTime},
                             {"@StartLocalDateTime", context.ActionDateTime},
                             {"@LastModifiedActorKey", (Guid?)context.Actor},
                             {"@FacilityKitKey", facilityKitKey}
                         });

                    connectionScope.Execute(
                        "Item.usp_FacilityKitDelete",
                        parameters,
                        null,
                        connectionScope.DefaultCommandTimeout,
                        CommandType.StoredProcedure);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #region Private Members

        private static void InsertFacilityKitItems(IConnectionScope connectionScope, Context context, Guid facilityKitKey, IEnumerable<FacilityKitItem> facilityKitItems)
        {
            Guard.ArgumentNotNull(connectionScope, "connectionScope");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(facilityKitItems, "facilityKitItems");

            foreach (FacilityKitItem facilityKitItem in facilityKitItems)
            {
                var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@StartUTCDateTime", context.ActionUtcDateTime},
                             {"@StartLocalDateTime", context.ActionDateTime},
                             {"@FacilityKitKey", facilityKitKey},
                             {"@FacilityItemKey", facilityKitItem.FacilityItemKey},
                             {"@ItemMemberQuantity", facilityKitItem.Quantity},
                             {"@LastModifiedActorKey", (Guid?)context.Actor}
                         });
                parameters.Add("@FacilityKitMemberKey", dbType: DbType.Guid, direction: ParameterDirection.Output);

                connectionScope.Execute(
                    "Item.usp_FacilityKitMemberInsert",
                    parameters,
                    null,
                    connectionScope.DefaultCommandTimeout,
                    CommandType.StoredProcedure);
            }
        }

        private static void UpdateFacilityKitItems(IConnectionScope connectionScope, Context context, Guid facilityKitKey, IEnumerable<FacilityKitItem> facilityKitItems)
        {
            Guard.ArgumentNotNull(connectionScope, "connectionScope");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(facilityKitItems, "facilityKitItems");

            SqlBuilder query = new SqlBuilder();
            query.SELECT("FacilityKitMemberKey")
                .FROM("Item.FacilityKitMemberSnapshot")
                .WHERE("EndUTCDateTime IS NULL")
                .WHERE("DeleteFlag = 0")
                .WHERE("FacilityKitKey = @FacilityKitKey");

            // Get the list of facility item keys associated with this facility kit
            IReadOnlyCollection<Guid> currentfacilityItemKeys = connectionScope.Query(
                        query.ToString(),
                        new { FacilityKitKey = facilityKitKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Select(x => (Guid)x.FacilityKitMemberKey)
                        .ToList();

            // Find the facility item keys that were removed.
            IEnumerable<Guid> removedFacilityItemKeys = currentfacilityItemKeys.Except(facilityKitItems.Select(fki => fki.Key));

            // Remove facility kit item that are no longer associated with this facility kit.
            foreach (Guid facilityKitItemKey in removedFacilityItemKeys)
            {
                var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@StartUTCDateTime", context.ActionUtcDateTime},
                             {"@StartLocalDateTime", context.ActionDateTime},
                             {"@LastModifiedActorKey", (Guid?)context.Actor},
                             {"@FacilityKitMemberKey", facilityKitItemKey}
                         });

                connectionScope.Execute(
                    "Item.usp_FacilityKitMemberDelete",
                    parameters,
                    null,
                    connectionScope.DefaultCommandTimeout,
                    CommandType.StoredProcedure);
            }

            // Find the facility kit items that were added.
            List<FacilityKitItem> addedFacilityKitItems = new List<FacilityKitItem>();
            foreach (FacilityKitItem facilityKitItem in facilityKitItems)
            {
                if (facilityKitItem.IsTransient())
                {
                    addedFacilityKitItems.Add(facilityKitItem);
                    continue;
                }

                var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@StartUTCDateTime", context.ActionUtcDateTime},
                             {"@StartLocalDateTime", context.ActionDateTime},
                             {"@FacilityItemKey", facilityKitItem.FacilityItemKey},
                             {"@ItemMemberQuantity", facilityKitItem.Quantity},
                             {"@LastModifiedActorKey", (Guid?)context.Actor},
                             {"@LastModifiedBinaryValue", facilityKitItem.LastModified},
                             {"@FacilityKitMemberKey", facilityKitItem.Key}
                         });

                connectionScope.Execute(
                    "Item.usp_FacilityKitMemberUpdate",
                    parameters,
                    null,
                    connectionScope.DefaultCommandTimeout,
                    CommandType.StoredProcedure);
            }

            // Add the facility kits.
            InsertFacilityKitItems(
                connectionScope,
                context,
                facilityKitKey,
                addedFacilityKitItems);
        }

        #endregion
    }
}
