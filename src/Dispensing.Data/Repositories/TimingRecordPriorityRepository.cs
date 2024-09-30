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
    #region ITimingRecordPriorityRepository Interface

    public interface ITimingRecordPriorityRepository
    {
        /// <summary>
        /// Retrieves a collection of <see cref="TimingRecordPriorityRepository"/> by key.
        /// </summary>
        /// <param name="findCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="TimingRecordPriorityRepository"/>.</returns>
        IPagedCollection<TimingRecordPriority> GetTimingRecordPriorities(FindCriteria findCriteria, int page = 1, int pageSize = Int32.MaxValue);

        /// <summary>
        /// Retreive the timing record priority record.
        /// </summary>
        /// <returns>The <see cref="TimingRecordPriority"/> instance; otherwise null if not found.</returns>
        TimingRecordPriority GetTimingRecordPriority(Guid timingRecordPriorityKey);

        /// <summary>
        /// Retreive the timing record priority record.
        /// </summary>
        /// <returns>The <see cref="TimingRecordPriority"/> instance; otherwise null if not found.</returns>
        TimingRecordPriority GetTimingRecordPriority(Guid externalSystemKey, string code);

        /// <summary>
        /// Checks to see if there is an existing timing record priority code within the external system.
        /// </summary>
        /// <param name="externalSystemKey"></param>
        /// <param name="code"></param>
        /// <param name="excludeTimingRecordPriorityFilter"></param>
        /// <returns></returns>
        bool TimingRecordPriorityCodeExists(Guid externalSystemKey, string code, Filter<Guid> excludeTimingRecordPriorityFilter = default(Filter<Guid>));

        /// <summary>
        /// Gets all associated timing record priorities.
        /// </summary>
        /// <returns></returns>
        ISet<Guid> GetAssociatedTimingRecordPriorityKeys();
        
        /// <summary>
        /// Persists the <see cref="TimingRecordPriority"/> to the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="timingRecordPriority">The timing record priority to save.</param>
        /// <returns>
        /// The timing record priority surrogate key, which uniquely identifies the timing record priority in the database.
        /// </returns>
        Guid InsertTimingRecordPriority(Context context, TimingRecordPriority timingRecordPriority);

        /// <summary>
        /// Updates an existing <see cref="TimingRecordPriority"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="timingRecordPriority">The timing record priority to update.</param>
        void UpdateTimingRecordPriority(Context context, TimingRecordPriority timingRecordPriority);

        /// <summary>
        /// Deletes an existing <see cref="TimingRecordPriority"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="timingRecordPriorityKey">The timing record priority to delete.</param>
        void DeleteTimingRecordPriority(Context context, Guid timingRecordPriorityKey);
    }

    #endregion

    public class TimingRecordPriorityRepository : RepositoryBase, ITimingRecordPriorityRepository
    {
        static TimingRecordPriorityRepository()
        {
            // Dapper custom mappings
            SqlMapper.SetTypeMap(
                typeof(TimingRecordPriority),
                new ColumnAttributeTypeMapper<TimingRecordPriority>());
        }

        IPagedCollection<TimingRecordPriority> ITimingRecordPriorityRepository.GetTimingRecordPriorities(FindCriteria findCriteria, int page, int pageSize)
        {
            Guard.ArgumentNotNull(findCriteria, "findCriteria");

            // Make sure paging is within bounds.
            if (page < 1) page = 1;
            if (pageSize < 0) pageSize = 0;

            IPagedCollection<TimingRecordPriority> timingRecordPriorities = PagedCollection<TimingRecordPriority>.EmptyResults;

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
                    var results = connectionScope.Query<TimingRecordPriority>(
                        "HcOrder.bsp_GetTimingRecordPriorities",
                        parameters,
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure)
                        .ToList();
                       
                    var totalRows = parameters.Get<long?>("@TotalRows");

                    timingRecordPriorities = new PagedCollection<TimingRecordPriority>(results, totalRows ?? 0);
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return timingRecordPriorities;
        }

        TimingRecordPriority ITimingRecordPriorityRepository.GetTimingRecordPriority(Guid timingRecordPriorityKey)
        {
            FindCriteria findCriteria = new FindCriteria();
            findCriteria.SelectedKeys.Add(timingRecordPriorityKey);

            IPagedCollection<TimingRecordPriority> timingRecordPriorities =
                ((ITimingRecordPriorityRepository)this).GetTimingRecordPriorities(findCriteria, 1, 1);

            return timingRecordPriorities.FirstOrDefault();
        }

        TimingRecordPriority ITimingRecordPriorityRepository.GetTimingRecordPriority(Guid externalSystemKey, string code)
        {
            FindCriteria findCriteria = new FindCriteria();
            findCriteria.Filters.Add(new FindFilter("ExternalSystem", "Equals", externalSystemKey));
            findCriteria.Filters.Add(new FindFilter("TimingRecordPriorityCode", "Equals", code));

            IPagedCollection<TimingRecordPriority> timingRecordPriorities =
                ((ITimingRecordPriorityRepository)this).GetTimingRecordPriorities(findCriteria, 1, 1);

            return timingRecordPriorities.FirstOrDefault();
        }

        bool ITimingRecordPriorityRepository.TimingRecordPriorityCodeExists(Guid externalSystemKey, string code, Filter<Guid> excludeTimingRecordPriorityFilter)
        {
            bool exists = false;

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    SqlBuilder query = new SqlBuilder();
                    query.SELECT()
                        ._("TimingRecordPriorityKey")
                        .FROM("HcOrder.TimingRecordPriority")
                        .WHERE("DeleteFlag = 0")
                        .WHERE("ExternalSystemKey = @ExternalSystemKey")
                        .WHERE("TimingRecordPriorityCode = @TimingRecordPriorityCode");

                    if (excludeTimingRecordPriorityFilter.HasValue)
                    {
                        query.WHERE("TimingRecordPriorityKey <> @ExcludeTimingRecordPriorityKey");
                    }

                    exists = connectionScope.Query(
                        query.ToString(),
                        new
                        {
                            ExternalSystemKey = externalSystemKey,
                            TimingRecordPriorityCode = code,
                            ExcludeTimingRecordPriorityKey = excludeTimingRecordPriorityFilter.GetValueOrDefault()
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

        ISet<Guid> ITimingRecordPriorityRepository.GetAssociatedTimingRecordPriorityKeys()
        {
            HashSet<Guid> associatedKeys = new HashSet<Guid>();

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    SqlBuilder query = new SqlBuilder();
                    query.SELECT()
                        ._("DISTINCT pop.TimingRecordPriorityKey")
                        .FROM("HcOrder.PharmacyOrderSnapshot pos")
                        .INNER_JOIN("HcOrder.PharmacyOrderTimingRecordSet potrs ON potrs.PharmacyOrderKey = pos.PharmacyOrderKey")
                        .INNER_JOIN("HcOrder.PharmacyOrderTimingRecord potr ON potr.PharmacyOrderTimingRecordSetKey = potrs.PharmacyOrderTimingRecordSetKey")
                        .INNER_JOIN("HcOrder.PharmacyOrderPriority pop ON pop.PharmacyOrderTimingRecordKey = potr.PharmacyOrderTimingRecordKey")
                        .WHERE("pos.EndUTCDateTime IS NULL")
                        .WHERE("potrs.EndUTCDateTime IS NULL");

                    associatedKeys = connectionScope.Query(
                        query.ToString(),
                        null,
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Select(x => (Guid)x.TimingRecordPriorityKey)
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

        Guid ITimingRecordPriorityRepository.InsertTimingRecordPriority(Context context, TimingRecordPriority timingRecordPriority)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (timingRecordPriority == null) throw new ArgumentNullException("timingRecordPriority");

            Guid? timingRecordPriorityKey = null;

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@ExternalSystemKey", timingRecordPriority.ExternalSystemKey},
                             {"@TimingRecordPriorityCode", timingRecordPriority.Code},
                             {"@DescriptionText", timingRecordPriority.Description},
                             {"@SortValue", timingRecordPriority.SortOrder},
                             {"@StandardTimingRecordPriorityInternalCode", timingRecordPriority.StandardTimingRecordPriorityInternalCode},
                             {"@CreatedUTCDateTime", context.ActionUtcDateTime},
                             {"@CreatedLocalDateTime", context.ActionDateTime},
                             {"@LastModifiedActorKey", (Guid?)context.Actor}
                         });
                    parameters.Add("@TimingRecordPriorityKey", dbType: DbType.Guid, direction: ParameterDirection.Output);

                    connectionScope.Execute(
                        "HcOrder.usp_TimingRecordPriorityInsert",
                        parameters,
                        null,
                        connectionScope.DefaultCommandTimeout,
                        CommandType.StoredProcedure);

                    timingRecordPriorityKey = parameters.Get<Guid?>("@TimingRecordPriorityKey");
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return timingRecordPriorityKey.GetValueOrDefault();
        }

        void ITimingRecordPriorityRepository.UpdateTimingRecordPriority(Context context, TimingRecordPriority timingRecordPriority)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (timingRecordPriority == null) throw new ArgumentNullException("timingRecordPriority");

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@TimingRecordPriorityCode", timingRecordPriority.Code},
                             {"@DescriptionText", timingRecordPriority.Description},
                             {"@SortValue", timingRecordPriority.SortOrder},
                             {"@StandardTimingRecordPriorityInternalCode", timingRecordPriority.StandardTimingRecordPriorityInternalCode},
                             {"@EndUTCDateTime", context.ActionUtcDateTime},
                             {"@EndLocalDateTime", context.ActionDateTime},
                             {"@LastModifiedActorKey", (Guid?)context.Actor},
                             {"@LastModifiedBinaryValue", timingRecordPriority.LastModified},
                             {"@TimingRecordPriorityKey", timingRecordPriority.Key}
                         });

                    connectionScope.Execute(
                        "HcOrder.usp_TimingRecordPriorityUpdate",
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

        void ITimingRecordPriorityRepository.DeleteTimingRecordPriority(Context context, Guid timingRecordPriorityKey)
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
                             {"@TimingRecordPriorityKey", timingRecordPriorityKey}
                         });

                    connectionScope.Execute(
                        "HcOrder.usp_TimingRecordPriorityDelete",
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
    }
}
