using System.Windows.Forms;

namespace Shuttle.TenPinBowling.Shell
{
	public partial class MainView : Form, IMainView
	{
		public MainView()
		{
			InitializeComponent();
		}

		private void PinKnockDownButton_Click(object sender, System.EventArgs e)
		{
			MessageBox.Show(((Button) sender).Text);
		}
	}
}