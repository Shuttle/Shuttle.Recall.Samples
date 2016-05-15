using System;
using System.Windows.Forms;

namespace Shuttle.TenPinBowling.Shell
{
	public partial class MainView : Form, IMainView
	{
		private IMainPresenter _presenter;
		private IModel _model;

		public MainView()
		{
			InitializeComponent();
		}

		private void PinKnockDownButton_Click(object sender, System.EventArgs e)
		{
			_presenter.Roll(int.Parse(((Button) sender).Text));
		}

		public void Assign(IMainPresenter presenter, IModel model)
		{
			_presenter = presenter;
			_model = model;

		    WireEvents(model);
		}

	    private void WireEvents(IModel model)
	    {
	        model.GameStarted += GameStarted;
	        model.GameAdded += GameAdded;
	    }

	    private void GameAdded(object sender, GameAddedEventArgs e)
	    {
	        Games.Items.Add(string.Format("{0} ({1})", e.Bowler, e.DateStarted.ToString("yyyy-MM-dd hh:mm")));
	    }

	    private void GameStarted(object sender, EventArgs e)
	    {
	        Bowler.Text = _model.Bowler;
	    }

	    public void ShowMessage(string message)
		{
			MessageBox.Show(message, "Message", MessageBoxButtons.OK);
		}

		private void StartGameButton_Click(object sender, System.EventArgs e)
		{
			_presenter.StartGame(Bowler.Text);
		}
	}
}