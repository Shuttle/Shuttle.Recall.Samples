using System;
using System.Collections.Generic;
using System.Linq;
using Shuttle.Core.Contract;
using Shuttle.TenPinBowling.Events.v1;

namespace Shuttle.TenPinBowling;

public class FrameBonusRoll
{
    private int _rolls;

    public FrameBonusRoll(int frame, int rolls)
    {
        Frame = frame;
        _rolls = rolls;
    }

    public int Frame { get; }

    public bool ShouldAssignBonus => _rolls > 0;

    public void BonusAssigned()
    {
        _rolls--;
    }
}

public class Game
{
    private readonly List<FrameBonusRoll> _frameBonusRolls = new();

    private readonly List<int> _frameScore = new();
    private int _frame = 1;
    private int _frameRoll = 1;

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

    public bool Finished { get; private set; }

    private bool HasGameStarted => _gameStarted != null;

    public Guid Id { get; }

    private bool IsFirstRoll => _frameRoll == 1;

    private bool IsLastFrame => _frame == 10;

    private bool IsSecondRoll => _frameRoll == 2;

    private bool IsThirdRoll => _frameRoll == 3;

    private GameStarted On(GameStarted gameStarted)
    {
        Guard.AgainstNull(gameStarted);

        if (HasGameStarted)
        {
            throw new ApplicationException($"The game has already started with bowler '{_gameStarted.Bowler}'.");
        }

        _gameStarted = gameStarted;
        _standingPins = 10;

        for (var i = 0; i < 10; i++)
        {
            _frameScore.Add(0);
        }

        return gameStarted;
    }

    private void On(Pinfall pinfall)
    {
        Guard.AgainstNull(pinfall);

        _frame = pinfall.Frame;
        _frameRoll = pinfall.FrameRoll + 1;
        _standingPins = pinfall.StandingPins;
        _roll = pinfall.Roll + 1;

        _frameScore[_frame - 1] += pinfall.Pins;

        if (pinfall.BonusRolls > 0)
        {
            _frameBonusRolls.Add(new(pinfall.Frame, pinfall.BonusRolls));
        }

        foreach (var frameBonus in pinfall.FrameBonuses)
        {
            _frameScore[frameBonus - 1] += pinfall.Pins;

            _frameBonusRolls.Find(candidate => candidate.Frame == frameBonus).BonusAssigned();
        }

        if (pinfall.FrameFinished)
        {
            _frame++;
            Finished = pinfall.Frame == 10;
            _frameRoll = 1;
        }
    }

    public Pinfall Roll(int pins)
    {
        if (!HasGameStarted)
        {
            throw new ApplicationException("The game has not yet started.");
        }

        if (Finished)
        {
            throw new ApplicationException("The game has finished.");
        }

        if (pins > _standingPins)
        {
            throw new ApplicationException($"You cannot knock over more than the '{_standingPins}' standing pins.");
        }

        var strike = pins == 10 && (IsLastFrame || IsFirstRoll);
        var spare = !strike && !IsFirstRoll && pins == _standingPins;

        var frameFinished = !IsLastFrame
            ? pins == 10 || IsSecondRoll
            : IsThirdRoll || (IsSecondRoll && !(spare || strike));

        Finished = frameFinished && IsLastFrame;
        _standingPins -= pins;

        var pinfall = new Pinfall
        {
            Pins = pins,
            StandingPins = Finished ? 0 :
                frameFinished || strike || spare ? 10 : _standingPins,
            Strike = strike,
            Spare = spare,
            Open = !IsFirstRoll && pins < _standingPins,
            FrameFinished = frameFinished,
            GameFinished = Finished,
            BonusRolls = IsLastFrame ? 0 :
                strike ? 2 :
                spare ? 1 : 0,
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

    public GameStarted Start(string bowler)
    {
        return On(new GameStarted
        {
            Bowler = bowler,
            StartDate = DateTime.Now
        });
    }
}