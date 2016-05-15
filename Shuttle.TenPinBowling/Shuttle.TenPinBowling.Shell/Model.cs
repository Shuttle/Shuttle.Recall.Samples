using System;

namespace Shuttle.TenPinBowling.Shell
{
	public class Model : IModel
	{
	    public event EventHandler<EventArgs> GameStarted = delegate { };
	    public event EventHandler<GameAddedEventArgs> GameAdded = delegate { };

	    public bool HasGameStarted { get; private set; }
		public string Bowler { get; private set; }

		public void OnGameStarted(string bowler)
		{
			Bowler = bowler;
			HasGameStarted = true;

            GameStarted.Invoke(this, new EventArgs());
		}

	    public void OnGameAdded(Guid id, string bowler, DateTime dateStarted)
	    {
	        GameAdded.Invoke(this, new GameAddedEventArgs(id, bowler, dateStarted));
	    }
	}
}