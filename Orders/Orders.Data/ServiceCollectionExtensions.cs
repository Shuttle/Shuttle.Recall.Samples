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
            services.AddDbContextFactory<OrderDbContext>((serviceProvider, dbContextFactoryBuilder) =>
            {
                dbContextFactoryBuilder.UseSqlServer(serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("Orders") ?? throw new ApplicationException("A 'ConnectionString' with name 'Orders' is required which points to a Sql Server database that will contain the orders."));
            });

            return services;
        }
    }
}