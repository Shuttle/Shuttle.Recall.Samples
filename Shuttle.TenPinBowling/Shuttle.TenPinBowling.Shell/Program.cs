using System;
using System.Data.Common;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Data;
using Shuttle.Recall;
using Shuttle.Recall.Sql.Storage;

namespace Shuttle.TenPinBowling.Shell;

internal static class Program
{
    [STAThread]
    private static void Main()
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
            .AddEventStore();

        var serviceProvider = services.BuildServiceProvider();

        _ = new MainPresenter(view,
            serviceProvider.GetRequiredService<IDatabaseContextFactory>(),
            serviceProvider.GetRequiredService<IEventStore>(),
            serviceProvider.GetRequiredService<IBowlingQuery>());

        Application.Run(view);
    }
}