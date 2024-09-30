using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using CareFusion.Dispensing.Resources;

namespace CareFusion.Dispensing.Data
{
    public static class DispensingConnection
    {
        public const string ConnectionStringName = "DispensingDatabase";

        public static string GetConnectionString()
        {
            ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionStringName];
            if (settings == null)
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.CurrentCulture,
                    DataResources.DatabaseConnectionString_NotFound, ConnectionStringName));
            }

            return settings.ConnectionString;
        }

        public static SqlConnection CreateSqlConnection()
        {
            return new SqlConnection(GetConnectionString());
        }
    }
}
