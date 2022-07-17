using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Windows.Forms;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Data;
using Shuttle.Core.Log4Net;
using Shuttle.Core.Logging;
using Shuttle.Recall;
using Shuttle.Recall.Sql.EventProcessing;
using Shuttle.Recall.Sql.Storage;

namespace Shuttle.TenPinBowling.Shell
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance); 
            
            Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Program))));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var view = new MainView();

            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

            services.AddSingleton<IBowlingQueryFactory, BowlingQueryFactory>();
            services.AddSingleton<IBowlingQuery, BowlingQuery>();

            services.AddDataAccess(builder =>
            {
                builder.AddConnectionString("ShuttleProjection", "System.Data.SqlClient");
                builder.AddConnectionString("Shuttle", "System.Data.SqlClient");
            });

            services.AddSqlEventStorage();
            services.AddSqlEventProcessing(builder =>
            {
                builder.Options.EventProjectionConnectionStringName = "ShuttleProjection";
                builder.Options.EventStoreConnectionStringName = "Shuttle";
            });

            services.AddEventStore(builder =>
            {
                builder.AddEventHandler<BowlingHandler>("Bowling");
            });

            var serviceProvider = services.BuildServiceProvider();

            _ = new MainPresenter(view,
                serviceProvider.GetRequiredService<IDatabaseContextFactory>(),
                serviceProvider.GetRequiredService<IEventStore>(),
                serviceProvider.GetRequiredService<IBowlingQuery>());

            var processor = serviceProvider.GetRequiredService<IEventProcessor>();

            processor.Start();

            Application.Run(view);

            processor.Dispose();
        }
    }
}