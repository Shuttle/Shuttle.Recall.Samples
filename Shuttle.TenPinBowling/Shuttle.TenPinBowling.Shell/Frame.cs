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
	        ScoreDisplay.Text = string.Empty;
	    }

	    public void Roll1(int pins)
	    {
	        Roll1Pins.Text = pins.ToString();
	    }

	    public void Roll2(int pins)
	    {
	        Roll2Pins.Text = pins.ToString();
	    }

	    public void Roll3(int pins)
	    {
	        Roll3Pins.Text = pins.ToString();
	        Roll3Pins.Visible = true;
	    }

	    public void Score(int score)
	    {
	        ScoreDisplay.Text = score.ToString();
	    }
	}
}