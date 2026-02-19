using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Recall.SqlServer.EventProcessing;
using System.Data.Common;

namespace Orders.Data;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddOrderData()
        {
            services.AddKeyedScoped<DbConnection>("Orders", (sp, _) => new SqlConnection(sp.GetRequiredService<IConfiguration>().GetConnectionString("Orders") ?? throw new ApplicationException("A 'ConnectionString' with name 'Orders' is required which points to a Sql Server database that will contain the orders.")));

            services.AddDbContext<OrderDbContext>((sp, options) =>
            {
                var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("Orders") ?? throw new ApplicationException("A 'ConnectionString' with name 'Orders' is required which points to a Sql Server database that will contain the orders.");

                var dbConnection = sp.GetKeyedService<DbConnection>("Orders");

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