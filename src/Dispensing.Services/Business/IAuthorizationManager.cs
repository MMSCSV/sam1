using System;
using System.Collections.Generic;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data;
using CareFusion.Dispensing.Data.Entities;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Services.Business
{
    /// <summary>
    /// Provides authorization access checking for service operations.
    /// </summary>
    public interface IAuthorizationManager
    {
        /// <summary>
        /// Returns the list of facilities authorized for the given context and any of the specified permissions.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="context">The <see cref="Context"/>.</param>
        /// <param name="permissions">The permissions that are authorized.</param>
        /// <param name="allPermissionsRequired">All permissions are required to be present.</param>
        /// <returns>An Queryable(T) object, where the generic parameter T is <see cref="FacilityEntity"/>.</returns>
        IQueryable<FacilityEntity> GetQueryableAuthorizedFacilities(IRepository repository, Context context, IEnumerable<PermissionInternalCode> permissions = null, bool allPermissionsRequired = false);

        /// <summary>
        /// Returns the list of facilities authorized for the given context and any of the specified permissions.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="userAccountKey">The user account key.</param>
        /// <param name="permissions">The permissions that are authorized.</param>
        /// <param name="allPermissionsRequired">All permissions are required to be present.</param>
        /// <returns>An Queryable(T) object, where the generic parameter T is <see cref="FacilityEntity"/>.</returns>
        IQueryable<FacilityEntity> GetQueryableAuthorizedFacilities(IRepository repository, Guid userAccountKey, IEnumerable<PermissionInternalCode> permissions = null, bool allPermissionsRequired = false);

        [Obsolete("Use FacilityDataSource on the server.")]
        void CheckAccess(Context context, IEnumerable<Guid> facilityKeys, IEnumerable<PermissionInternalCode> permissions, bool allPermissionsRequired = false);
    }
}
