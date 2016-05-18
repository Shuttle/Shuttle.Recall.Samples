using System.Collections.Generic;

namespace Shuttle.TenPinBowling.Events.v1
{
	public class Pinfall
	{
	    public Pinfall()
	    {
            FrameBonuses = new List<int>();
	    }

	    public int Frame { get; set; }
		public int FrameRoll { get; set; }
		public int Pins { get; set; }
		public int Roll { get; set; }
		public int Score { get; set; }
		public int BonusRolls { get; set; }
		public bool FrameFinished { get; set; }
		public bool Strike { get; set; }
		public bool Spare { get; set; }
		public bool Open { get; set; }
		public int StandingPins { get; set; }
		public bool GameFinished { get; set; }
	    public List<int> FrameBonuses { get; set; }
	}
}