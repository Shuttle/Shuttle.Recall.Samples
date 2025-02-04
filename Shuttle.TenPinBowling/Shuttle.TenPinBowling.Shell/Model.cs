using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Shuttle.TenPinBowling.Shell;

public class Model : IModel
{
    private List<FrameScoreModel> _frameScores = new();

    public event EventHandler<EventArgs> GameStarted = delegate
    {
    };

    public event EventHandler<GameAddedEventArgs> GameAdded = delegate
    {
    };

    public event EventHandler<EventArgs> FrameScored = delegate
    {
    };

    public bool HasGameStarted { get; private set; }
    public string Bowler { get; private set; } = string.Empty;
    public int Score { get; private set; }
    public int StandingPins { get; private set; }

    public IEnumerable<FrameScoreModel> FrameScores => new ReadOnlyCollection<FrameScoreModel>(_frameScores);

    public void StartGame(string bowler)
    {
        Bowler = bowler;
        Score = 0;
        StandingPins = 10;

        _frameScores = new();

        HasGameStarted = true;

        GameStarted.Invoke(this, EventArgs.Empty);
    }

    public void AddGame(Guid id, string bowler, DateTime dateStarted)
    {
        GameAdded.Invoke(this, new(id, bowler, dateStarted));
    }

    public void AddFrameBonusScore(int frame, int pins)
    {
        var model = GetFrame(frame);

        if (model == null)
        {
            return;
        }

        model.AddScore(pins);

        Score += pins;

        FrameScored.Invoke(this, EventArgs.Empty);
    }

    public void AddFrameScore(int frame, int roll, int pins, bool strike, bool spare, int standingPins)
    {
        var model = GetFrame(frame);

        if (model == null)
        {
            _frameScores.Add(new(frame, pins, strike, spare));
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

        Score += pins;

        StandingPins = standingPins;

        FrameScored.Invoke(this, EventArgs.Empty);
    }

    private FrameScoreModel? GetFrame(int frame)
    {
        return _frameScores.Find(candidate => candidate.Frame == frame);
    }
}