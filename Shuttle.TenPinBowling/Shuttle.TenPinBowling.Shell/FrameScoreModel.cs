namespace Shuttle.TenPinBowling.Shell
{
    public class FrameScoreModel
    {
        public FrameScoreModel(int frame, int roll1Pins, bool strike, bool spare)
        {
            Frame = frame;
            Roll1Pins = roll1Pins;
            Score = roll1Pins;

            Roll1PinsDisplay = GetPinDisplay(roll1Pins, strike, spare);
        }

        private string GetPinDisplay(int pins, bool strike, bool spare)
        {
            return strike
                ? "X"
                : spare
                    ? "/"
                    : pins.ToString();
        }

        public int Frame { get; private set; }
        public int Roll1Pins { get; private set; }
        public int? Roll2Pins { get; private set; }
        public int? Roll3Pins { get; private set; }
        public string Roll1PinsDisplay { get; private set; }
        public string Roll2PinsDisplay { get; private set; }
        public string Roll3PinsDisplay { get; private set; }
        public int Score { get; private set; }

        public void SetRoll2Pins(int pins, bool strike, bool spare)
        {
            if (Roll2Pins.HasValue)
            {
                return;
            }

            Roll2Pins = pins;

            Roll2PinsDisplay = GetPinDisplay(pins, strike, spare);

            AddScore(pins);
        }

        public void SetRoll3Pins(int pins, bool strike, bool spare)
        {
            if (Roll3Pins.HasValue)
            {
                return;
            }

            Roll3Pins = pins;

            Roll3PinsDisplay = GetPinDisplay(pins, strike, spare);

            AddScore(pins);
        }

        public void AddScore(int pins)
        {
            Score += pins;
        }
    }
}