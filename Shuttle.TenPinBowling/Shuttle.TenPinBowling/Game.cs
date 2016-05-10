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

		private int _frame = 1;
		private int _frameRoll;
		private int _standingPins;
		private readonly List<int> _rolls = new List<int>();

		private List<int> _frameScore = new List<int>(10);
		private List<FrameBonusRoll> _frameBonusRolls = new List<FrameBonusRoll>();


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
				throw new ApplicationException(string.Format("You cannot knock over more than the '{0}' standing pins.", _standingPins));
			}

			_frameRoll++;
			_rolls.Add(pins);

			var pinfall = new Pinfall
			{
				Pins = pins,
				Strike = (pins == 10 && (IsLastFrame || IsFirstRoll)),
				Spare = !IsFirstRoll && pins == _standingPins,
				Open = !IsFirstRoll && pins < _standingPins,
				FrameFinished = !IsLastFrame
					? (pins == 10) || IsSecondRoll
					: (_bonusRoll ? IsThirdRoll : IsSecondRoll),
				Score = TallyScore(),
				Frame = _frame,
				FrameRoll = _frameRoll,
				OverallRoll = _rolls.Count
			};

			foreach (var frameBonusRoll in _frameBonusRolls)
			{
				if (!frameBonusRoll.ShouldAssignBonus)
				{
					continue;
				}

				pinfall.FrameBonusPins.Add(new FrameBonus
				{
					Frame = frameBonusRoll.Frame,
					Pins = pins
				});

				frameBonusRoll.BonusAssigned();
			}

			return On(pinfall);
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
			get { return _frame != 10; }
		}

		private int TallyScore()
		{
			return _frameScore.Sum();
		}

		public Pinfall On(Pinfall pinfall)
		{
			Guard.AgainstNull(pinfall, "pinfall");

			_frame = pinfall.FrameFinished ? pinfall.Frame + 1 : pinfall.Frame;
			_frameRoll = pinfall.FrameFinished ? 0 : pinfall.FrameRoll;
			_standingPins = pinfall.FrameFinished ? 10 : 10 - pinfall.Pins;
			_gameFinished = pinfall.FrameFinished && pinfall.Frame == 10;

			return pinfall;
		}
	}
}