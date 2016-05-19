using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;

namespace Shuttle.TenPinBowling.Shell
{
    public class Model : IModel
    {
        private List<FrameScoreModel> _frameScores;
        public event EventHandler<EventArgs> GameStarted = delegate { };
        public event EventHandler<GameAddedEventArgs> GameAdded = delegate { };
        public event EventHandler<EventArgs> FrameScored = delegate { };
        public event EventHandler<EventArgs> StandingPinsSet = delegate { };

        public bool HasGameStarted { get; private set; }
        public string Bowler { get; private set; }
        public int Score { get; private set;  }
        public int StandingPins { get; private set; }

        public IEnumerable<FrameScoreModel> FrameScores
        {
            get { return new ReadOnlyCollection<FrameScoreModel>(_frameScores); }
        }

        public void StartGame(string bowler)
        {
            Bowler = bowler;
            Score = 0;

            _frameScores = new List<FrameScoreModel>();

            HasGameStarted = true;

            GameStarted.Invoke(this, new EventArgs());
        }

        public void AddGame(Guid id, string bowler, DateTime dateStarted)
        {
            GameAdded.Invoke(this, new GameAddedEventArgs(id, bowler, dateStarted));
        }

        public void AddFrameBonusScore(int frame, int pins)
        {
            var model = GetFrame(frame);

            if (model == null)
            {
                return;
            }

            model.AddScore(pins);

            Score = Score + pins;

            FrameScored.Invoke(this, new EventArgs());
        }

        private FrameScoreModel GetFrame(int frame)
        {
            return _frameScores.Find(candidate => candidate.Frame == frame);
        }

        public void AddFrameScore(int frame, int roll, int pins, bool strike, bool spare, int standingPins)
        {
            var model = GetFrame(frame);

            if (model == null)
            {
                _frameScores.Add(new FrameScoreModel(frame, pins, strike, spare));
            }
            else
            {
                if (roll == 2)
                {
                    model.SetRoll2Pins(pins, strike, spare);
                }
                else
                {
                    model.SetRoll3Pins(pins, strike, spare);
                }
            }

            Score = Score + pins;

            StandingPins = standingPins;

            FrameScored.Invoke(this, new EventArgs());
        }
    }
}