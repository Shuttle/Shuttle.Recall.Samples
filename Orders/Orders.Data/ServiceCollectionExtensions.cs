using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shuttle.Recall.SqlServer.EventProcessing;
using Shuttle.Recall.SqlServer.Storage;
using System.Data.Common;
using Resources = Shuttle.Recall.SqlServer.Storage.Resources;

namespace Orders.Data;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddOrderData()
        {
            services.AddKeyedScoped<DbConnection>("OrdersDbConnection", (serviceProvider, _) =>
                new SqlConnection(serviceProvider.GetRequiredService<IOptions<SqlServerStorageOptions>>().Value.ConnectionString));

            services.AddDbContext<OrderDbContext>((sp, options) =>
            {
                var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("Orders") ?? throw new ApplicationException("A 'ConnectionString' with name 'Orders' is required which points to a Sql Server database that will contain the orders.");

                var dbConnection = sp.GetKeyedService<DbConnection>("OrdersDbConnection");

                if (dbConnection != null)
                {
                    var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

                    if (!dbConnection.Database.Equals(sqlConnectionStringBuilder.InitialCatalog, StringComparison.InvariantCultureIgnoreCase) ||
                        !dbConnection.DataSource.Equals(sqlConnectionStringBuilder.DataSource, StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new ApplicationException(Resources.DbConnectionException);
                    }

                    options.UseSqlServer(dbConnection);
                }
                else
                {
                    options.UseSqlServer(connectionString);
                }
            });

            return services;
        }
    }
}