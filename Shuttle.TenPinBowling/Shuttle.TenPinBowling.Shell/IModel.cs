using System;

namespace Shuttle.TenPinBowling.Shell
{
	public interface IModel
	{
		event EventHandler<EventArgs> GameStarted;

		bool HasGameStarted { get; }
		void OnGameStarted(string bowler);
	}
}