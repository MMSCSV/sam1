using System;
using System.Collections.Generic;
using CareFusion.Dispensing.Contracts;
using Pyxis.Core.Data.InternalCodes;

namespace CareFusion.Dispensing.Data
{
    /// <summary>
    /// Represents a repository specific to common data entities.
    /// </summary>
    public interface ICoreRepository : IRepository
    {
        #region Authentication Members

        bool PasswordContainsDictionaryWord(string passwordText);

        #endregion

        #region Distributor Members

        /// <summary>
        /// Retrieves a collection of <see cref="Distributor"/> by key.
        /// </summary>
        /// <param name="distributorKeys">The collection of distributor keys or NULL for all.</param>
        /// <param name="deleted"></param>
        /// <param name="distributorId"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="Distributor"/>.</returns>
        IEnumerable<Distributor> GetDistributors(IEnumerable<Guid> distributorKeys = null, bool? deleted = null, string distributorId = null);

        /// <summary>
        /// Retreive the dispensing system record
        /// </summary>
        /// <returns></returns>
        Distributor GetDistributor(string distributorId);

        /// <summary>
        /// Persists the <see cref="Distributor"/> to the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="distributor">The distributor to save.</param>
        /// <returns>
        /// The distributor surrogate key, which uniquely identifies the distributor in the database.
        /// </returns>
        Guid InsertDistributor(Context context, Distributor distributor);

        /// <summary>
        /// Updates an existing <see cref="Distributor"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="distributor">The distributor to update.</param>
        void UpdateDistributor(Context context, Distributor distributor);

        /// <summary>
        /// Logically deletes an existing <see cref="Distributor"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="distributorKey">The distributor surrogate key.</param>
        void DeleteDistributor(Context context, Guid distributorKey);

        #endregion

        #region User Account (Authentication) Members

        AuthUserAccount GetAuthenticationUserAccount(Guid userAccountKey);

        AuthUserAccount GetAuthenticationUserAccount(string cardSerialID);

        AuthUserAccount GetAuthUserAccountByRFIDCardSerialID(string rfidCardSerialID);

        AuthUserAccount GetAuthenticationUserAccount(string domainNameOrFullyQualifiedName, string userId);

        IEnumerable<AuthUserAccount> GetAuthenticationUserAccounts(string userId);

        AuthUserAccount GetAuthenticationUserAccountByScanCode(string domainNameOrFullyQualifiedName, string userScanCode);

        IEnumerable<AuthUserAccount> GetAuthenticationUserAccountsByScanCode(string userScanCode);

        PasswordCredential GetPasswordCredential(Guid userAccountKey);

        bool UserHasFacilityPrivileges(Guid facilityKey, Guid userAccountKey);

        #endregion

        #region User Account Members

        /// <summary>
        /// Returns all active user permissions for the specified user account key.
        /// </summary>
        /// <param name="userAccountKey">The user account key.</param>
        /// <param name="isSupportUser"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="PermissionInternalCode"/>.</returns>
        IEnumerable<PermissionInternalCode> GetUserPermissions(Guid userAccountKey, bool isSupportUser = false);

        /// <summary>
        /// Returns all active user permissions for the specified user account key and for a specific device.
        /// </summary>
        /// <param name="userAccountKey">The user account key.</param>
        /// <param name="dispensingDeviceKey">The dispensng device key.</param>
        /// <param name="isSupportUser"></param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="PermissionInternalCode"/>.</returns>
        IEnumerable<PermissionInternalCode> GetUserPermissions(Guid userAccountKey, Guid dispensingDeviceKey, bool isSupportUser = false);
        
        /// <summary>
        /// Gets the user account that matches the specified key.
        /// </summary>
        /// <param name="userAccountKey">The user account key.</param>
        /// <returns>A <see cref="UserAccount"/> object otherwise null if not exist.</returns>
        UserAccount GetUserAccount(Guid userAccountKey);
        
        /// <summary>
        /// Gets the user account that matches the specified identity.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>A list of <see cref="UserAccount"/> object otherwise null if not exist.</returns>
        IEnumerable<UserAccount> GetUserAccountByUserId(string userId);

        /// <summary>
        /// Gets the user account that matches the specified identity and active directory.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="activeDirectoryDomainKey"></param>
        /// <returns>A list of <see cref="UserAccount"/> object otherwise null if not exist.</returns>
        UserAccount GetUserAccountByUserId(string userId, Guid? activeDirectoryDomainKey);

