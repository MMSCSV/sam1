using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using Dapper;
using Pyxis.Core.Data;
using Pyxis.Core.Data.Schema.TableTypes;
using CoreDAL = Pyxis.Core.Data.Schema.Core;

namespace CareFusion.Dispensing.Data.Repositories
{
    #region IActiveDirectoryDomainRepository

    public interface IActiveDirectoryDomainRepository
    {
        /// <summary>
        /// Retrieves a collection of <see cref="ActiveDirectoryDomain"/> by key.
        /// </summary>
        /// <param name="activeDirectoryDomainKeys">The collection of active diredtory domain keys or NULL for all.</param>
        /// <returns>An IEnumerable(T) object, where the generic parameter T is <see cref="ActiveDirectoryDomain"/>.</returns>
        IReadOnlyCollection<ActiveDirectoryDomain> GetActiveDirectoryDomains(
            IEnumerable<Guid> activeDirectoryDomainKeys = null);

        /// <summary>
        /// Retrieves an Active Directory domain by key.
        /// </summary>
        /// <param name="activeDirectoryDomainKey"></param>
        /// <returns>An <see cref="ActiveDirectoryDomain"/> object if exists, otherwise null.</returns>
        ActiveDirectoryDomain GetActiveDirectoryDomain(Guid activeDirectoryDomainKey);

        bool FullyQualifiedDomainNameExists(string name,
            Filter<Guid> ignoreActiveDirectoryDomainKey = default(Filter<Guid>));

        /// <summary>
        /// Persists the active directory domain.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="activeDirectoryDomain">The active directory domain.</param>
        Guid InsertActiveDirectoryDomain(Context context, ActiveDirectoryDomain activeDirectoryDomain);

        /// <summary>
        /// Updates the active directory doman.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="activeDirectoryDomain">The acive directory domain.</param>
        void UpdateActiveDirectoryDomain(Context context, ActiveDirectoryDomain activeDirectoryDomain);

        /// <summary>
        /// Updates only the properties related to synchronization for the specified active directory domain.
        /// </summary>
        /// <param name="activeDirectoryDomainKey"></param>
        /// <param name="highestCommittedUsnValue"></param>
        /// <param name="lastSuccessfulPollUtcDateTime"></param>
        /// <param name="lastSuccessfulPollDateTime"></param>
        /// <param name="lastPollStatusInternalCode"></param>
        void UpdateActiveDirectorySyncValues(Guid activeDirectoryDomainKey, long highestCommittedUsnValue,
            DateTime lastSuccessfulPollUtcDateTime, DateTime lastSuccessfulPollDateTime,
            string lastPollStatusInternalCode);

        bool AreDevicesUpgradedToVersion(string versionText);
    }

    #endregion

    public class ActiveDirectoryDomainRepository : IActiveDirectoryDomainRepository
    {
        static ActiveDirectoryDomainRepository()
        {
            // Dapper custom mappings
            SqlMapper.SetTypeMap(
                typeof(ActiveDirectoryDomain),
                new ColumnAttributeTypeMapper<ActiveDirectoryDomain>());
            SqlMapper.SetTypeMap(
                typeof(UserDirectoryGroup),
                new ColumnAttributeTypeMapper<UserDirectoryGroup>());
        }

