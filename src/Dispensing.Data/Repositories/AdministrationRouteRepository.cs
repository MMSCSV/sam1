using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data.Models;
using Dapper;
using Pyxis.Core.Data;
using Pyxis.Core.Data.Schema.TableTypes;
using HcOrderDAL = Pyxis.Core.Data.Schema.HcOrder;

namespace CareFusion.Dispensing.Data.Repositories
{
    #region IAdministrationRouteRepository

    public interface IAdministrationRouteRepository
    {
        IReadOnlyCollection<AdministrationRoute> GetAdministrationRoutes(IEnumerable<Guid> administrationRouteKeys = null,
                                                                 bool? deleted = null, Guid? externalSystemKey = null,
                                                                 string displayCode = null);

        AdministrationRoute GetAdministrationRoute(Guid administrationRouteKey);

        bool CodeExists(Guid externalSystemKey, string code,
            Filter<Guid> ignoreAdministrationRouteKey = default(Filter<Guid>));

        Guid InsertAdministrationRoute(Context context, AdministrationRoute administrationRoute);

        void UpdateAdministrationRoute(Context context, AdministrationRoute administrationRoute);

        void DeleteAdministrationRoute(Context context, Guid administrationRouteKey);
    }

    #endregion

    public class AdministrationRouteRepository : IAdministrationRouteRepository
    {
        IReadOnlyCollection<AdministrationRoute> IAdministrationRouteRepository.GetAdministrationRoutes(IEnumerable<Guid> administrationRouteKeys, bool? deleted,
            Guid? externalSystemKey, string displayCode)
        {
            List<AdministrationRoute> administrationRoutes = new List<AdministrationRoute>();
            if (administrationRouteKeys != null && !administrationRouteKeys.Any())
                return administrationRoutes; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (administrationRouteKeys != null)
                    selectedKeys = new GuidKeyTable(administrationRouteKeys.Distinct());

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var adminRouteResults = connectionScope.Query<AdminRouteResult>(
                        "HcOrder.bsp_GetAdminRoutes",
                        new
                        {
                            SelectedKeys = selectedKeys.AsTableValuedParameter(),
                            DeleteFlag = deleted,
                            ExternalSystemKey = externalSystemKey,
                            DisplayCode = displayCode
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure);

                    foreach (var adminRouteResult in adminRouteResults)
                    {
                        AdministrationRoute administrationRoute = new AdministrationRoute(adminRouteResult.AdminRouteKey)
                        {
                            ExternalSystemKey = adminRouteResult.ExternalSystemKey,
                            ExternalSystemName = adminRouteResult.ExternalSystemName,
                            DisplayCode = adminRouteResult.AdminRouteCode,
                            Description = adminRouteResult.DescriptionText,
                            SortOrder = adminRouteResult.SortValue,
                            IsDeleted = adminRouteResult.DeleteFlag,
                            LastModified = adminRouteResult.LastModifiedBinaryValue.ToArray()
                        };

                        administrationRoutes.Add(administrationRoute);
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return administrationRoutes;
        }

        AdministrationRoute IAdministrationRouteRepository.GetAdministrationRoute(Guid administrationRouteKey)
        {
            var administrationRoutes =
                ((IAdministrationRouteRepository)this).GetAdministrationRoutes(new[] { administrationRouteKey });

            return administrationRoutes.FirstOrDefault();
        }

        bool IAdministrationRouteRepository.CodeExists(Guid externalSystemKey, string code, Filter<Guid> ignoreAdministrationRouteKey)
        {
            bool exists = false;

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    SqlBuilder query = new SqlBuilder();
                    query.SELECT()
                        ._("ar.AdminRouteKey")
                        .FROM("HcOrder.AdminRoute ar")
                        .WHERE("ar.DeleteFlag = 0")
                        .WHERE("ar.ExternalSystemKey = @ExternalSystemKey")
                        .WHERE("ar.AdminRouteCode = @AdminRouteCode");

                    if (ignoreAdministrationRouteKey.HasValue)
                    {
                        query.WHERE("ar.AdminRouteKey <> @IgnoreAdminRouteKey");
                    }

                    exists = connectionScope.Query(
                        query.ToString(),
                        new
                        {
                            ExternalSystemKey = externalSystemKey,
                            AdminRouteCode = code,
                            IgnoreAdminRouteKey = ignoreAdministrationRouteKey.HasValue ? ignoreAdministrationRouteKey.GetValueOrDefault() : default(Guid?)
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

        Guid IAdministrationRouteRepository.InsertAdministrationRoute(Context context, AdministrationRoute administrationRoute)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(administrationRoute, "administrationRoute");
            Guid? administrationRouteKey = null;

            try
            {
                HcOrderDAL.IAdminRouteRepository adminRouteRepository = new HcOrderDAL.AdminRouteRepository();

                administrationRouteKey = adminRouteRepository.InsertAdminRoute(context.ToActionContext(),
                    new HcOrderDAL.Models.AdminRoute
                    {
                        AdminRouteKey = administrationRoute.Key,
                        ExternalSystemKey = administrationRoute.ExternalSystemKey,
                        AdminRouteCode = administrationRoute.DisplayCode,
                        DescriptionText = administrationRoute.Description,
                        SortValue = administrationRoute.SortOrder,
                        LastModifiedBinaryValue = administrationRoute.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return administrationRouteKey.GetValueOrDefault();
        }

        void IAdministrationRouteRepository.UpdateAdministrationRoute(Context context, AdministrationRoute administrationRoute)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(administrationRoute, "administrationRoute");

            try
            {
                HcOrderDAL.IAdminRouteRepository adminRouteRepository = new HcOrderDAL.AdminRouteRepository();

                adminRouteRepository.UpdateAdminRoute(context.ToActionContext(),
                    new HcOrderDAL.Models.AdminRoute
                    {
                        AdminRouteKey = administrationRoute.Key,
                        ExternalSystemKey = administrationRoute.ExternalSystemKey,
                        AdminRouteCode = administrationRoute.DisplayCode,
                        DescriptionText = administrationRoute.Description,
                        SortValue = administrationRoute.SortOrder,
                        LastModifiedBinaryValue = administrationRoute.LastModified
                    });
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IAdministrationRouteRepository.DeleteAdministrationRoute(Context context, Guid administrationRouteKey)
        {
            Guard.ArgumentNotNull(context, "context");

            try
            {
                HcOrderDAL.IAdminRouteRepository adminRouteRepository = new HcOrderDAL.AdminRouteRepository();

                adminRouteRepository.DeleteAdminRoute(context.ToActionContext(),
                    administrationRouteKey);
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }
    }
}
