namespace Shuttle.TenPinBowling.Shell
{
    public class FrameScoreModel
    {
        public FrameScoreModel(int frame, int roll1Pins)
        {
            Frame = frame;
            Roll1Pins = roll1Pins;
            Score = roll1Pins;
        }

        public int Frame { get; private set; }
        public int Roll1Pins { get; private set; }
        public int? Roll2Pins { get; private set; }
        public int? Roll3Pins { get; private set; }
        public int Score { get; private set; }

        public void SetRoll2Pins(int pins)
        {
            if (Roll2Pins.HasValue)
            {
                return;
            }

            Roll2Pins = pins;

            AddScore(pins);
        }

        public void SetRoll3Pins(int pins)
        {
            if (Roll3Pins.HasValue)
            {
                return;
            }

            Roll3Pins = pins;

            AddScore(pins);
        }

        public void AddScore(int pins)
        {
            Score += pins;
        }
    }
}