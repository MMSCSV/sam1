using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data;
using CareFusion.Dispensing.Data.Entities;
using CareFusion.Dispensing.Resources;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Services.Business
{
    public class AuthorizationManager : IAuthorizationManager
    {
        #region IAuthorizationManager Members

        public IQueryable<FacilityEntity> GetQueryableAuthorizedFacilities(IRepository repository, Context context, IEnumerable<PermissionInternalCode> permissions, bool allPermissionsRequired = false)
        {
            Guard.ArgumentNotNull(repository, "repository");
            Guard.ArgumentNotNull(context, "context");

            if (context.Actor is UserActor)
            {
                if (context.User.IsSupportUser)
                {
                    return GetQueryableAuthorizedFacilities(repository, context.Actor.Key, permissions,
                                                        allPermissionsRequired);
                }
                
                return GetQueryableAuthorizedFacilitiesInternal(repository, context.Actor.Key, permissions,
                                                    allPermissionsRequired);
            }

            // Return all facilities if any other actor.
            return repository.GetQueryableEntity<FacilityEntity>();
        }

        public IQueryable<FacilityEntity> GetQueryableAuthorizedFacilities(IRepository repository, Guid userAccountKey, IEnumerable<PermissionInternalCode> permissions, bool allPermissionsRequired = false)
        {
            Guard.ArgumentNotNull(repository, "repository");

            var contextUser = repository.GetQueryableEntity<UserAccountEntity>()
                        .Where(ua => ua.Key == userAccountKey)
                        .Select(ua => new { ua.Key, ua.SupportUserFlag })
                        .SingleOrDefault();

            if (contextUser == null)
                throw new EntityNotFoundException(DataResources.LoadFailed_UserAccountNotFound, userAccountKey);

            if (contextUser.SupportUserFlag)
            {
                return repository.GetQueryableEntity<FacilityEntity>();
            }

            return GetQueryableAuthorizedFacilitiesInternal(repository, userAccountKey, permissions,
                                                        allPermissionsRequired);
        }

        #region Obsoleted

        public void CheckAccess(Context context, IEnumerable<Guid> facilityKeys, IEnumerable<PermissionInternalCode> permissions, bool allPermissionsRequired = false)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(facilityKeys, "facilityKeys");

            if (context.Actor is UserActor)
            {
                if (!facilityKeys.Any())
                {
                    // The user might have been removed from all facilities
                    throw new AccessDeniedException(ServiceResources.Exception_AccessDenied_NoFacility);
                }

                IEnumerable<Guid> authorizedFacilities;
                using (ICoreRepository repository = RepositoryFactory.Create<ICoreRepository>())
                {
                    // Spec: 36264 is not correctly implemented here. Will need to change in GA to meet the spec. The common set of permssions
                    // must be the same throughout the facilities.

                    // Get all authorized facilities
                    authorizedFacilities = GetQueryableAuthorizedFacilities(repository, context, permissions, allPermissionsRequired)
                        .Select(f => f.Key)
                        .ToArray();

                    IEnumerable<Guid> unauthorizedFacilities = facilityKeys.Where(f => !authorizedFacilities.Contains(f));
                    if (unauthorizedFacilities.Any())
                    {
                        IEnumerable<string> unauthorizedFacilityNames = repository.GetQueryableEntity<FacilityEntity>()
                            .Where(f => unauthorizedFacilities.Contains(f.Key))
                            .Select(f => f.FacilityName)
                            .ToArray();

                        IEnumerable<string> permissionInternalCodes = permissions.Select(p => p.ToInternalCode());
                        IEnumerable<string> permissionNames = repository.GetQueryableEntity<PermissionEntity>()
                            .Where(p => permissionInternalCodes.Contains(p.InternalCode))
                            .Select(p => p.PermissionName)
                            .ToArray();

                        string p1 = string.Join(ServiceResources.Exception_AccessDeniedSeparator,
                                                unauthorizedFacilityNames);
                        string p2 = string.Join(ServiceResources.Exception_AccessDeniedSeparator, permissionNames);

                        if (allPermissionsRequired)
                        {
                            throw new AccessDeniedException(string.Format(CultureInfo.CurrentCulture,
                                                                      ServiceResources.Exception_AccessDeniedRequiredPermissions, p1, p2));
                        }

                        throw new AccessDeniedException(string.Format(CultureInfo.CurrentCulture,
                                                                      ServiceResources.Exception_AccessDenied, p1, p2));
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Private Members

        private static IQueryable<FacilityEntity> GetQueryableAuthorizedFacilitiesInternal(IRepository repository, Guid userAccountKey, IEnumerable<PermissionInternalCode> permissions, bool allPermissionsRequired = false)
        {
            Guard.ArgumentNotNull(repository, "repository");

            if (permissions != null &&
                permissions.Count() > 0)
            {
                IEnumerable<string> permissionInternalCodes = permissions.Select(p => p.ToInternalCode());

                if (!allPermissionsRequired)
                {
                    // Return authorized facilities based on permissions.
                    return (from auth in repository.GetQueryableEntity<UserAuthorizationEntity>()
                            join f in repository.GetQueryableEntity<FacilityEntity>()
                                on auth.FacilityKey equals f.Key
                            where auth.UserAccountKey == userAccountKey &&
                                  permissionInternalCodes.Contains(auth.PermissionInternalCode)
                            select f).Distinct();
                }

                // Get facilities that have all the specified permissions.
                var authorizedFacilitiesPermissions = (
                    from auth in repository.GetQueryableEntity<UserAuthorizationEntity>()
                    where auth.UserAccountKey == userAccountKey &&
                        permissionInternalCodes.Contains(auth.PermissionInternalCode)
                    select auth)
                    .GroupBy(auth => new { auth.FacilityKey, auth.PermissionInternalCode })
                    .Select(g => g.Key);

                var authorizedFacilities = (
                    from afp in authorizedFacilitiesPermissions
                    select afp)
                    .GroupBy(afp => afp.FacilityKey)
                    .Where(g => g.Count() == permissions.Count())
                    .Select(g => g.Key);

                // Return authorized facilities based on permissions.
                return (from f in repository.GetQueryableEntity<FacilityEntity>()
                        join af in authorizedFacilities
                            on f.Key equals af
                        select f);
            }

            // Return authorized facilities.
            return (from uaf in repository.GetQueryableEntity<UserAccountFacilityEntity>()
                    join f in repository.GetQueryableEntity<FacilityEntity>()
                        on uaf.FacilityKey equals f.Key
                    where uaf.UserAccountKey == userAccountKey
                    select f);
        }

        #endregion
    }
}
