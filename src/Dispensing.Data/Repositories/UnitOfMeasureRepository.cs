using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Models;
using Dapper;
using Pyxis.Core.Data;
using Pyxis.Core.Data.InternalCodes;
using Pyxis.Core.Data.Schema;
using Pyxis.Core.Data.Schema.Core.Models;
using Pyxis.Core.Data.Schema.TableTypes;

namespace CareFusion.Dispensing.Data.Repositories
{
    #region IUnitOfMeasure Interface

    public interface IUnitOfMeasureRepository
    {
        /// <summary>
        /// Retrieves a collection of <see cref="UnitOfMeasure"/> by key.
        /// </summary>
        /// <param name="findCriteria"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="UnitOfMeasure"/>.</returns>
        IPagedCollection<UnitOfMeasure> GetUnitOfMeasures(FindCriteria findCriteria, int page = 1, int pageSize = Int32.MaxValue);

        /// <summary>
        /// Retreive the unit of measure record.
        /// </summary>
        /// <returns>The <see cref="UnitOfMeasure"/> instance; otherwise null if not found.</returns>
        UnitOfMeasure GetUnitOfMeasure(Guid unitOfMeasureKey);

        /// <summary>
        /// Retreive the unit of measure record.
        /// </summary>
        /// <returns>The <see cref="UnitOfMeasure"/> instance; otherwise null if not found.</returns>
        UnitOfMeasure GetUnitOfMeasure(string displayCode);

        /// <summary>
        /// Checks to see if there is an existing unit of measure display code within the system.
        /// </summary>
        /// <param name="displayCode"></param>
        /// <param name="excludeUnitOfMeasureFilter"></param>
        /// <returns></returns>
        bool UnitOfMeasureDisplayCodeExists(string displayCode, Filter<Guid> excludeUnitOfMeasureFilter = default(Filter<Guid>));

        /// <summary>
        /// Checks to see if the specified unit of measure is used as a base.
        /// </summary>
        /// <param name="unitOfMeasureKey"></param>
        /// <returns></returns>
        bool UnitOfMeasureIsUsedAsBase(Guid unitOfMeasureKey);

        /// <summary>
        /// Checks to see if the specified unit of measure has a base assigned to it.
        /// </summary>
        /// <param name="unitOfMeasureKey"></param>
        /// <returns></returns>
        bool UnitOfMeasureHasBase(Guid unitOfMeasureKey);

        /// <summary>
        /// Returns the external unit of measure codes that are assigned a role that is not part of the specified
        /// roles.
        /// </summary>
        /// <param name="unitOfMeasureKey"></param>
        /// <param name="roleInternalCodes"></param>
        /// <returns></returns>
        IReadOnlyCollection<string> GetAffectedExternalUomCodes(Guid unitOfMeasureKey,
            IEnumerable<string> roleInternalCodes);
            
        /// <summary>
        /// Persists the <see cref="UnitOfMeasure"/> to the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="unitOfMeasure">The unit of measure to save.</param>
        /// <returns>
        /// The unit of measure surrogate key, which uniquely identifies the unit of measure in the database.
        /// </returns>
        Guid InsertUnitOfMeasure(Context context, UnitOfMeasure unitOfMeasure);

        /// <summary>
        /// Updates an existing <see cref="UnitOfMeasure"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="unitOfMeasure">The unit of measure to update.</param>
        void UpdateUnitOfMeasure(Context context, UnitOfMeasure unitOfMeasure);
    }

    #endregion

    public class UnitOfMeasureRepository : RepositoryBase, IUnitOfMeasureRepository
    {
        static UnitOfMeasureRepository()
        {
            // Dapper custom mappings
            SqlMapper.SetTypeMap(
                typeof(UnitOfMeasure),
                new ColumnAttributeTypeMapper<UnitOfMeasure>());
        }

