using System;
using System.Windows.Forms;
using Castle.Windsor;
using Shuttle.Core.Container;
using Shuttle.Core.Castle;
using Shuttle.Core.Data;
using Shuttle.Recall;

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

			var processor = EventProcessor.Create(container);

            using (container.Resolve<IDatabaseContextFactory>().Create("ShuttleProjection"))
            {
                resolver.AddEventHandler<BowlingHandler>("Bowling");
            }

            processor.Start();

			Application.Run(view);

			processor.Dispose();
		}
	}
}