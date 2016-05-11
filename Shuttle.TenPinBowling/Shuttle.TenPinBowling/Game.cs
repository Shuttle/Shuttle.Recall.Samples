using System;
using System.Collections.Generic;
using System.Linq;
using Shuttle.Core.Infrastructure;
using Shuttle.TenPinBowling.Events.v1;

namespace Shuttle.TenPinBowling
{
	public class Game
	{
		private class FrameBonusRoll
		{
			private int _rolls;
			public int Frame { get; private set; }

			public FrameBonusRoll(int frame, int rolls)
			{
				Frame = frame;
				_rolls = rolls;
			}

			public void BonusAssigned()
			{
				_rolls--;
			}

			public bool ShouldAssignBonus
			{
				get { return _rolls > 0; }
			}
		}

		private int _roll = 1;
		private int _frame = 1;
		private int _frameRoll = 1;
		private int _standingPins;

		private readonly List<int> _frameScore = new List<int>();
		private readonly List<FrameBonusRoll> _frameBonusRolls = new List<FrameBonusRoll>();

		private GameStarted _gameStarted;
		private bool _gameFinished;
		private bool _bonusRoll;

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

		public GameStarted Start(string bowler)
		{
			return On(new GameStarted
			{
				Bowler = bowler
			});
		}

		public GameStarted On(GameStarted gameStarted)
		{
			Guard.AgainstNull(gameStarted, "gameStarted");

			if (HasGameStarted)
			{
				throw new ApplicationException(string.Format("The game has already started with bowler '{0}'.", _gameStarted.Bowler));
			}

			_gameStarted = gameStarted;
			_standingPins = 10;

			for (int i = 0; i < 10; i++)
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
				: (_bonusRoll ? IsThirdRoll : IsSecondRoll);

			_gameFinished = frameFinished && IsLastFrame;
			_bonusRoll = _frame == 10 && (strike || spare);
			_standingPins -= pins;

			_frameScore[_frame - 1] += pins;

			var pinfall = new Pinfall
			{
				Pins = pins,
				StandingPins = _standingPins,
				Strike = strike,
				Spare = spare,
				Open = !IsFirstRoll && pins < _standingPins,
				FrameFinished = frameFinished,
				GameFinished = _gameFinished,
				BonusRoll = _bonusRoll,
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

				pinfall.BonusFrames.Add(frameBonusRoll.Frame);

				_frameScore[frameBonusRoll.Frame - 1] += pins;

				frameBonusRoll.BonusAssigned();
			}

			if (frameFinished || strike || spare)
			{
				_standingPins = 10;
			}

			if (strike && IsFirstRoll && !IsLastFrame)
			{
				_frameBonusRolls.Add(new FrameBonusRoll(_frame, 2));
			}

			if (spare)
			{
				_frameBonusRolls.Add(new FrameBonusRoll(_frame, 1));
			}

			pinfall.Score = Score();

			if (frameFinished)
			{
				_frame++;
				_frameRoll = 1;
			}
			else
			{
				_frameRoll++;
			}

			return pinfall;
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

		public int Score()
		{
			return _frameScore.Sum();
		}

		public void On(Pinfall pinfall)
		{
			Guard.AgainstNull(pinfall, "pinfall");

			_frame = pinfall.Frame;
			_frameRoll = pinfall.FrameRoll;
			_standingPins = pinfall.StandingPins;
			_gameFinished = pinfall.FrameFinished && pinfall.Frame == 10;
			_bonusRoll = pinfall.BonusRoll;

			_frameScore[_frame - 1] += pinfall.Pins;

			foreach (var frame in pinfall.BonusFrames)
			{
				_frameScore[frame - 1] += pinfall.Pins;
			}
		}
	}
}