        IPagedCollection<UnitOfMeasure> IUnitOfMeasureRepository.GetUnitOfMeasures(FindCriteria findCriteria, int page, int pageSize)
        {
            Guard.ArgumentNotNull(findCriteria, "findCriteria");

            // Make sure paging is within bounds.
            if (page < 1) page = 1;
            if (pageSize < 0) pageSize = 0;

            IPagedCollection<UnitOfMeasure> unitOfMeasures = PagedCollection<UnitOfMeasure>.EmptyResults;

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
                    using (var multi = connectionScope.Connection.QueryMultiple(
                        "Core.bsp_GetUnitOfMeasures",
                        parameters,
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure))
                    {
                        var results = multi.Read<UnitOfMeasure>()
                            .ToList();
                        var uomRoles = multi.Read()
                            .ToLookup(x => (Guid)x.UOMKey, x => (string)x.UOMRoleInternalCode);
                        var totalRows = parameters.Get<long?>("@TotalRows");

                        foreach (var result in results)
                        {
                            if (uomRoles.Contains(result.Key))
                            {
                                IEnumerable<string> roles = uomRoles[result.Key];

                                result.Roles = roles.Select(x => x.FromInternalCode<UOMRoleInternalCode>())
                                    .ToArray();
                            }
                        }

                        unitOfMeasures = new PagedCollection<UnitOfMeasure>(results, totalRows ?? 0);
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return unitOfMeasures;
        }

        UnitOfMeasure IUnitOfMeasureRepository.GetUnitOfMeasure(Guid unitOfMeasureKey)
        {
            FindCriteria findCriteria = new FindCriteria();
            findCriteria.SelectedKeys.Add(unitOfMeasureKey);

            IPagedCollection<UnitOfMeasure> unitOfMeasures =
                ((IUnitOfMeasureRepository)this).GetUnitOfMeasures(findCriteria, 1, 1);

            return unitOfMeasures.FirstOrDefault();
        }

        UnitOfMeasure IUnitOfMeasureRepository.GetUnitOfMeasure(string displayCode)
        {
            FindCriteria findCriteria = new FindCriteria();
            findCriteria.Filters.Add(new FindFilter("DisplayCode", "Equals", displayCode));

            IPagedCollection<UnitOfMeasure> unitOfMeasures =
                ((IUnitOfMeasureRepository)this).GetUnitOfMeasures(findCriteria, 1, 1);

            return unitOfMeasures.FirstOrDefault();
        }

        bool IUnitOfMeasureRepository.UnitOfMeasureDisplayCodeExists(string displayCode, Filter<Guid> excludeUnitOfMeasureFilter)
        {
            bool exists = false;

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    SqlBuilder query = new SqlBuilder();
                    query.SELECT()
                        ._("UOMKey")
                        .FROM("Core.UOM")
                        .WHERE("DisplayCode = @DisplayCode");

                    if (excludeUnitOfMeasureFilter.HasValue)
                    {
                        query.WHERE("UOMKey <> @ExcludeUOMKey");
                    }

                    exists = connectionScope.Query(
                        query.ToString(),
                        new
                        {
                            DisplayCode = displayCode,
                            ExcludeUOMKey = excludeUnitOfMeasureFilter.GetValueOrDefault()
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

        bool IUnitOfMeasureRepository.UnitOfMeasureIsUsedAsBase(Guid unitOfMeasureKey)
        {
            bool exists = false;

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    SqlBuilder query = new SqlBuilder();
                    query.SELECT()
                        ._("UOMKey")
                        .FROM("Core.UOM")
                        .WHERE("BaseUOMKey = @BaseUOMKey");

                    exists = connectionScope.Query(
                        query.ToString(),
                        new
                        {
                            BaseUOMKey = unitOfMeasureKey
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

        bool IUnitOfMeasureRepository.UnitOfMeasureHasBase(Guid unitOfMeasureKey)
        {
            bool exists = false;

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    SqlBuilder query = new SqlBuilder();
                    query.SELECT()
                        ._("UOMKey")
                        .FROM("Core.UOM")
                        .WHERE("UOMKey = @UOMKey")
                        .WHERE("BaseUOMKey IS NOT NULL");

                    exists = connectionScope.Query(
                        query.ToString(),
                        new
                        {
                            UOMKey = unitOfMeasureKey
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

        IReadOnlyCollection<string> IUnitOfMeasureRepository.GetAffectedExternalUomCodes(Guid unitOfMeasureKey, IEnumerable<string> roleInternalCodes)
        {
            if (roleInternalCodes == null) throw new ArgumentNullException("roleInternalCodes");
            var results = new List<string>();

            try
            {
                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var roles = string.Join(",", Array.ConvertAll(roleInternalCodes.ToArray(), input => $"'{input}'"));

                    var query = new SqlBuilder()
                        .SELECT()
                        ._("UOMCode")
                        .FROM("Ext.ExternalUOM")
                        .WHERE("UOMKey = @UOMKey")
                        .WHERE("UOMRoleInternalCode NOT IN ({0})", roles)
                        .WHERE("DeleteFlag = 0");

                    results = connectionScope.Query(
                        query.ToString(),
                        new { UOMKey = unitOfMeasureKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Select(r => (string)r.UOMCode)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return results;
        }

        Guid IUnitOfMeasureRepository.InsertUnitOfMeasure(Context context, UnitOfMeasure unitOfMeasure)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (unitOfMeasure == null) throw new ArgumentNullException("unitOfMeasure");

            Guid? unitOfMeasureKey = null;

            try
            {
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@DisplayCode", unitOfMeasure.DisplayCode},
                             {"@DescriptionText", unitOfMeasure.Description},
                             {"@SortValue", unitOfMeasure.SortOrder},
                             {"@ActiveFlag", unitOfMeasure.IsActive},
                             {"@BaseUOMKey", unitOfMeasure.BaseUnitOfMeasureKey},
                             {"@ConversionAmount", unitOfMeasure.Conversion},
                             {"@CreatedUTCDateTime", context.ActionUtcDateTime},
                             {"@CreatedLocalDateTime", context.ActionDateTime},
                             {"@LastModifiedActorKey", (Guid?)context.Actor}
                         });
                    parameters.Add("@UOMKey", dbType: DbType.Guid, direction: ParameterDirection.Output);

                    connectionScope.Connection.Execute(
                        "Core.usp_UOMInsert",
                        parameters,
                        null,
                        connectionScope.DefaultCommandTimeout,
                        CommandType.StoredProcedure);

                    unitOfMeasureKey = parameters.Get<Guid?>("@UOMKey");

                    if (unitOfMeasureKey != null &&
                        unitOfMeasure.Roles != null)
                    {
                        foreach (var role in unitOfMeasure.Roles)
                        {
                            InsertUnitOfMeasureRole(context, unitOfMeasureKey.Value, role.ToInternalCode());
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

            return unitOfMeasureKey.GetValueOrDefault();
        }

        void IUnitOfMeasureRepository.UpdateUnitOfMeasure(Context context, UnitOfMeasure unitOfMeasure)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (unitOfMeasure == null) throw new ArgumentNullException("unitOfMeasure");

            try
            {
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                         {
                             {"@DisplayCode", unitOfMeasure.DisplayCode},
                             {"@DescriptionText", unitOfMeasure.Description},
                             {"@ActiveFlag", unitOfMeasure.IsActive},
                             {"@BaseUOMKey", unitOfMeasure.BaseUnitOfMeasureKey},
                             {"@ConversionAmount", unitOfMeasure.Conversion},
                             {"@SortValue", unitOfMeasure.SortOrder},
                             {"@EndUTCDateTime", context.ActionUtcDateTime},
                             {"@EndLocalDateTime", context.ActionDateTime},
                             {"@LastModifiedActorKey", (Guid?)context.Actor},
                             {"@LastModifiedBinaryValue", unitOfMeasure.LastModified},
                             {"@UOMKey", unitOfMeasure.Key}
                         });

                    connectionScope.Connection.Execute(
                        "Core.usp_UOMUpdate",
                        parameters,
                        null,
                        connectionScope.DefaultCommandTimeout,
                        CommandType.StoredProcedure);

                    UpdateUnitOfMeasureRoles(context, unitOfMeasure.Key, unitOfMeasure.Roles);

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        #region Private Members

        private void UpdateUnitOfMeasureRoles(Context context, Guid unitOfMeasureKey, IEnumerable<UOMRoleInternalCode> unitOfMeasureRoles)
        {
            SqlBuilder query = new SqlBuilder();
            query.SELECT("UOMRoleInternalCode")
                .FROM("Core.UOMRoleMember")
                .WHERE("DisassociationUTCDateTime IS NULL")
                .WHERE("UOMKey = @UOMKey");

            using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
            {
                // Get the list of unit of measure roles associated with this unit of measure.
                IReadOnlyCollection<string> currentRoles = connectionScope.Query(
                        query.ToString(),
                        new { UOMKey = unitOfMeasureKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Select(x => (string)x.UOMRoleInternalCode)
                        .ToList();

                // List of changes to roles.
                IReadOnlyCollection<string> roles = unitOfMeasureRoles.Select(uomr => uomr.ToInternalCode())
                    .ToList();

                // Find the unit of measure roles that were removed.
                IEnumerable<string> removedRoles = currentRoles.Except(roles);

                // Remove unit of measure roles that are no longer associated with this unit of measure.
                foreach (string role in removedRoles)
                {
                    DeleteUnitOfMeasureRole(context, unitOfMeasureKey, role);
                }

                // Find the unit of measure roles that were added.
                IEnumerable<string> addedRoles = roles.Except(currentRoles);
                foreach (var role in addedRoles)
                {
                    InsertUnitOfMeasureRole(context, unitOfMeasureKey, role);
                }
            }
        }

        private void InsertUnitOfMeasureRole(Context context, Guid unitOfMeasureKey, string unitOfMeasureRoleInternalCode)
        {
            using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
            {
                var parameters = new DynamicParameters(
                    new Dictionary<string, object>
                         {
                             {"@UOMKey", unitOfMeasureKey},
                             {"@UOMRoleInternalCode", unitOfMeasureRoleInternalCode},
                             {"@AssociationUTCDateTime", context.ActionUtcDateTime},
                             {"@AssociationLocalDateTime", context.ActionDateTime},
                             {"@AssociationActorKey", (Guid?)context.Actor}
                         });
                parameters.Add("@UOMRoleMemberKey", dbType: DbType.Guid, direction: ParameterDirection.Output);

                connectionScope.Connection.Execute(
                    "Core.usp_UOMRoleMemberInsert",
                    parameters,
                    null,
                    connectionScope.DefaultCommandTimeout,
                    CommandType.StoredProcedure);
            }
        }

        private void DeleteUnitOfMeasureRole(Context context, Guid unitOfMeasureKey, string unitOfMeasureRoleInternalCode)
        {
            using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
            {
                var parameters = new DynamicParameters(
                        new Dictionary<string, object>
                             {
                                 {"@UOMKey", unitOfMeasureKey},
                                 {"@UOMRoleInternalCode", unitOfMeasureRoleInternalCode},
                                 {"@DisassociationUTCDateTime", context.ActionUtcDateTime},
                                 {"@DisassociationLocalDateTime", context.ActionDateTime},
                                 {"@DisassociationActorKey", (Guid?)context.Actor}
                             });

                connectionScope.Connection.Execute(
                    "Core.usp_UOMRoleMemberDelete",
                    parameters,
                    null,
                    connectionScope.DefaultCommandTimeout,
                    CommandType.StoredProcedure);
            }
        }

        #endregion
    }
}
