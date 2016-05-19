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

	    public void Reset()
	    {
	        Roll1Pins.Text = string.Empty;
	        Roll2Pins.Text = string.Empty;
	        Roll3Pins.Text = string.Empty;
	        Roll3Pins.Visible = false;

            ScoreDisplay.Text = string.Empty;
	    }

	    public void Roll1(string display)
	    {
	        Roll1Pins.Text = display;
	    }

	    public void Roll2(string display)
	    {
	        Roll2Pins.Text = display;
	    }

	    public void Roll3(string display)
	    {
	        Roll3Pins.Text = display;
	        Roll3Pins.Visible = !string.IsNullOrEmpty(display);
	    }

	    public void Score(int score)
	    {
	        ScoreDisplay.Text = score.ToString();
	    }
	}
}