using System;
using System.Windows.Forms;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.Recall;
using Shuttle.Recall.SqlServer;

namespace Shuttle.TenPinBowling.Shell
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var view = new MainView();

            var databaseContextFactory = new DatabaseContextFactory(new DbConnectionFactory(),
                new DbCommandFactory(), new ThreadStaticDatabaseContextCache());

            var databaseGateway = new DatabaseGateway();

            new MainPresenter(view,
                databaseContextFactory,
                new EventStore(new DefaultSerializer(), databaseGateway, new EventStoreQueryFactory()),
                new BowlingQuery(databaseGateway, new BowlingQueryFactory()));

            var eventProcessor = EventProcessor.Create(c =>
                c.ProjectionService(new ProjectionService(new DefaultSerializer(), ProjectionSection.Configuration(),
                    databaseContextFactory, databaseGateway, new ProjectionQueryFactory())));

            var eventProjection = new EventProjection("Bowling");

            eventProjection.AddEventHandler(new BowlingHandler(databaseGateway,
                new BowlingQueryFactory()));

            eventProcessor.AddEventProjection(eventProjection);

            eventProcessor.Start();

            Application.Run(view);

            eventProcessor.Dispose();
        }
    }
}