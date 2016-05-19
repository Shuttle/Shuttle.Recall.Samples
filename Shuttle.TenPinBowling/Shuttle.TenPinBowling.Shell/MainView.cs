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
            _presenter.Roll(int.Parse(((Button)sender).Text));
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
            model.FrameScored += FrameScored;
        }

        private void FrameScored(object sender, EventArgs e)
        {
            foreach (var model in _model.FrameScores)
            {
                var frame = GetFrame(model.Frame);

                frame.Roll1(model.Roll1PinsDisplay);

                if (model.Roll2Pins.HasValue)
                {
                    frame.Roll2(model.Roll2PinsDisplay);
                }

                if (model.Roll3Pins.HasValue)
                {
                    frame.Roll3(model.Roll3PinsDisplay);
                }

                frame.Score(model.Score);
            }

            ScoreDisplay.Text = _model.Score.ToString();

            ShowStandingPins();
        }

        private void ShowStandingPins()
        {
            HidePinButtons(_model.StandingPins);
        }

        private void HidePinButtons(int from)
        {
            for (int i = 1; i < 11; i++)
            {
                GetPinButton(i).Visible = (i <= from);
            }
        }

        private Button GetPinButton(int pin)
        {
            var name = string.Format("Pin{0}Button", pin);

            foreach (Control control in Controls)
            {
                if (control.Name.Equals(name))
                {
                    return (Button)control;
                }
            }

            throw new ApplicationException(string.Format("Could not find a pin button control for pin number '{0}'.", pin));
        }

        private void GameAdded(object sender, GameAddedEventArgs e)
        {
            Games.Items.Add(string.Format("{0} ({1})", e.Bowler, e.DateStarted.ToString("yyyy-MM-dd hh:mm")))
                .Tag = e.Id;
        }

        private void GameStarted(object sender, EventArgs e)
        {
            Bowler.Text = _model.Bowler;

            for (int i = 1; i < 11; i++)
            {
                GetFrame(i).Reset();
            }

            ShowStandingPins();
            ScoreDisplay.Text = string.Empty;
        }

        private Frame GetFrame(int frame)
        {
            var name = string.Concat("Frame", frame);

            foreach (Control control in Controls)
            {
                if (control.Name.Equals(name))
                {
                    return (Frame)control;
                }
            }

            throw new ApplicationException(string.Format("Could not find a frame control for frame number '{0}'.", frame));
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Message", MessageBoxButtons.OK);
        }

        private void StartGameButton_Click(object sender, System.EventArgs e)
        {
            _presenter.StartGame(Bowler.Text);
        }

        private void Games_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Games.SelectedItems.Count == 0)
            {
                return;
            }

            _presenter.SelectGame((Guid)Games.SelectedItems[0].Tag);
        }
    }
}