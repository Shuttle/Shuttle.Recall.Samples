using System.Collections.Generic;

namespace Shuttle.TenPinBowling.Events.v1
{
	public class Pinfall
	{
		public Pinfall()
		{
			BonusRolls = new List<BonusRoll>();
		}

		public int Frame { get; set; }
		public int FrameRoll { get; set; }
		public int Pins { get; set; }
		public int Roll { get; set; }
		public int Score { get; set; }
		public List<BonusRoll> BonusRolls { get; set; }
		public bool FrameFinished { get; set; }
		public bool Strike { get; set; }
		public bool Spare { get; set; }
		public bool Open { get; set; }
		public int StandingPins { get; set; }
		public bool BonusRoll { get; set; }
		public bool GameFinished { get; set; }
	}
}