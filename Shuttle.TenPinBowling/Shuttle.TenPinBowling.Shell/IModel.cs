using System;
using System.Collections.Generic;

namespace Shuttle.TenPinBowling.Shell
{
	public interface IModel
	{
		event EventHandler<EventArgs> GameStarted;
		event EventHandler<GameAddedEventArgs> GameAdded;
		event EventHandler<EventArgs> FrameScored;

		bool HasGameStarted { get; }
	    string Bowler { get; }
        int Score { get; }
        int StandingPins { get; }

        IEnumerable<FrameScoreModel> FrameScores { get; }

	    void StartGame(string bowler);
	    void AddGame(Guid id, string bowler, DateTime dateStarted);
	    void AddFrameBonusScore(int frame, int pins);
	    void AddFrameScore(int frame, int roll, int pins, bool strike, bool spare, int standingPins);
	}
}