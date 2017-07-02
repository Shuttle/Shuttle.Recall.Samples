using System;
using System.Windows.Forms;
using Castle.Windsor;
using Shuttle.Core.Castle;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
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

			container.Register<BowlingHandler, BowlingHandler>();
			container.Register<IBowlingQueryFactory, BowlingQueryFactory>();
			container.Register<IBowlingQuery, BowlingQuery>();

			EventStore.Register(container);

			new MainPresenter(view,
				container.Resolve<IDatabaseContextFactory>(),
				EventStore.Create(container),
				container.Resolve<IBowlingQuery>());

			var processor = EventProcessor.Create(container);

			var projection = new Projection("Bowling");

			projection.AddEventHandler(container.Resolve<BowlingHandler>());

			processor.AddProjection(projection);

			processor.Start();

			Application.Run(view);

			processor.Dispose();
		}
	}
}