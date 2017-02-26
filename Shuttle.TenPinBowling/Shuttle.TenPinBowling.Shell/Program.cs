using System;
using System.Windows.Forms;
using Castle.Windsor;
using Shuttle.Core.Castle;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.Recall;
using Shuttle.Recall.Sql;
using IScriptProvider = Shuttle.Recall.Sql.IScriptProvider;
using IScriptProviderConfiguration = Shuttle.Recall.Sql.IScriptProviderConfiguration;
using ScriptProvider = Shuttle.Recall.Sql.ScriptProvider;
using ScriptProviderConfiguration = Shuttle.Recall.Sql.ScriptProviderConfiguration;

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

			container.Register<IScriptProviderConfiguration, ScriptProviderConfiguration>();
			container.Register<IScriptProvider, ScriptProvider>();

			container.Register<IDatabaseContextCache, ThreadStaticDatabaseContextCache>();
			container.Register<IDatabaseContextFactory, DatabaseContextFactory>();
			container.Register<IDbConnectionFactory, DbConnectionFactory>();
			container.Register<IDbCommandFactory, DbCommandFactory>();
			container.Register<IDatabaseGateway, DatabaseGateway>();
			container.Register<IQueryMapper, QueryMapper>();
			container.Register<IProjectionRepository, ProjectionRepository>();
			container.Register<IProjectionQueryFactory, ProjectionQueryFactory>();
			container.Register<IPrimitiveEventRepository, PrimitiveEventRepository>();
			container.Register<IPrimitiveEventQueryFactory, PrimitiveEventQueryFactory>();

			container.Register<IProjectionConfiguration>(ProjectionSection.Configuration());
			container.Register<EventProcessingModule, EventProcessingModule>();

			container.Register<BowlingHandler, BowlingHandler>();
			container.Register<IBowlingQueryFactory, BowlingQueryFactory>();
			container.Register<IBowlingQuery, BowlingQuery>();

			EventStoreConfigurator.Configure(container);

			container.Resolve<EventProcessingModule>();

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