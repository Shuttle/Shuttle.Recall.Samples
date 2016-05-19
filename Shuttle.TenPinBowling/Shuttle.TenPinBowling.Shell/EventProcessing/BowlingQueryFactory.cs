using System;
using Shuttle.Core.Data;
using Shuttle.TenPinBowling.Events.v1;

namespace Shuttle.TenPinBowling.Shell
{
    public class BowlingQueryFactory : IBowlingQueryFactory
    {
        public IQuery GameStarted(Guid id, GameStarted gameStarted)
        {
            return RawQuery.Create("insert into [TenPinBowling].Game (Id, Bowler, DateStarted) values (@Id, @Bowler, @DateStarted)")
                .AddParameterValue(GameColumns.Id, id)
                .AddParameterValue(GameColumns.Bowler, gameStarted.Bowler)
                .AddParameterValue(GameColumns.DateStarted, gameStarted.StartDate);
        }

        public IQuery AllGames()
        {
            return RawQuery.Create("select Id, Bowler, DateStarted from [TenPinBowling].Game");
        }

        public IQuery GetGame(Guid id)
        {
            return RawQuery.Create("select Id, Bowler, DateStarted from [TenPinBowling].Game where Id = @Id")
                .AddParameterValue(GameColumns.Id, id);
        }

        public IQuery AddFrame(Guid id, Pinfall domainEvent)
        {
            return RawQuery.Create(@"
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
               .AddParameterValue(FrameColumns.GameId, id)
               .AddParameterValue(FrameColumns.Frame, domainEvent.Frame)
               .AddParameterValue(FrameColumns.FrameRoll, domainEvent.FrameRoll)
               .AddParameterValue(FrameColumns.Pins, domainEvent.Pins)
               .AddParameterValue(FrameColumns.Roll, domainEvent.Roll)
               .AddParameterValue(FrameColumns.Score, domainEvent.Score)
               .AddParameterValue(FrameColumns.BonusRolls, domainEvent.BonusRolls)
               .AddParameterValue(FrameColumns.FrameFinished, domainEvent.FrameFinished)
               .AddParameterValue(FrameColumns.Strike, domainEvent.Strike)
               .AddParameterValue(FrameColumns.Spare, domainEvent.Spare)
               .AddParameterValue(FrameColumns.Open, domainEvent.Open)
               .AddParameterValue(FrameColumns.StandingPins, domainEvent.StandingPins)
               .AddParameterValue(FrameColumns.GameFinished, domainEvent.GameFinished);
        }

        public IQuery AddFrameBonus(Guid id, int frame, int bonusFrame, int pins)
        {
            return RawQuery.Create(@"
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
                .AddParameterValue(FrameBonusColumns.GameId, id)
                .AddParameterValue(FrameBonusColumns.Frame, frame)
                .AddParameterValue(FrameBonusColumns.BonusFrame, bonusFrame)
                .AddParameterValue(FrameBonusColumns.BonusPins, pins);
        }

        public IQuery GameFrames(Guid gameId)
        {
            return RawQuery.Create(@"
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
                .AddParameterValue(FrameColumns.GameId, gameId);
        }

        public IQuery GameFrameBonuses(Guid gameId)
        {
            return RawQuery.Create(@"
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
                .AddParameterValue(FrameColumns.GameId, gameId);
        }
    }
}