        DateTime? GetLastUnlockedUtcDateTime(Guid userAccountKey, DateTime sinceUtcDateTime);

        DateTime? GetLastUndeletedUtcDateTime(Guid userAccountKey, DateTime sinceUtcDateTime);

        /// <summary>
        /// Insert support user account to the database.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userAccount"></param>
        /// <returns></returns>
        Guid UpsertSupportUserAccount(Context context, UserAccount userAccount);

        /// <summary>
        /// Persists the user account to the database.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="userAccount">The user account to save.</param>
        /// <returns>
        /// A user account key, which uniquely identifies the user account in the database.
        /// </returns>
        /// <exception cref="DataException">An error was encountered accessing the data store.</exception>
        Guid InsertUserAccount(Context context, UserAccount userAccount);

        /// <summary>
        /// Updates an existing user account.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="userAccount">The user account to update.</param>
        void UpdateUserAccount(Context context, UserAccount userAccount);

        /// <summary>
        /// Updates activity information of an existing user account.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userAccountKey"></param>
        /// <param name="lastPasswordExpirationNoticeUtcDateTime"></param>
        /// <param name="lastPasswordExpirationNoticeDateTime"></param>
        void UpdateUserAccountActivity(Context context, Guid userAccountKey,
                                       DateTime? lastPasswordExpirationNoticeUtcDateTime,
                                       DateTime? lastPasswordExpirationNoticeDateTime);

        /// <summary>
        /// Updates activity information of an existing user account.
        /// </summary>
        /// <param name="dispensingDeviceKey"></param>
        /// <param name="LastSuccessfulPasswordAuthenticationUtcDateTime"></param>
        /// <param name="lastSuccessfulPasswordAuthenticationDateTime"></param>
        void UpdateUserAccountLastSuccessfulPasswordAuthentication(Guid? dispensingDeviceKey, Guid userAccountKey,
                                       DateTime? lastSuccessfulPasswordAuthenticationUtcDateTime,
                                       DateTime? lastSuccessfulPasswordAuthenticationDateTime);

        /// <summary>
        /// Logically deletes an existing user account.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="userAccountKey">The user account to update.</param>
        void DeleteUserAccount(Context context, Guid userAccountKey);

        /// <summary>
        /// Undelete an existing user account.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="activeDirectoryObjectGuid">The active directory object GUID.</param>
        void UndeleteActiveDirectoryUserAccount(Context context, Guid activeDirectoryObjectGuid);

        /// <summary>
        /// Locks an existing user account.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="userAccountKey">The user account key.</param>
        void LockUserAccount(Context context, Guid userAccountKey);

        /// <summary>
        /// Unlocks an existing user account.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="userAccountKey">The user account key.</param>
        void UnlockUserAccount(Context context, Guid userAccountKey);

        /// <summary>
        /// Persists a users password credential.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userAccountKey">The user account key.</param>
        /// <param name="passwordCredential">The password credential.</param>
        void InsertPasswordCredential(Context context, Guid userAccountKey, PasswordCredential passwordCredential);

        /// <summary>
        /// Updates a users password credential.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userAccountKey">The user account key.</param>
        /// <param name="passwordCredential">The password credential.</param>
        void UpdatePasswordCredential(Context context, Guid userAccountKey, PasswordCredential passwordCredential);

        /// <summary>
        /// Returns a list of a specified number of previous passwords for the specified user.
        /// </summary>
        /// <param name="userAccountKey">The user account key.</param>
        /// <param name="passwordCount">The number or previous password to return.</param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="PasswordCredential"/>.</returns>
        IEnumerable<PasswordCredential> GetPasswordHistory(Guid userAccountKey, int passwordCount);

        /// <summary>
        /// Deletes an existing <see cref="UserAccount"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userAccountKeys">The user account keys which include a tuple of UserAccountKey and UserAccountSnapshotKey.</param>
        /// <param name="attributes"></param>
        void DeleteMultiUserAccounts(Context context, IEnumerable<Guid> activeDirectoryGloballyUniqueId);

        /// <summary>
        /// Updates existing <see cref="UserAccount"/> in batch
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userAccountKeys">The user account attributes include User account info .</param>
        /// <param name="attributes"></param>
        void UpdateMultiActiveDirectoryUserAccounts(Context context, IEnumerable<UserAccount> attributes);

        /// <summary>
        /// Insert <see cref="UserAccount"/> in batch 
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userAccountKeys">The user account attributes include User account info .</param>
        /// <param name="attributes"></param>
        void InsertMultiActiveDirectoryUserAccounts(Context context, IEnumerable<UserAccount> attributes);

