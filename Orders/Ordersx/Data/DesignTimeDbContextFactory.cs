using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Orders.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OrderDbContext>
{
    public OrderDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<DesignTimeDbContextFactory>(true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();

        optionsBuilder.UseSqlServer(configuration.GetConnectionString("Orders") ?? throw new ApplicationException("A 'ConnectionString' with name 'Order' is required which points to a Sql Server database that will contain the orders."));

        return new(optionsBuilder.Options);
    }
}