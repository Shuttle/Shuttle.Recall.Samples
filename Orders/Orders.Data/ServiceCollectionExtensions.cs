using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Orders.Data;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddOrderData()
        {
            services.AddDbContext<OrderDbContext>((serviceProvider, options) =>
            {
                var connectionString = serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("Orders") ?? throw new ApplicationException("A 'ConnectionString' with name 'Orders' is required which points to a Sql Server database that will contain the orders.");

                options.UseSqlServer(connectionString);
            });

            return services;
        }
    }
}