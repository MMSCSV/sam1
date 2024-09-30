using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Models;
using Dapper;
using Pyxis.Core.Data;
using Pyxis.Core.Data.Schema;
using Pyxis.Core.Data.Schema.TableTypes;

namespace CareFusion.Dispensing.Data.Repositories
{
    #region IServerRepository Interface

    public interface IServerRepository
    {
        /// <summary>
        /// Retrieves a collection of <see cref="CareFusion.Dispensing.Server"/> by key.
        /// </summary>
        /// <param name="serverKeysFilter">The collection of server keys or NULL for all.</param>
        /// <param name="serverNameFilter"></param>
        /// <param name="syncFilter"></param>
        /// <returns>An IReadOnlyCollection(T) object, where the generic parameter T is <see cref="CareFusion.Dispensing.Server"/>.</returns>
        IReadOnlyCollection<CareFusion.Dispensing.Models.Server> GetServers(Filter<IEnumerable<Guid>> serverKeysFilter = default(Filter<IEnumerable<Guid>>),
            Filter<string> serverNameFilter = default(Filter<string>),
            Filter<bool> syncFilter = default(Filter<bool>));

        /// <summary>
        /// Retreive the server record.
        /// </summary>
        /// <returns></returns>
        CareFusion.Dispensing.Models.Server GetServer(Guid serverKey);

        /// <summary>
        /// Retreive the server record.
        /// </summary>
        /// <returns></returns>
        CareFusion.Dispensing.Models.Server GetServer(string serverName);

        /// <summary>
        /// Checks to see if there is an existing server name within the system.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ignoreServerKey"></param>
        /// <returns></returns>
        bool ServerNameExists(string name, Guid? ignoreServerKey = null);

        /// <summary>
        /// Checks to see if there is an existing server address within the system.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="ignoreServerKey"></param>
        /// <returns></returns>
        bool ServerAddressExists(string address, Guid? ignoreServerKey = null);

        /// <summary>
        /// Persists the <see cref="CareFusion.Dispensing.Server"/> to the database.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The server to save.</param>
        /// <returns>
        /// The exernal system surrogate key, which uniquely identifies the server in the database.
        /// </returns>
        Guid InsertServer(Context context, CareFusion.Dispensing.Models.Server server);

        /// <summary>
        /// Updates an existing <see cref="CareFusion.Dispensing.Server"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="server">The server to update.</param>
        void UpdateServer(Context context, CareFusion.Dispensing.Models.Server server);

        /// <summary>
        /// Logically deletes an existing <see cref="CareFusion.Dispensing.Server"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="serverKey">The server surrogate key.</param>
        void DeleteServer(Context context, Guid serverKey);
    }

    #endregion

    public class ServerRepository : RepositoryBase, IServerRepository
    {
        static ServerRepository()
        {
            // Dapper custom mappings
            SqlMapper.SetTypeMap(
                typeof(CareFusion.Dispensing.Models.Server),
                new ColumnAttributeTypeMapper<CareFusion.Dispensing.Models.Server>());
        }

