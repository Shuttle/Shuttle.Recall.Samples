using System.ComponentModel;
using System.Windows.Forms;

namespace Shuttle.TenPinBowling.Shell
{
	public partial class Frame : UserControl
	{
		public Frame()
		{
			InitializeComponent();
		}

		[Description("Title")]
		public string Title
		{
			get { return TitleDisplay.Text; }
			set { TitleDisplay.Text = value; }
		}
	}
}