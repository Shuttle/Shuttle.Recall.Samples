using System;
using System.Collections.Generic;

namespace Shuttle.TenPinBowling.Shell;

public interface IModel
{
    string Bowler { get; }

    IEnumerable<FrameScoreModel> FrameScores { get; }

    bool HasGameStarted { get; }
    int Score { get; }
    int StandingPins { get; }
    void AddFrameBonusScore(int frame, int pins);
    void AddFrameScore(int frame, int roll, int pins, bool strike, bool spare, int standingPins);
    void AddGame(Guid id, string bowler, DateTime dateStarted);
    event EventHandler<EventArgs> FrameScored;
    event EventHandler<GameAddedEventArgs> GameAdded;
    event EventHandler<EventArgs> GameStarted;

    void StartGame(string bowler);
}