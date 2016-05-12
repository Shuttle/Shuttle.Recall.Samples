using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Shuttle.Core.Infrastructure;
using Shuttle.TenPinBowling.Events.v1;

namespace Shuttle.TenPinBowling
{
	public class FrameBonusRoll
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

	public class Game
	{
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

			_bonusRoll = _bonusRoll || _frame == 10 && (strike || spare);

			var frameFinished = !IsLastFrame
				? (pins == 10) || IsSecondRoll
				: (_bonusRoll ? IsThirdRoll : IsSecondRoll);

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
				BonusRoll = _bonusRoll,
				Frame = _frame,
				FrameRoll = _frameRoll,
				Roll = _roll++
			};

			if (strike && !IsLastFrame)
			{
				pinfall.BonusRolls.Add(new BonusRoll
				{
					Frame = _frame,
					Rolls = 2
				});
			}

			if (spare && !IsLastFrame)
			{
				pinfall.BonusRolls.Add(new BonusRoll
				{
					Frame = _frame,
					Rolls = 1
				});
			}

			On(pinfall);

			pinfall.Score = Score();

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
			_frameRoll = pinfall.FrameRoll + 1;
			_standingPins = pinfall.StandingPins;
			_bonusRoll = pinfall.BonusRoll;
			_roll = pinfall.Roll + 1;

			_frameScore[_frame - 1] += pinfall.Pins;

			foreach (var frameBonusRoll in _frameBonusRolls)
			{
				if (!frameBonusRoll.ShouldAssignBonus)
				{
					continue;
				}

				_frameScore[frameBonusRoll.Frame - 1] += pinfall.Pins;

				frameBonusRoll.BonusAssigned();
			}

			foreach (var bonusRoll in pinfall.BonusRolls)
			{
				_frameBonusRolls.Add(new FrameBonusRoll(bonusRoll.Frame, bonusRoll.Rolls));
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