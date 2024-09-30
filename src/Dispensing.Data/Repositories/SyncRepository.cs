using System;
using System.Data;
using System.Linq;
using CareFusion.Dispensing.Contracts;
using CareFusion.Dispensing.Data.Models;
using Dapper;
using Pyxis.Core.Data;

namespace CareFusion.Dispensing.Data.Repositories
{
    internal class SyncRepository : LinqBaseRepository, ISyncRepository
    {
        public DateTimePair GetLastServerCommunicationTime(Guid dispensingDeviceKey)
        {
            DateTimePair lastCommunication = null;

            try
            {
                var query = new SqlBuilder();
                query.SELECT("dd.LastServerCommunicationUTCDateTime")
                    ._("dd.LastServerCommunicationLocalDateTime")
                    .FROM("Strg.DispensingDevice dd")
                    .WHERE("dd.DispensingDeviceKey = @DispensingDeviceKey");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var result = connectionScope.QueryFirstOrDefault<LastServerCommunicationTimeResult>(
                         query.ToString(),
                         new
                         {
                             DispensingDeviceKey = dispensingDeviceKey,
                         },
                         commandTimeout: connectionScope.DefaultCommandTimeout,
                         commandType: CommandType.Text);

                    if (result != null)
                    {
                        lastCommunication = new DateTimePair();
                        lastCommunication.UtcDateTime = result.LastServerCommunicationUTCDateTime;
                        lastCommunication.LocalDateTime = result.LastServerCommunicationLocalDateTime;
                    }
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }

            return lastCommunication;
        }

        public bool DataUpgradeRequired()
        {
            try
            {
                var query = new SqlBuilder();
                query.SELECT("*")
                    .FROM("Sync.NewColumnList");

                using (var connectionScope = ConnectionScopeFactory.Create())
                {
                    var newColumns = connectionScope.Query(
                        query.ToString(),
                        commandTimeout: connectionScope.DefaultCommandTimeout,
                        commandType: CommandType.Text);
                    return newColumns.Any();
                }
            }
            catch (Exception e)
            {
                if (DataExceptionHandler.HandleException(e))
                    throw;
            }
            return false;
        }
    }
}
