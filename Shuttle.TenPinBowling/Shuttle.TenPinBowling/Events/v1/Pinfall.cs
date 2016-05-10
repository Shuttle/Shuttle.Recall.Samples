using System.Collections.Generic;

namespace Shuttle.TenPinBowling.Events.v1
{
	public class Pinfall
	{
		public Pinfall()
		{
			FrameBonusPins = new List<FrameBonus>();
		}

		public int Frame { get; set; }
		public int FrameRoll { get; set; }
		public int Pins { get; set; }
		public int OverallRoll { get; set; }
		public int Score { get; set; }
		public List<FrameBonus> FrameBonusPins { get; set; }
		public bool FrameFinished { get; set; }
		public bool Strike { get; set; }
		public bool Spare { get; set; }
		public bool Open { get; set; }
	}
}