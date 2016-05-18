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

        IEnumerable<FrameScoreModel> FrameScores { get; }

	    void OnGameStarted(string bowler);
	    void AddGame(Guid id, string bowler, DateTime dateStarted);
	    void AddFrameScore(int frame, int pins);
	    void AddFrameScore(int frame, int roll, int pins);
	}
}