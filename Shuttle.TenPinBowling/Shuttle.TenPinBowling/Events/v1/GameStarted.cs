using System;

namespace Shuttle.TenPinBowling.Events.v1
{
	public class GameStarted
	{
		public string Bowler { get; set; }
	    public DateTime StartDate { get; set; }
	}
}