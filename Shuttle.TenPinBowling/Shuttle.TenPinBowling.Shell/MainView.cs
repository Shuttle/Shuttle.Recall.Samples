using System.Windows.Forms;

namespace Shuttle.TenPinBowling.Shell
{
	public partial class MainView : Form, IMainView
	{
		private MainPresenter _presenter;
		private IModel _model;

		public MainView()
		{
			InitializeComponent();
		}

		private void PinKnockDownButton_Click(object sender, System.EventArgs e)
		{
			_presenter.Roll(int.Parse(((Button) sender).Text));
		}

		public void Assign(MainPresenter presenter, IModel model)
		{
			_presenter = presenter;
			_model = model;
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