using System.Data.Common;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shuttle.Core.Data;
using Shuttle.Core.Data.Logging;
using Shuttle.Recall;
using Shuttle.Recall.Logging;
using Shuttle.Recall.Sql.EventProcessing;
using Shuttle.Recall.Sql.Storage;

namespace Shuttle.TenPinBowling.Projection;

internal class Program
{
    private static async Task Main(string[] args)
    {
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services
                    .AddSingleton<IConfiguration>(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build())
                    .AddSingleton<IBowlingQueryFactory, BowlingQueryFactory>()
                    .AddSingleton<IBowlingQuery, BowlingQuery>()
                    .AddLogging(configure =>
                    {
                        configure.AddConsole();
                    })
                    .AddDataAccess(builder =>
                    {
                        builder.AddConnectionString("ShuttleProjection", "Microsoft.Data.SqlClient");
                        builder.AddConnectionString("Shuttle", "Microsoft.Data.SqlClient");
                    })
                    .AddDataAccessLogging()
                    .AddSqlEventStorage(builder =>
                    {
                        builder.Options.ConnectionStringName = "Shuttle";
                        builder.Options.Schema = "TenPinBowling";

                        builder.UseSqlServer();
                    })
                    .AddSqlEventProcessing(builder =>
                    {
                        builder.Options.ConnectionStringName = "ShuttleProjection";
                        builder.Options.Schema = "TenPinBowling";

                        builder.UseSqlServer();
                    })
                    .AddEventStore(builder =>
                    {
                        builder.AddProjection("Bowling").AddEventHandler<BowlingHandler>();

                        builder.Options.ProjectionThreadCount = 1;
                    })
                    .AddEventStoreLogging();
            })
            .Build();

        var databaseContextFactory = host.Services.GetRequiredService<IDatabaseContextFactory>();

        var cancellationTokenSource = new CancellationTokenSource();

        Console.CancelKeyPress += delegate
        {
            cancellationTokenSource.Cancel();
        };

        if (!databaseContextFactory.IsAvailable("Shuttle", cancellationTokenSource.Token))
        {
            throw new ApplicationException("[connection failure] : name = 'Shuttle'");
        }


        if (!databaseContextFactory.IsAvailable("ShuttleProjection", cancellationTokenSource.Token))
        {
            throw new ApplicationException("[connection failure] : name = 'ShuttleProjection'");
        }

        if (cancellationTokenSource.Token.IsCancellationRequested)
        {
            return;
        }

        await host.RunAsync(cancellationTokenSource.Token);
    }
}