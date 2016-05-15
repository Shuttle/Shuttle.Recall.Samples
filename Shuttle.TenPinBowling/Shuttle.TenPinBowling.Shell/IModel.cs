using System;

namespace Shuttle.TenPinBowling.Shell
{
	public interface IModel
	{
		event EventHandler<EventArgs> GameStarted;
		event EventHandler<GameAddedEventArgs> GameAdded;

		bool HasGameStarted { get; }
	    string Bowler { get; }

	    void OnGameStarted(string bowler);
	    void OnGameAdded(Guid id, string bowler, DateTime dateStarted);
	}
}