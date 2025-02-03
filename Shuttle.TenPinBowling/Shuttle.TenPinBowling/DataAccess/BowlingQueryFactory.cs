using System;
using Shuttle.Core.Data;
using Shuttle.TenPinBowling.Events.v1;

namespace Shuttle.TenPinBowling;

public class BowlingQueryFactory : IBowlingQueryFactory
{
    public IQuery GameStarted(Guid id, GameStarted gameStarted)
    {
        return new Query("insert into [TenPinBowling].Game (Id, Bowler, DateStarted) values (@Id, @Bowler, @DateStarted)")
            .AddParameter(Columns.Id, id)
            .AddParameter(Columns.Bowler, gameStarted.Bowler)
            .AddParameter(Columns.DateStarted, gameStarted.StartDate);
    }

    public IQuery AllGames()
    {
        return new Query("select Id, Bowler, DateStarted from [TenPinBowling].Game");
    }

    public IQuery GetGame(Guid id)
    {
        return new Query("select Id, Bowler, DateStarted from [TenPinBowling].Game where Id = @Id")
            .AddParameter(Columns.Id, id);
    }

    public IQuery AddFrame(Guid id, Pinfall domainEvent)
    {
        return new Query(@"
insert into [TenPinBowling].[Frame]
    ([GameId]
    ,[Frame]
    ,[FrameRoll]
    ,[Pins]
    ,[Roll]
    ,[Score]
    ,[BonusRolls]
    ,[FrameFinished]
    ,[Strike]
    ,[Spare]
    ,[Open]
    ,[StandingPins]
    ,[GameFinished])
values
    (@GameId
    ,@Frame
    ,@FrameRoll
    ,@Pins
    ,@Roll
    ,@Score
    ,@BonusRolls
    ,@FrameFinished
    ,@Strike
    ,@Spare
    ,@Open
    ,@StandingPins
    ,@GameFinished)
")
            .AddParameter(Columns.GameId, id)
            .AddParameter(Columns.Frame, domainEvent.Frame)
            .AddParameter(Columns.FrameRoll, domainEvent.FrameRoll)
            .AddParameter(Columns.Pins, domainEvent.Pins)
            .AddParameter(Columns.Roll, domainEvent.Roll)
            .AddParameter(Columns.Score, domainEvent.Score)
            .AddParameter(Columns.BonusRolls, domainEvent.BonusRolls)
            .AddParameter(Columns.FrameFinished, domainEvent.FrameFinished)
            .AddParameter(Columns.Strike, domainEvent.Strike)
            .AddParameter(Columns.Spare, domainEvent.Spare)
            .AddParameter(Columns.Open, domainEvent.Open)
            .AddParameter(Columns.StandingPins, domainEvent.StandingPins)
            .AddParameter(Columns.GameFinished, domainEvent.GameFinished);
    }

    public IQuery AddFrameBonus(Guid id, int frame, int bonusFrame, int pins)
    {
        return new Query(@"
insert into [TenPinBowling].[FrameBonus]
    ([GameId]
    ,[Frame]
    ,[BonusFrame]
    ,[BonusPins])
values
    (@GameId
    ,@Frame
    ,@BonusFrame
    ,@BonusPins)
")
            .AddParameter(Columns.GameId, id)
            .AddParameter(Columns.Frame, frame)
            .AddParameter(Columns.BonusFrame, bonusFrame)
            .AddParameter(Columns.BonusPins, pins);
    }

    public IQuery GameFrames(Guid gameId)
    {
        return new Query(@"
select
    [GameId]
    ,[Frame]
    ,[FrameRoll]
    ,[Pins]
    ,[Roll]
    ,[Score]
    ,[BonusRolls]
    ,[FrameFinished]
    ,[Strike]
    ,[Spare]
    ,[Open]
    ,[StandingPins]
    ,[GameFinished]
from
    [TenPinBowling].Frame
where
    GameId = @GameId
")
            .AddParameter(Columns.GameId, gameId);
    }

    public IQuery GameFrameBonuses(Guid gameId)
    {
        return new Query(@"
select
    [GameId]
    ,[Frame]
    ,[BonusFrame]
    ,[BonusPins]
from
    [TenPinBowling].FrameBonus
where
    GameId = @GameId
")
            .AddParameter(Columns.GameId, gameId);
    }
}