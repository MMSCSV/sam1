using System;
using System.Data;
using System.Data.SqlClient;
using Pyxis.Core.Data;

namespace CareFusion.Dispensing.Data.Repositories
{
    public static class SqlQueryNotification
    {
        public static void Start()
        {
            SqlDependency.Start(DispensingConnection.GetConnectionString());
        }

        public static void Stop()
        {
            SqlDependency.Stop(DispensingConnection.GetConnectionString());
        }

        public static void Register(string query, Action changeNotification)
        {
            try
            {
                using (SqlConnection connection = DispensingConnection.CreateSqlConnection())
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;

                    SqlDependency dependency = new SqlDependency(command);
                    dependency.OnChange += (sender, args) => changeNotification();

                    connection.Open();
                    command.ExecuteScalar();
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
