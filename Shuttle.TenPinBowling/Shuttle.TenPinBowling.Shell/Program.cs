using System;
using System.IO;
using System.Windows.Forms;
using Castle.Windsor;
using log4net;
using Shuttle.Core.Container;
using Shuttle.Core.Castle;
using Shuttle.Core.Data;
using Shuttle.Core.Log4Net;
using Shuttle.Core.Logging;
using Shuttle.Recall;

namespace Shuttle.TenPinBowling.Shell
{
	internal static class Program
	{

		[STAThread]
		private static void Main()
		{
            Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(Program))));

            Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var view = new MainView();

			var container = new WindsorComponentContainer(new WindsorContainer());
            var resolver = (IComponentResolver)container;

			container.Register<BowlingHandler, BowlingHandler>();
			container.Register<IBowlingQueryFactory, BowlingQueryFactory>();
			container.Register<IBowlingQuery, BowlingQuery>();

			EventStore.Register(container);

			new MainPresenter(view,
				container.Resolve<IDatabaseContextFactory>(),
				EventStore.Create(container),
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
	}
}