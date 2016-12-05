using System;
using System.Configuration;
using System.Data.Common;

namespace MapperOrm.DBContext
{
    class DataSourceProviderFactory
    {
        static DbConnection CreateDbConnection(string connectionString, string providerName)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("connectionString is null or whitespace");
            }
            DbConnection connection;
            DbProviderFactory factory;
            try
            {
                factory = DbProviderFactories.GetFactory(providerName);
                connection = factory.CreateConnection();
                if (connection != null) connection.ConnectionString = connectionString;
            }
            catch (ArgumentException)
            {
                try
                {
                    factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
                    connection = factory.CreateConnection();
                    if (connection != null)
                    {
                        connection.ConnectionString = connectionString;
                    }
                }
                catch (Exception)
                {
                    throw new Exception("DB connection has been failed.");
                }
            }


            return connection;
        }

        public static IDataSourceProvider Create(string connectionString)
        {
            var settings = ConfigurationManager.ConnectionStrings[connectionString];

            var dbConn = CreateDbConnection(settings.ConnectionString, settings.ProviderName);
            return new DbProvider(dbConn);
        }

        public static IDataSourceProvider CreateByDefaultDataProvider(string connectionString)
        {
            var dbConn = CreateDbConnection(connectionString, string.Empty);
            return new DbProvider(dbConn);
        }

    }
}