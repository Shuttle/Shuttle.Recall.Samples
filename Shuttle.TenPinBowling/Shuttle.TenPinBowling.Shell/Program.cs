using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Windows.Forms;
using Castle.Windsor;
using log4net;
using Shuttle.Core.Castle;
using Shuttle.Core.Container;
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

            var container = new WindsorComponentContainer(new WindsorContainer());
            var resolver = (IComponentResolver)container;

            container.Register<BowlingHandler, BowlingHandler>();
            container.Register<IBowlingQueryFactory, BowlingQueryFactory>();
            container.Register<IBowlingQuery, BowlingQuery>();

            container.RegisterDataAccess();
            container.RegisterEventStore();
            container.RegisterEventStoreStorage();
            container.RegisterEventProcessing();

            container.Resolve<EventProcessingModule>();

            _ = new MainPresenter(view,
                container.Resolve<IDatabaseContextFactory>(),
                container.Resolve<IEventStore>(),
                container.Resolve<IBowlingQuery>());

            var processor = container.Resolve<IEventProcessor>();

            using (container.Resolve<IDatabaseContextFactory>().Create("ShuttleProjection"))
            {
                processor.AddProjection("Bowling");

                resolver.AddEventHandler<BowlingHandler>("Bowling");
            }

            processor.Start();

            Application.Run(view);

            processor.Dispose();
        }

        //[STAThread]
        //static void Main()
        //{
        //    Application.SetHighDpiMode(HighDpiMode.SystemAware);
        //    Application.EnableVisualStyles();
        //    Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new Form1());
        //}
    }
}