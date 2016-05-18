using System;
using System.Collections.Generic;
using System.Linq;
using Shuttle.Core.Infrastructure;
using Shuttle.TenPinBowling.Events.v1;

namespace Shuttle.TenPinBowling
{
    public class FrameBonusRoll
    {
        private int _rolls;

        public FrameBonusRoll(int frame, int rolls)
        {
            Frame = frame;
            _rolls = rolls;
        }

        public int Frame { get; private set; }

        public bool ShouldAssignBonus
        {
            get { return _rolls > 0; }
        }

        public void BonusAssigned()
        {
            _rolls--;
        }
    }

    public class Game
    {
        private readonly List<FrameBonusRoll> _frameBonusRolls = new List<FrameBonusRoll>();

        private readonly List<int> _frameScore = new List<int>();
        private int _frame = 1;
        private int _frameRoll = 1;
        private bool _gameFinished;

        private GameStarted _gameStarted;
        private int _roll = 1;
        private int _standingPins;

        public Game()
            : this(Guid.NewGuid())
        {
        }

        public Game(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; private set; }

        private bool HasGameStarted
        {
            get { return _gameStarted != null; }
        }

        private bool IsFirstRoll
        {
            get { return _frameRoll == 1; }
        }

        private bool IsSecondRoll
        {
            get { return _frameRoll == 2; }
        }

        private bool IsThirdRoll
        {
            get { return _frameRoll == 3; }
        }

        private bool IsLastFrame
        {
            get { return _frame == 10; }
        }

        public GameStarted Start(string bowler)
        {
            return On(new GameStarted
            {
                Bowler = bowler,
                StartDate = DateTime.Now
            });
        }

        public GameStarted On(GameStarted gameStarted)
        {
            Guard.AgainstNull(gameStarted, "gameStarted");

            if (HasGameStarted)
            {
                throw new ApplicationException(string.Format("The game has already started with bowler '{0}'.",
                    _gameStarted.Bowler));
            }

            _gameStarted = gameStarted;
            _standingPins = 10;

            for (var i = 0; i < 10; i++)
            {
                _frameScore.Add(0);
            }

            return gameStarted;
        }

        public Pinfall Roll(int pins)
        {
            if (!HasGameStarted)
            {
                throw new ApplicationException("The game has not yet started.");
            }

            if (_gameFinished)
            {
                throw new ApplicationException("The game has finished.");
            }

            if (pins > _standingPins)
            {
                throw new ApplicationException(string.Format("You cannot knock over more than the '{0}' standing pins.",
                    _standingPins));
            }

            var strike = pins == 10 && (IsLastFrame || IsFirstRoll);
            var spare = !strike && !IsFirstRoll && pins == _standingPins;

            var frameFinished = !IsLastFrame
                ? (pins == 10) || IsSecondRoll
                : IsThirdRoll || (IsSecondRoll && !(spare || strike));

            _gameFinished = frameFinished && IsLastFrame;
            _standingPins -= pins;

            var pinfall = new Pinfall
            {
                Pins = pins,
                StandingPins = _standingPins,
                Strike = strike,
                Spare = spare,
                Open = !IsFirstRoll && pins < _standingPins,
                FrameFinished = frameFinished,
                GameFinished = _gameFinished,
                BonusRolls = IsLastFrame ? 0 : strike ? 2 : spare ? 1 : 0,
                Frame = _frame,
                FrameRoll = _frameRoll,
                Roll = _roll++
            };

            foreach (var frameBonusRoll in _frameBonusRolls)
            {
                if (!frameBonusRoll.ShouldAssignBonus)
                {
                    continue;
                }

                pinfall.FrameBonuses.Add(frameBonusRoll.Frame);
            }

            On(pinfall);

            pinfall.Score = Score();

            return pinfall;
        }

        public int Score()
        {
            return _frameScore.Sum();
        }

        public void On(Pinfall pinfall)
        {
            Guard.AgainstNull(pinfall, "pinfall");

            _frame = pinfall.Frame;
            _frameRoll = pinfall.FrameRoll + 1;
            _standingPins = pinfall.StandingPins;
            _roll = pinfall.Roll + 1;

            _frameScore[_frame - 1] += pinfall.Pins;

            if (pinfall.BonusRolls > 0)
            {
                _frameBonusRolls.Add(new FrameBonusRoll(pinfall.Frame, pinfall.BonusRolls));
            }

            foreach (var frameBonus in pinfall.FrameBonuses)
            {
                _frameScore[frameBonus - 1] += pinfall.Pins;

                _frameBonusRolls.Find(candidate => candidate.Frame == frameBonus).BonusAssigned();
            }

            if (pinfall.FrameFinished)
            {
                _frame++;
                _gameFinished = pinfall.Frame == 10;
                _frameRoll = 1;
            }

            if (pinfall.FrameFinished || pinfall.Strike || pinfall.Spare)
            {
                _standingPins = 10;
            }
        }
    }
}