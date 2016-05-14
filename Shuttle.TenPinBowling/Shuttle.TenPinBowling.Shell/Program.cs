using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;
using Shuttle.Recall.SqlServer;

namespace Shuttle.TenPinBowling.Shell
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var view = new MainView();

			var presenter = new MainPresenter(view, new EventStore(new DefaultSerializer(), new DatabaseGateway(), new EventStoreQueryFactory()));

			Application.Run(view);
		}
	}
}