        IReadOnlyCollection<ActiveDirectoryDomain> IActiveDirectoryDomainRepository.GetActiveDirectoryDomains(
            IEnumerable<Guid> activeDirectoryDomainKeys)
        {
            var activeDirectoryDomains = new List<ActiveDirectoryDomain>();
            if (activeDirectoryDomainKeys != null && !activeDirectoryDomainKeys.Any())
                return activeDirectoryDomains; // Empty results

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (activeDirectoryDomainKeys != null)
                    selectedKeys = new GuidKeyTable(activeDirectoryDomainKeys.Distinct());

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    using (var multi = connectionScope.Connection.QueryMultiple(
                        "Core.bsp_GetActiveDirectoryDomains",
                        new
                        {
                            SelectedKeys = selectedKeys.AsTableValuedParameter()
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure))
                    {
                        activeDirectoryDomains = multi.Read<ActiveDirectoryDomain>()
                            .ToList();

                        var userDirectoryGroups = !multi.IsConsumed
                            ? multi.Read<UserDirectoryGroup>().ToArray()
                            : new UserDirectoryGroup[] {};

                        foreach (var activeDirectoryDomain in activeDirectoryDomains)
                        {
                            activeDirectoryDomain.Groups = userDirectoryGroups.Where(udg =>
                                        udg.ActiveDirectoryDomainKey == activeDirectoryDomain.Key)
                                .ToArray();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return activeDirectoryDomains;
        }

        ActiveDirectoryDomain IActiveDirectoryDomainRepository.GetActiveDirectoryDomain(Guid activeDirectoryDomainKey)
        {
            var activeDirectoryDomains =
                ((IActiveDirectoryDomainRepository) this).GetActiveDirectoryDomains(new[] {activeDirectoryDomainKey});

            return activeDirectoryDomains.FirstOrDefault();
        }

        bool IActiveDirectoryDomainRepository.FullyQualifiedDomainNameExists(string name, Filter<Guid> ignoreActiveDirectoryDomainKey)
        {
            bool exists = false;

            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    SqlBuilder query = new SqlBuilder();
                    query.SELECT()
                        ._("adds.ActiveDirectoryDomainKey")
                        .FROM("Core.ActiveDirectoryDomainSnapshot adds")
                        .WHERE("adds.EndUTCDateTime IS NULL")
                        .WHERE("adds.FullyQualifiedDomainName = @FullyQualifiedDomainName");

                    if (ignoreActiveDirectoryDomainKey.HasValue)
                    {
                        query.WHERE("adds.ActiveDirectoryDomainKey <> @IgnoreActiveDirectoryDomainKey");
                    }

                    exists = connectionScope.Query(
                        query.ToString(),
                        new
                        {
                            FullyQualifiedDomainName = name,
                            IgnoreActiveDirectoryDomainKey = ignoreActiveDirectoryDomainKey.HasValue ? ignoreActiveDirectoryDomainKey.GetValueOrDefault() : default(Guid?)
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

        Guid IActiveDirectoryDomainRepository.InsertActiveDirectoryDomain(Context context,
            ActiveDirectoryDomain activeDirectoryDomain)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(activeDirectoryDomain, "activeDirectoryDomain");
            Guid activeDirectoryDomainKey = Guid.Empty;

            try
            {
                CoreDAL.IActiveDirectoryDomainRepository repository = new CoreDAL.ActiveDirectoryDomainRepository();

                using (ConnectionScopeFactory.Create())
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                {
                    activeDirectoryDomainKey = repository.InsertActiveDirectoryDomain(context.ToActionContext(),
                        new CoreDAL.Models.ActiveDirectoryDomain
                        {
                            ActiveDirectoryDomainKey = activeDirectoryDomain.Key,
                            HighestCommittedUsnValue = activeDirectoryDomain.HighestCommittedUsn,
                            LastSuccessfulPollUtcDateTime = activeDirectoryDomain.LastSuccessfulPollUtcDateTime,
                            LastSuccessfulPollLocalDateTime = activeDirectoryDomain.LastSuccessfulPollDateTime,
                            WorkgroupFlag = activeDirectoryDomain.Workgroup,
                            FullyQualifiedDomainName = activeDirectoryDomain.FullyQualifiedName,
                            ShortDomainName = activeDirectoryDomain.Name,
                            PollingDurationAmount = activeDirectoryDomain.PollingDuration,
                            ActiveFlag = activeDirectoryDomain.IsActive,
                            DomainControllerAddressValue = activeDirectoryDomain.ControllerAddress,
                            SystemAccountId = activeDirectoryDomain.SystemAccountId,
                            SystemAccountPasswordEncryptedValue = activeDirectoryDomain.SystemAccountPasswordEncrypted,
                            InvocationId = activeDirectoryDomain.InvocationId,
                            ScheduledPasswordSignInIntervalAmount = activeDirectoryDomain.ScheduledPasswordSignInInterval,
                            UserDirectoryTypeInternalCode = activeDirectoryDomain.UserDirectoryTypeInternalCode,
                            EncryptionAlgorithmInternalCode = activeDirectoryDomain.EncryptionAlgorithmInternalCode,
                            SecuredCommunicationFlag = activeDirectoryDomain.SecuredCommunication,
                            PortNumber = activeDirectoryDomain.PortNumber,
                            LDAPCertificateFileName = activeDirectoryDomain.LDAPCertificateFileName,
                            SupportUserFlag = activeDirectoryDomain.SupportUsers, // Obsolete
                            GroupName = activeDirectoryDomain.GroupName, // Obsolete
                            LastModifiedBinaryValue = activeDirectoryDomain.LastModified,
                            SupportDomainFlag = activeDirectoryDomain.IsSupportDomain
                        });

                    if (activeDirectoryDomain.Groups != null)
                    {
                        InsertUserDirectoryGroups(
                            context,
                            activeDirectoryDomainKey,
                            activeDirectoryDomain.Groups);
                    }

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return activeDirectoryDomainKey;
        }

        void IActiveDirectoryDomainRepository.UpdateActiveDirectoryDomain(Context context, ActiveDirectoryDomain activeDirectoryDomain)
        {
            try
            {
                CoreDAL.IActiveDirectoryDomainRepository repository = new CoreDAL.ActiveDirectoryDomainRepository();

                using (var connectionScope = ConnectionScopeFactory.Create())
                using (ITransactionScope tx = TransactionScopeFactory.Create())
                {
                    repository.UpdateActiveDirectoryDomain(context.ToActionContext(),
                        new CoreDAL.Models.ActiveDirectoryDomain
                        {
                            ActiveDirectoryDomainKey = activeDirectoryDomain.Key,
                            HighestCommittedUsnValue = activeDirectoryDomain.HighestCommittedUsn,
                            LastSuccessfulPollUtcDateTime = activeDirectoryDomain.LastSuccessfulPollUtcDateTime,
                            LastSuccessfulPollLocalDateTime = activeDirectoryDomain.LastSuccessfulPollDateTime,
                            WorkgroupFlag = activeDirectoryDomain.Workgroup,
                            FullyQualifiedDomainName = activeDirectoryDomain.FullyQualifiedName,
                            ShortDomainName = activeDirectoryDomain.Name,
                            PollingDurationAmount = activeDirectoryDomain.PollingDuration,
                            ActiveFlag = activeDirectoryDomain.IsActive,
                            DomainControllerAddressValue = activeDirectoryDomain.ControllerAddress,
                            SystemAccountId = activeDirectoryDomain.SystemAccountId,
                            SystemAccountPasswordEncryptedValue = activeDirectoryDomain.SystemAccountPasswordEncrypted,
                            InvocationId = activeDirectoryDomain.InvocationId,
                            ScheduledPasswordSignInIntervalAmount = activeDirectoryDomain.ScheduledPasswordSignInInterval,
                            UserDirectoryTypeInternalCode = activeDirectoryDomain.UserDirectoryTypeInternalCode,
                            EncryptionAlgorithmInternalCode = activeDirectoryDomain.EncryptionAlgorithmInternalCode,
                            SecuredCommunicationFlag = activeDirectoryDomain.SecuredCommunication,
                            PortNumber = activeDirectoryDomain.PortNumber,
                            LDAPCertificateFileName = activeDirectoryDomain.LDAPCertificateFileName,
                            SupportUserFlag = activeDirectoryDomain.SupportUsers, // Obsolete
                            GroupName = activeDirectoryDomain.GroupName, // Obsolete
                            LastModifiedBinaryValue = activeDirectoryDomain.LastModified,
                            SupportDomainFlag = activeDirectoryDomain.IsSupportDomain
                        });

                    // If groups is null then we assume all associations were removed.
                    UpdateUserDirectoryGroups(
                        connectionScope,
                        context,
                        activeDirectoryDomain.Key,
                        activeDirectoryDomain.Groups ?? new UserDirectoryGroup[0]);

                    tx.Complete();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
        }

        void IActiveDirectoryDomainRepository.UpdateActiveDirectorySyncValues(Guid activeDirectoryDomainKey, long highestCommittedUsnValue,
            DateTime lastSuccessfulPollUtcDateTime, DateTime lastSuccessfulPollDateTime, string lastPollStatusInternalCode)
        {
            try
            {
                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    connectionScope.Execute(
                        "Core.usp_ActiveDirectoryDomainSyncUpdate",
                        new
                        {
                            HighestCommittedUSNValue = highestCommittedUsnValue,
                            LastSuccessfulPollUTCDateTime = lastSuccessfulPollUtcDateTime,
                            LastSuccessfulPollLocalDateTime = lastSuccessfulPollDateTime,
                            LastPollStatusInternalCode = lastPollStatusInternalCode,
                            ActiveDirectoryDomainKey = activeDirectoryDomainKey
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

        bool IActiveDirectoryDomainRepository.AreDevicesUpgradedToVersion(string versionText)
        {
            CoreDAL.IActiveDirectoryDomainRepository repository = new CoreDAL.ActiveDirectoryDomainRepository();
            return repository.AreDevicesUpgradedToVersion(versionText);
        }

        #region Private Members

        private void InsertUserDirectoryGroups(Context context, Guid activeDirectoryDomainKey, IEnumerable<UserDirectoryGroup> userDirectoryGroups)
        {
            Guard.ArgumentNotNull(context, "context");

            if (userDirectoryGroups == null)
                return;

            CoreDAL.IActiveDirectoryDomainRepository repository = new CoreDAL.ActiveDirectoryDomainRepository();
            foreach (UserDirectoryGroup userDirectoryGroup in userDirectoryGroups)
            {
                repository.InsertUserDirectoryGroup(context.ToActionContext(),
                    new CoreDAL.Models.UserDirectoryGroup
                    {
                        UserDirectoryGroupKey = userDirectoryGroup.Key,
                        ActiveDirectoryDomainKey = activeDirectoryDomainKey,
                        GroupName = userDirectoryGroup.GroupName,
                        SupportUserFlag = userDirectoryGroup.SupportUser,
                        LastModifiedBinaryValue = userDirectoryGroup.LastModified
                    });
            }
        }

        private void UpdateUserDirectoryGroups(IConnectionScope connectionScope, Context context, Guid activeDirectoryDomainKey, IEnumerable<UserDirectoryGroup> userDirectoryGroups)
        {
            Guard.ArgumentNotNull(context, "context");
            CoreDAL.IActiveDirectoryDomainRepository repository = new CoreDAL.ActiveDirectoryDomainRepository();

            SqlBuilder query = new SqlBuilder();
            query.SELECT("udgs.UserDirectoryGroupKey")
                .FROM("Core.UserDirectoryGroupSnapshot udgs")
                .WHERE("udgs.EndUTCDateTime IS NULL")
                .WHERE("udgs.DeleteFlag = 0")
                .WHERE("udgs.ActiveDirectoryDomainKey = @ActiveDirectoryDomainKey");

            // Get the list of user directory group entities associated with this active directory domain.
            IReadOnlyCollection<Guid> currentUserDirectoryGroups = connectionScope.Query(
                        query.ToString(),
                        new { ActiveDirectoryDomainKey = activeDirectoryDomainKey },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text)
                        .Select(x => (Guid)x.UserDirectoryGroupKey)
                        .ToList();

            // Find the user directory groups that were removed.
            var removedUserDirectoryGroupKeys = currentUserDirectoryGroups.Except(userDirectoryGroups.Select(udg => udg.Key));

            // Remove user directory group entities that are no longer associated with this active directory domain.
            foreach (var userDirectoryGroupKey in removedUserDirectoryGroupKeys)
            {
                repository.DeleteUserDirectoryGroup(context.ToActionContext(), userDirectoryGroupKey);
            }

            // Find the user directory groups that were added or updated.
            List<UserDirectoryGroup> addedUserUserDirectoryGroups = new List<UserDirectoryGroup>();
            foreach (UserDirectoryGroup userDirectoryGroup in userDirectoryGroups)
            {
                if (userDirectoryGroup.IsTransient())
                {
                    addedUserUserDirectoryGroups.Add(userDirectoryGroup);
                    continue;
                }

                repository.UpdateUserDirectoryGroup(context.ToActionContext(),
                    new CoreDAL.Models.UserDirectoryGroup
                    {
                        UserDirectoryGroupKey = userDirectoryGroup.Key,
                        ActiveDirectoryDomainKey = userDirectoryGroup.ActiveDirectoryDomainKey,
                        GroupName = userDirectoryGroup.GroupName,
                        SupportUserFlag = userDirectoryGroup.SupportUser,
                        LastModifiedBinaryValue = userDirectoryGroup.LastModified
                    });
            }

            // Add the new user directory groups.
            InsertUserDirectoryGroups(
                context,
                activeDirectoryDomainKey,
                addedUserUserDirectoryGroups);
        }

        #endregion
    }
}