        /// <summary>
        /// Register Domian user account and map all the user lists from the local user
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="facilityKey">Facility Key</param>
        /// <param name="localUserAccountKey">local user account key</param>
        /// <param name="domainUserAccountKey">domain user account key</param>
        void MapLocalUserListsToDomainUser(Context context, Guid localUserAccountKey, Guid domainUserAccountKey);

        void InsertUserMergeAssociation(Context context, Guid localUserAccountKey, Guid domainUserAccountKey);

        bool UserHasUnResolvedMyItems(Guid localUserKey);

        UserMergeStatus IsUserAccountRegistered(Guid domainUserAccountKey);

        UserMergeSummary GetUserMergeSummary(Guid userAccountKey);

        #endregion

        #region User Fingerprint Members

        /// <summary>
        /// Gets a users fingerprint based on the specified user account key.
        /// </summary>
        /// <param name="userAccountKey">The user account key.</param>
        /// <returns>A <see cref="byte"/> array otherwise null if not exist.</returns>
        UserFingerprint GetUserFingerprint(Guid userAccountKey);

        /// <summary>
        /// Persists the user fingerprint to the database.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="userFingerprint">The user fingerprint to save.</param>
        void InsertUserFingerprint(Context context, UserFingerprint userFingerprint);

        /// <summary>
        /// Updates an existing user fingerprint.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="userFingerprint">The user fingerprint to update.</param>
        void UpdateUserFingerprint(Context context, UserFingerprint userFingerprint);

        #endregion

        #region Role Members

        /// <summary>
        /// Loads the role that matched the specified key otherwise throws an exception.
        /// </summary>
        /// <param name="roleKey">The role key.</param>
        /// <returns>A <see cref="Role"/> object otherwise throws an <see cref="EntityNotFoundException"/> if not exist.</returns>
        Role LoadRole(Guid roleKey);

        /// <summary>
        /// Retrieves a role by key.
        /// </summary>
        /// <param name="roleKey"></param>
        /// <returns></returns>
        Role GetRole(Guid roleKey);

        /// <summary>
        /// Persists the role in the database.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="role">The role to save.</param>
        /// <returns>A role key, which uniquely identifies the role in the database.</returns>
        Guid InsertRole(Context context, Role role);

        /// <summary>
        /// Updates an existing role in the database.
        /// </summary>
        /// <param name="context">The actionable context.</param>
        /// <param name="role">The role to update.</param>
        void UpdateRole(Context context, Role role);

        /// <summary>
        /// Logically deletes an existing <see cref="Role"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="roleKey">The role's surrogate key.</param>
        void DeleteRole(Context context, Guid roleKey);

        #endregion

        #region UserRoleMember Members

        Guid InsertUserRoleMember(Context context, Guid userAccountKey, Guid roleKey, bool medicationTemporaryAccess);

        void DeleteUserRoleMember(Context context, Guid userAccountKey, Guid roleKey, bool medicationTemporaryAccess);

        void DeleteUserRoleMember(Context context, Guid userRoleMemberKey);

        #endregion

        #region UserRoleMemberArea Members

        void InsertUserRoleMemberArea(Context context, Guid userRoleMemberKey, Guid areaKey);

        void DeleteUserRoleMemberArea(Context context, Guid userRoleMemberKey, Guid areaKey);

        #endregion

        #region User Type Members

        /// <summary>
        /// Retrieves a collection of <see cref="UserType"/>.
        /// </summary>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="UserType"/>.</returns>
        IEnumerable<UserType> GetUserTypes();

        /// <summary>
        /// Retreive the user type record.
        /// </summary>
        /// <returns>The <see cref="UserType"/> instance; otherwise null if not found.</returns>
        UserType GetUserType(Guid userTypeKey);

        /// <summary>
        /// Persists the <see cref="UserType"/> to the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userType">The user type to save.</param>
        /// <returns>
        /// The user type surrogate key, which uniquely identifies the user type in the database.
        /// </returns>
        Guid InsertUserType(Context context, UserType userType);

        /// <summary>
        /// Updates an existing <see cref="UserType"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userType">The user type to update.</param>
        void UpdateUserType(Context context, UserType userType);

        /// <summary>
        /// Deletes an existing <see cref="UserType"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userTypeKey">The user type to delete.</param>
        void DeleteUserType(Context context, Guid userTypeKey);

        #endregion
    }
}

