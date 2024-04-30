using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shuttle.Core.Data;
using Shuttle.Recall;
using Shuttle.Recall.Sql.EventProcessing;
using Shuttle.Recall.Sql.Storage;

namespace Shuttle.TenPinBowling.Shell;

internal static class Program
{
    [STAThread]
    private static async Task Main()
    {
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);

        ApplicationConfiguration.Initialize();

        var view = new MainView();

        var services = new ServiceCollection()
            .AddSingleton<IConfiguration>(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build())
            .AddSingleton<IBowlingQueryFactory, BowlingQueryFactory>()
            .AddSingleton<IBowlingQuery, BowlingQuery>()
            .AddDataAccess(builder =>
            {
                builder.AddConnectionString("ShuttleProjection", "Microsoft.Data.SqlClient");
                builder.AddConnectionString("Shuttle", "Microsoft.Data.SqlClient");
            })
            .AddSqlEventStorage(builder =>
            {
                builder.Options.ConnectionStringName = "Shuttle";
            })
            .AddSqlEventProcessing(builder =>
            {
                builder.Options.ConnectionStringName = "ShuttleProjection";
            })
            .AddEventStore(builder =>
            {
                builder.AddEventHandler<BowlingHandler>("Bowling");
            });

        var serviceProvider = services.BuildServiceProvider();

        var hostedServices = serviceProvider.GetServices<IHostedService>().ToList();

        foreach (var hostedService in hostedServices)
        {
            await hostedService.StartAsync(CancellationToken.None);
        }

        _ = new MainPresenter(view,
            serviceProvider.GetRequiredService<IDatabaseContextFactory>(),
            serviceProvider.GetRequiredService<IEventStore>(),
            serviceProvider.GetRequiredService<IBowlingQuery>());

        Application.Run(view);

        foreach (var hostedService in hostedServices)
        {
            await hostedService.StopAsync(CancellationToken.None);
        }
    }
}