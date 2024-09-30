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
    #region IRepeatPatternRepository Interface

    public interface IRepeatPatternRepository
    {
        /// <summary>
        /// Retrieves a collection of <see cref="RepeatPattern"/> by criteria.
        /// </summary>
        /// <param name="findCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>An IPagedCollection(T) object, where the generic parameter T is <see cref="RepeatPattern"/>.</returns>
        IPagedCollection<RepeatPattern> GetRepeatPatterns(FindCriteria findCriteria, int page = 1, int pageSize = Int32.MaxValue);

        /// <summary>
        /// Retrieves a collection of <see cref="RepeatPattern"/> by external system.
        /// </summary>
        /// <param name="externalSystemKey"></param>
        /// <returns></returns>
        IReadOnlyCollection<RepeatPattern> GetRepeatPatterns(Guid externalSystemKey); 
        
        /// <summary>
        /// Retreive the repeat pattern.
        /// </summary>
        /// <returns>The <see cref="RepeatPattern"/> instance; otherwise null if not found.</returns>
        RepeatPattern GetRepeatPattern(Guid repeatPatternKey);

        /// <summary>
        /// Retreive the repeat pattern.
        /// </summary>
        /// <returns>The <see cref="RepeatPattern"/> instance; otherwise null if not found.</returns>
        RepeatPattern GetRepeatPattern(Guid externalSystemKey, string code);

        /// <summary>
        /// Checks to see if there is an existing repeat pattern code within the external system.
        /// </summary>
        /// <param name="externalSystemKey"></param>
        /// <param name="code"></param>
        /// <param name="excludeRepeatPatternFilter"></param>
        /// <returns></returns>
        bool RepeatPatternCodeExists(Guid externalSystemKey, string code, Filter<Guid> excludeRepeatPatternFilter = default(Filter<Guid>));

        /// <summary>
        /// Gets all associated repeat patterns.
        /// </summary>
        /// <returns></returns>
        ISet<Guid> GetAssociatedRepeatPatternKeys();
        
        /// <summary>
        /// Persists the <see cref="RepeatPattern"/> to the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="repeatPattern">The repeat pattern to save.</param>
        /// <returns>
        /// The repeat pattern surrogate key, which uniquely identifies the repeat pattern in the database.
        /// </returns>
        Guid InsertRepeatPattern(Context context, RepeatPattern repeatPattern);

        /// <summary>
        /// Updates an existing <see cref="RepeatPattern"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="repeatPattern">The repeat pattern to update.</param>
        void UpdateRepeatPattern(Context context, RepeatPattern repeatPattern);

        /// <summary>
        /// Deletes an existing <see cref="RepeatPattern"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="repeatPatternKey">The repeat pattern to delete.</param>
        void DeleteRepeatPattern(Context context, Guid repeatPatternKey);
    }

    #endregion

    public class RepeatPatternRepository : RepositoryBase, IRepeatPatternRepository
    {
        static RepeatPatternRepository()
        {
            // Dapper custom mappings
            SqlMapper.SetTypeMap(
                typeof(RepeatPattern),
                new ColumnAttributeTypeMapper<RepeatPattern>());

            SqlMapper.SetTypeMap(
                typeof(LocationRepeatPattern),
                new ColumnAttributeTypeMapper<LocationRepeatPattern>());

            SqlMapper.SetTypeMap(
                typeof(LocationRepeatPatternTime),
                new ColumnAttributeTypeMapper<LocationRepeatPatternTime>());
        }

        IPagedCollection<RepeatPattern> IRepeatPatternRepository.GetRepeatPatterns(FindCriteria findCriteria, int page, int pageSize)
        {
            Guard.ArgumentNotNull(findCriteria, "findCriteria");

            // Make sure paging is within bounds.
            if (page < 1) page = 1;
            if (pageSize < 0) pageSize = 0;

            IPagedCollection<RepeatPattern> repeatPatterns = PagedCollection<RepeatPattern>.EmptyResults;

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable(findCriteria.SelectedKeys);
                GuidKeyTable excludedKeys = new GuidKeyTable(findCriteria.ExcludedKeys);
                FilterCriteriaTable filterCriteria = new FilterCriteriaTable(findCriteria.Filters);

                var parameters = new DynamicParameters(
                    new Dictionary<string, object>
                        {
                            {"@PageNumber", page},
                            {"@RowsPerPage", pageSize},
                            {"@SelectedKeys", selectedKeys.AsTableValuedParameter()},
                            {"@ExcludedKeys", excludedKeys.AsTableValuedParameter()},
                            {"@SearchText", findCriteria.SearchText},
                            {"@OrderByField", findCriteria.OrderBy},
                            {"@AscendingFlag", findCriteria.Ascending},
                            {"@FilterCriteria", filterCriteria.AsTableValuedParameter()}
                        });
                parameters.Add("@TotalRows", dbType: DbType.Int64, direction: ParameterDirection.Output);

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    using (var multi = connectionScope.QueryMultiple(
                        "HcOrder.bsp_GetRepeatPatterns",
                        parameters,
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure))
                    {
                        var results = multi.Read<RepeatPattern>().ToList();
                        var results2 = multi.Read<LocationRepeatPattern>()
                            .ToLookup(lrp => lrp.RepeatPatternKey);
                        var results3 = multi.Read<LocationRepeatPatternTime>()
                            .ToLookup(lrpt => lrpt.LocationRepeatPatternKey);
                        var totalRows = parameters.Get<long?>("@TotalRows");

                        foreach (var repeatPattern in results)
                        {
                            if (results2.Contains(repeatPattern.Key))
                            {
                                var locationRepeatPatterns = results2[repeatPattern.Key].ToArray();
                                foreach (var locationRepeatPattern in locationRepeatPatterns)
                                {
                                    if (results3.Contains(locationRepeatPattern.Key))
                                    {
                                        locationRepeatPattern.LocationRepeatPatternTimes = results3[locationRepeatPattern.Key].ToArray();
                                    }
                                }

                                repeatPattern.ExplicitTimes = locationRepeatPatterns;    
                            }
                        }

                        repeatPatterns = new PagedCollection<RepeatPattern>(results, totalRows ?? 0);
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return repeatPatterns;
        }

        IReadOnlyCollection<RepeatPattern> IRepeatPatternRepository.GetRepeatPatterns(Guid externalSystemKey)
        {
            FindCriteria findCriteria = new FindCriteria();
            findCriteria.Filters.Add(new FindFilter("ExternalSystem", "Equals", externalSystemKey));

            IPagedCollection<RepeatPattern> repeatPatterns =
                ((IRepeatPatternRepository)this).GetRepeatPatterns(findCriteria);

            return repeatPatterns;
        }

        RepeatPattern IRepeatPatternRepository.GetRepeatPattern(Guid repeatPatternKey)
        {
            FindCriteria findCriteria = new FindCriteria();
            findCriteria.SelectedKeys.Add(repeatPatternKey);

            IPagedCollection<RepeatPattern> repeatPatterns =
                ((IRepeatPatternRepository)this).GetRepeatPatterns(findCriteria, 1, 1);

            return repeatPatterns.FirstOrDefault();
        }

        RepeatPattern IRepeatPatternRepository.GetRepeatPattern(Guid externalSystemKey, string code)
        {
            FindCriteria findCriteria = new FindCriteria();
            findCriteria.Filters.Add(new FindFilter("ExternalSystem", "Equals", externalSystemKey));
            findCriteria.Filters.Add(new FindFilter("RepeatPatternCode", "Equals", code));

            IPagedCollection<RepeatPattern> repeatPatterns =
                ((IRepeatPatternRepository)this).GetRepeatPatterns(findCriteria, 1, 1);

            return repeatPatterns.FirstOrDefault();
        }

        bool IRepeatPatternRepository.RepeatPatternCodeExists(Guid externalSystemKey, string code, Filter<Guid> excludeRepeatPatternFilter)
        {
            bool exists = false;

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    SqlBuilder query = new SqlBuilder();
                    query.SELECT()
                        ._("RepeatPatternKey")
                        .FROM("HcOrder.RepeatPattern")
                        .WHERE("DeleteFlag = 0")
                        .WHERE("ExternalSystemKey = @ExternalSystemKey")
                        .WHERE("RepeatPatternCode = @RepeatPatternCode");

                    if (excludeRepeatPatternFilter.HasValue)
                    {
                        query.WHERE("RepeatPatternKey <> @ExcludeRepeatPatternKey");
                    }

                    exists = connectionScope.Query(
                        query.ToString(),
                        new
                        {
                            ExternalSystemKey = externalSystemKey,
                            RepeatPatternCode = code,
                            ExcludeRepeatPatternKey = excludeRepeatPatternFilter.GetValueOrDefault()
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

        ISet<Guid> IRepeatPatternRepository.GetAssociatedRepeatPatternKeys()
        {
            HashSet<Guid> associatedKeys = new HashSet<Guid>();

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    SqlBuilder query = new SqlBuilder();
                    query.SELECT()
                        ._("DISTINCT porp.RepeatPatternKey")
                        .FROM("HcOrder.PharmacyOrderSnapshot pos")
                        .INNER_JOIN("HcOrder.PharmacyOrderTimingRecordSet potrs ON potrs.PharmacyOrderKey = pos.PharmacyOrderKey")
                        .INNER_JOIN("HcOrder.PharmacyOrderTimingRecord potr ON potr.PharmacyOrderTimingRecordSetKey = potrs.PharmacyOrderTimingRecordSetKey")
                        .INNER_JOIN("HcOrder.PharmacyOrderRepeatPattern porp ON porp.PharmacyOrderTimingRecordKey = potr.PharmacyOrderTimingRecordKey")
                        .WHERE("pos.EndUTCDateTime IS NULL")
                        .WHERE("potrs.EndUTCDateTime IS NULL");

                    associatedKeys = connectionScope.Query(
                        query.ToString(),
                        null,
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Select(x => (Guid)x.RepeatPatternKey)
                        .ToHashSet();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return associatedKeys;
        }

        Guid IRepeatPatternRepository.InsertRepeatPattern(Context context, RepeatPattern repeatPattern)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (repeatPattern == null) throw new ArgumentNullException("repeatPattern");

            Guid? repeatPatternKey = null;

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@ExternalSystemKey", repeatPattern.ExternalSystemKey},
                             {"@RepeatPatternCode", repeatPattern.Code},
                             {"@DescriptionText", repeatPattern.Description},
                             {"@SortValue", repeatPattern.SortOrder},
                             {"@StandardRepeatPatternInternalCode", repeatPattern.StandardRepeatPatternInternalCode},
                             {"@RepeatPatternPeriodAmount", repeatPattern.PeriodAmount},
                             {"@MondayFlag", repeatPattern.Monday},
                             {"@TuesdayFlag", repeatPattern.Tuesday},
                             {"@WednesdayFlag", repeatPattern.Wednesday},
                             {"@ThursdayFlag", repeatPattern.Thursday},
                             {"@FridayFlag", repeatPattern.Friday},
                             {"@SaturdayFlag", repeatPattern.Saturday},
                             {"@SundayFlag", repeatPattern.Sunday},
                             {"@StatFlag", repeatPattern.Stat},
                             {"@CreatedUTCDateTime", context.ActionUtcDateTime},
                             {"@CreatedLocalDateTime", context.ActionDateTime},
                             {"@LastModifiedActorKey", (Guid?)context.Actor}
                         });
                    parameters.Add("@RepeatPatternKey", dbType: DbType.Guid, direction: ParameterDirection.Output);

                    connectionScope.Execute(
                        "HcOrder.usp_RepeatPatternInsert",
                        parameters,
                        null,
                        connectionScope.DefaultCommandTimeout,
                        CommandType.StoredProcedure);

                    repeatPatternKey = parameters.Get<Guid?>("@RepeatPatternKey");

                    if (repeatPatternKey.HasValue)
                    {
                        if (repeatPattern.ExplicitTimes != null)
                        {
                            InsertLocationRepeatPatterns(
                                connectionScope,
                                context,
                                repeatPatternKey.Value,
                                repeatPattern.ExplicitTimes);
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

            return repeatPatternKey.GetValueOrDefault();
        }

        void IRepeatPatternRepository.UpdateRepeatPattern(Context context, RepeatPattern repeatPattern)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (repeatPattern == null) throw new ArgumentNullException("repeatPattern");

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@RepeatPatternCode", repeatPattern.Code},
                             {"@DescriptionText", repeatPattern.Description},
                             {"@SortValue", repeatPattern.SortOrder},
                             {"@StandardRepeatPatternInternalCode", repeatPattern.StandardRepeatPatternInternalCode},
                             {"@RepeatPatternPeriodAmount", repeatPattern.PeriodAmount},
                             {"@MondayFlag", repeatPattern.Monday},
                             {"@TuesdayFlag", repeatPattern.Tuesday},
                             {"@WednesdayFlag", repeatPattern.Wednesday},
                             {"@ThursdayFlag", repeatPattern.Thursday},
                             {"@FridayFlag", repeatPattern.Friday},
                             {"@SaturdayFlag", repeatPattern.Saturday},
                             {"@SundayFlag", repeatPattern.Sunday},
                             {"@StatFlag", repeatPattern.Stat},
                             {"@EndUTCDateTime", context.ActionUtcDateTime},
                             {"@EndLocalDateTime", context.ActionDateTime},
                             {"@LastModifiedActorKey", (Guid?)context.Actor},
                             {"@LastModifiedBinaryValue", repeatPattern.LastModified},
                             {"@RepeatPatternKey", repeatPattern.Key}
                         });

                    connectionScope.Execute(
                        "HcOrder.usp_RepeatPatternUpdate",
                        parameters,
                        null,
                        connectionScope.DefaultCommandTimeout,
                        CommandType.StoredProcedure);

                    // If LocationRepeatPatterns is null then we assume all associations were removed.
                    UpdateLocationRepeatPatterns(
                        connectionScope,
                        context,
                        repeatPattern.Key,
                        repeatPattern.ExplicitTimes ?? new LocationRepeatPattern[0]);

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IRepeatPatternRepository.DeleteRepeatPattern(Context context, Guid repeatPatternKey)
        {
            if (context == null) throw new ArgumentNullException("context");

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@EndUTCDateTime", context.ActionUtcDateTime},
                             {"@EndLocalDateTime", context.ActionDateTime},
                             {"@LastModifiedActorKey", (Guid?)context.Actor},
                             {"@RepeatPatternKey", repeatPatternKey}
                         });

                    connectionScope.Execute(
                        "HcOrder.usp_RepeatPatternDelete",
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

        private static void InsertLocationRepeatPatterns(IConnectionScope connectionScope, Context context, Guid repeatPatternKey, IEnumerable<LocationRepeatPattern> locationRepeatPatterns)
        {
            Guard.ArgumentNotNull(connectionScope, "connectionScope");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(locationRepeatPatterns, "locationRepeatPatterns");

            using (ITransactionScope tx = TransactionScopeFactory.Create())
            {
                foreach (LocationRepeatPattern locationRepeatPattern in locationRepeatPatterns)
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@FacilityKey", locationRepeatPattern.FacilityKey},
                             {"@RepeatPatternKey", repeatPatternKey},
                             {"@AssociationUTCDateTime", context.ActionUtcDateTime},
                             {"@AssociationLocalDateTime", context.ActionDateTime},
                             {"@AssociationActorKey", (Guid?)context.Actor}
                         });
                    parameters.Add("@LocationRepeatPatternKey", dbType: DbType.Guid, direction: ParameterDirection.Output);

                    connectionScope.Execute(
                        "HcOrder.usp_LocationRepeatPatternInsert",
                        parameters,
                        null,
                        connectionScope.DefaultCommandTimeout,
                        CommandType.StoredProcedure);

                    var locationRepeatPatternKey = parameters.Get<Guid?>("@LocationRepeatPatternKey");

                    if (locationRepeatPatternKey.HasValue &&
                        locationRepeatPattern.LocationRepeatPatternTimes != null)
                    {
                        InsertLocationRepeatPatternTimes(
                            connectionScope,
                            context,
                            locationRepeatPatternKey.Value,
                            locationRepeatPattern.LocationRepeatPatternTimes);
                    }
                }

                tx.Complete();
            }
        }

        private static void UpdateLocationRepeatPatterns(IConnectionScope connectionScope, Context context, Guid repeatPatternKey, IEnumerable<LocationRepeatPattern> locationRepeatPatterns)
        {
            Guard.ArgumentNotNull(connectionScope, "connectionScope");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(locationRepeatPatterns, "locationRepeatPatterns");

            SqlBuilder query = new SqlBuilder();
            query.SELECT()
                ._("LocationRepeatPatternKey")
                ._("FacilityKey")
                .FROM("HcOrder.LocationRepeatPattern")
                .WHERE("DisassociationUTCDateTime IS NULL")
                .WHERE("RepeatPatternKey = @RepeatPatternKey");

            // Get the list of location repeat patterns associated with this repeat pattern.
            IReadOnlyCollection<Tuple<Guid,Guid>> currentLocationRepeatPatterns = connectionScope.Query(
                        query.ToString(),
                        new { RepeatPatternKey = repeatPatternKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Select(x => new Tuple<Guid,Guid>((Guid)x.LocationRepeatPatternKey, (Guid)x.FacilityKey))
                        .ToList();

            // Find the location repeat patterns that were removed.
            var removedLocationRepeatPatterns = currentLocationRepeatPatterns.Except(locationRepeatPatterns.Select(lrp => 
                new Tuple<Guid, Guid>(lrp.Key, lrp.FacilityKey))).ToList();

            using (ITransactionScope tx = TransactionScopeFactory.Create())
            {
                // Remove facility keys that are no longer associated with this repeat pattern.
                foreach (var locationRepeatPattern in removedLocationRepeatPatterns)
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@FacilityKey", locationRepeatPattern.Item2}, // FacilityKey
                             {"@RepeatPatternKey", repeatPatternKey},
                             {"@DisassociationUTCDateTime", context.ActionUtcDateTime},
                             {"@DisassociationLocalDateTime", context.ActionDateTime},
                             {"@DisassociationActorKey", (Guid?)context.Actor}
                         });

                    connectionScope.Execute(
                        "HcOrder.usp_LocationRepeatPatternDelete",
                        parameters,
                        null,
                        connectionScope.DefaultCommandTimeout,
                        CommandType.StoredProcedure);
                }

                // Find the location repeat patterns that were added.
                List<LocationRepeatPattern> addedLocationRepeatPatterns = new List<LocationRepeatPattern>();
                foreach (LocationRepeatPattern locationRepeatPattern in locationRepeatPatterns)
                {
                    if (locationRepeatPattern.IsTransient())
                    {
                        addedLocationRepeatPatterns.Add(locationRepeatPattern);
                        continue;
                    }

                    UpdateLocationRepeatPatternTimes(
                        connectionScope,
                        context,
                        locationRepeatPattern.Key,
                        locationRepeatPattern.LocationRepeatPatternTimes ?? new LocationRepeatPatternTime[0]);
                }

                // Add the new location repeat patterns.
                InsertLocationRepeatPatterns(
                    connectionScope,
                    context,
                    repeatPatternKey,
                    addedLocationRepeatPatterns);

                tx.Complete();
            }
        }

        private static void InsertLocationRepeatPatternTimes(IConnectionScope connectionScope, Context context, Guid locationRepeatPatternKey, IEnumerable<LocationRepeatPatternTime> locationRepeatPatternTimes)
        {
            Guard.ArgumentNotNull(connectionScope, "connectionScope");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(locationRepeatPatternTimes, "locationRepeatPatternTimes");

            using (ITransactionScope tx = TransactionScopeFactory.Create())
            {
                foreach (LocationRepeatPatternTime locationRepeatPatternTime in locationRepeatPatternTimes)
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@LocationRepeatPatternKey", locationRepeatPatternKey},
                             {"@StartUTCDateTime", context.ActionUtcDateTime},
                             {"@StartLocalDateTime", context.ActionDateTime},
                             {"@ExplicitTimeOfDayValue", locationRepeatPatternTime.ExplicitTimeOfDay},
                             {"@LastModifiedActorKey", (Guid?)context.Actor}
                         });
                    parameters.Add("@LocationRepeatPatternTimeKey", dbType: DbType.Guid, direction: ParameterDirection.Output);

                    connectionScope.Execute(
                        "HcOrder.usp_LocationRepeatPatternTimeInsert",
                        parameters,
                        null,
                        connectionScope.DefaultCommandTimeout,
                        CommandType.StoredProcedure);
                }

                tx.Complete();
            }
        }

        private static void UpdateLocationRepeatPatternTimes(IConnectionScope connectionScope, Context context, Guid locationRepeatPatternKey, IEnumerable<LocationRepeatPatternTime> locationRepeatPatternTimes)
        {
            Guard.ArgumentNotNull(connectionScope, "connectionScope");
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(locationRepeatPatternTimes, "locationRepeatPatternTimes");

            SqlBuilder query = new SqlBuilder();
            query.SELECT("LocationRepeatPatternTimeKey")
                .FROM("HcOrder.LocationRepeatPatternTime")
                .WHERE("EndUTCDateTime IS NULL")
                .WHERE("LocationRepeatPatternKey = @LocationRepeatPatternKey");

            // Get the list of location repeat pattern times associated with this location repeat pattern.
            IReadOnlyCollection<Guid> currentLocationRepeatPatternTimeKeys = connectionScope.Query(
                        query.ToString(),
                        new { LocationRepeatPatternKey = locationRepeatPatternKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Select(x => (Guid)x.LocationRepeatPatternTimeKey)
                        .ToList();

            // Find the location repeat pattern time keys that were removed.
            var removedLocationRepeatPatternTimeKeys = currentLocationRepeatPatternTimeKeys.Except(locationRepeatPatternTimes.Select(lrpt => lrpt.Key));

            using (ITransactionScope tx = TransactionScopeFactory.Create())
            {
                // Remove location repeat pattern time keys that are no longer associated with this location repeat pattern.
                foreach (var locationRepeatPatternTimeKey in removedLocationRepeatPatternTimeKeys)
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@StartUTCDateTime", context.ActionUtcDateTime},
                             {"@StartLocalDateTime", context.ActionDateTime},
                             {"@LastModifiedActorKey", (Guid?)context.Actor},
                             {"@LocationRepeatPatternTimeKey", locationRepeatPatternTimeKey}
                         });

                    connectionScope.Execute(
                        "HcOrder.usp_LocationRepeatPatternTimeDelete",
                        parameters,
                        null,
                        connectionScope.DefaultCommandTimeout,
                        CommandType.StoredProcedure);
                }

                List<LocationRepeatPatternTime> addedLocationRepeatPatternTimes = new List<LocationRepeatPatternTime>();
                foreach (LocationRepeatPatternTime locationRepeatPatternTime in locationRepeatPatternTimes)
                {
                    if (locationRepeatPatternTime.IsTransient())
                    {
                        addedLocationRepeatPatternTimes.Add(locationRepeatPatternTime);
                        continue;
                    }

                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@StartUTCDateTime", context.ActionUtcDateTime},
                             {"@StartLocalDateTime", context.ActionDateTime},
                             {"@ExplicitTimeOfDayValue", locationRepeatPatternTime.ExplicitTimeOfDay},
                             {"@LastModifiedActorKey", (Guid?)context.Actor},
                             {"@LastModifiedBinaryValue", locationRepeatPatternTime.LastModified},
                             {"@LocationRepeatPatternTimeKey", locationRepeatPatternTime.Key}
                         });

                    connectionScope.Execute(
                        "HcOrder.usp_LocationRepeatPatternTimeUpdate",
                        parameters,
                        null,
                        connectionScope.DefaultCommandTimeout,
                        CommandType.StoredProcedure);
                }

                InsertLocationRepeatPatternTimes(
                    connectionScope,
                    context,
                    locationRepeatPatternKey,
                    addedLocationRepeatPatternTimes);

                tx.Complete();
            }
        }

        #endregion
    }
}