        IReadOnlyCollection<CareFusion.Dispensing.Models.Server> IServerRepository.GetServers(Filter<IEnumerable<Guid>> serverKeysFilter,
            Filter<string> serverNameFilter, Filter<bool> syncFilter)
        {
            List<CareFusion.Dispensing.Models.Server> servers = new List<CareFusion.Dispensing.Models.Server>();

            try
            {
                GuidKeyTable selectedKeys = new GuidKeyTable();
                if (serverKeysFilter.HasValue)
                {
                    IEnumerable<Guid> externalSystemKeys = serverKeysFilter.Value;
                    if (externalSystemKeys == null ||
                        !externalSystemKeys.Any())
                    {
                        return new List<CareFusion.Dispensing.Models.Server>(); // return empty results.
                    }

                    selectedKeys = new GuidKeyTable(externalSystemKeys.Distinct());
                }

                if (serverNameFilter.HasValue &&
                    serverNameFilter.Value == null)
                {
                    return new List<CareFusion.Dispensing.Models.Server>(); // return empty results.
                }

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    using (var multi = connectionScope.QueryMultiple(
                        "Core.bsp_GetServers",
                        new
                        {
                            SelectedKeys = selectedKeys.AsTableValuedParameter(),
                            ServerName = serverNameFilter.GetValueOrDefault(),
                            SyncFlag = syncFilter.HasValue ? syncFilter.Value : default(bool?)
                        },
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.StoredProcedure))
                    {
                        servers = multi.Read<CareFusion.Dispensing.Models.Server>().ToList();
                        
                        var associations = multi.Read();
                        foreach (var server in servers)
                        {
                            server.Associations = associations.Where(a => a.ServerKey == server.Key)
                                .Select(a => new ServerAssociation()
                                {
                                    FacilityName = (string)a.FacilityName,
                                    DispensingDeviceCount = (int)a.DispensingDeviceCount
                                }).ToList();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return servers;
        }

        CareFusion.Dispensing.Models.Server IServerRepository.GetServer(string serverName)
        {
            var servers =
                ((IServerRepository)this).GetServers(serverNameFilter: serverName);

            return servers.FirstOrDefault();
        }

        bool IServerRepository.ServerNameExists(string name, Guid? ignoreServerKey)
        {
            try
            {
                SqlBuilder query = new SqlBuilder();
                query.SELECT("TOP 1 ServerKey")
                    .FROM("Core.Server")
                    .WHERE("DeleteFlag = 0")
                    .WHERE("ServerName = @ServerName");

                if (ignoreServerKey != null)
                {
                    query.WHERE("ServerKey <> @ServerKey");
                }

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    return connectionScope.Query(
                        query.ToString(),
                        new
                        {
                            ServerName = name,
                            ServerKey = ignoreServerKey
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

            return false;
        }

        bool IServerRepository.ServerAddressExists(string address, Guid? ignoreServerKey)
        {
            try
            {
                SqlBuilder query = new SqlBuilder();
                query.SELECT("TOP 1 ServerKey")
                    .FROM("Core.Server")
                    .WHERE("DeleteFlag = 0")
                    .WHERE("ServerAddressValue = @Address");

                if (ignoreServerKey != null)
                {
                    query.WHERE("ServerKey <> @ServerKey");
                }

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    return connectionScope.Query(
                        query.ToString(),
                        new
                        {
                            Address = address,
                            ServerKey = ignoreServerKey
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

            return false;
        }

        CareFusion.Dispensing.Models.Server IServerRepository.GetServer(Guid serverKey)
        {
            var servers = ((IServerRepository)this).GetServers(new[] { serverKey });

            return servers.FirstOrDefault();
        }

        Guid IServerRepository.InsertServer(Context context, CareFusion.Dispensing.Models.Server server)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(server, "server");

            Guid? serverKey = null;

            try
            {

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                    new Dictionary<string, object>
                     {
                         {"@ServerAddressValue", server.Address},
                         {"@ServerName", server.Name},
                         {"@CoreFlag", server.Core},
                         {"@CreatedUTCDateTime", context.ActionUtcDateTime},
                         {"@CreatedLocalDateTime", context.ActionDateTime},
                         {"@LastModifiedActorKey", (Guid?)context.Actor}
                     });
                    parameters.Add("@ServerKey", dbType: DbType.Guid, direction: ParameterDirection.Output);

                    connectionScope.Connection.Execute(
                        "Core.usp_ServerInsert",
                        parameters,
                        null,
                        connectionScope.DefaultCommandTimeout,
                        CommandType.StoredProcedure);

                    serverKey = parameters.Get<Guid?>("@ServerKey");
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return serverKey.GetValueOrDefault();
        }

        void IServerRepository.UpdateServer(Context context, CareFusion.Dispensing.Models.Server server)
        {
            Guard.ArgumentNotNull(context, "context");
            Guard.ArgumentNotNull(server, "server");

            try
            {

                using (IConnectionScope connectionScope = ConnectionScopeFactory.Create())
                {
                    var parameters = new DynamicParameters(
                    new Dictionary<string, object>
                     {
                         {"@ServerAddressValue", server.Address},
                         {"@ServerName", server.Name},
                         {"@CoreFlag", server.Core},
                         {"@EndUTCDateTime", context.ActionUtcDateTime},
                         {"@EndLocalDateTime", context.ActionDateTime},
                         {"@LastModifiedActorKey", (Guid?)context.Actor},
                         {"@LastModifiedBinaryValue", new byte[] {0,0,0,0,0,0,0,0 }},
                         {"@ServerKey", server.Key},
                     });

                    connectionScope.Connection.Execute(
                        "Core.usp_ServerUpdate",
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

        void IServerRepository.DeleteServer(Context context, Guid serverKey)
        {
            Guard.ArgumentNotNull(context, "context");

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
                         {"@ServerKey", serverKey},
                     });

                    connectionScope.Connection.Execute(
                        "Core.usp_ServerDelete",
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
