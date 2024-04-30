using System;
using Shuttle.Core.Data;
using Shuttle.TenPinBowling.Events.v1;

namespace Shuttle.TenPinBowling.Shell
{
    public class BowlingQueryFactory : IBowlingQueryFactory
    {
        public IQuery GameStarted(Guid id, GameStarted gameStarted)
        {
            return new Query("insert into [TenPinBowling].Game (Id, Bowler, DateStarted) values (@Id, @Bowler, @DateStarted)")
                .AddParameter(GameColumns.Id, id)
                .AddParameter(GameColumns.Bowler, gameStarted.Bowler)
                .AddParameter(GameColumns.DateStarted, gameStarted.StartDate);
        }

        public IQuery AllGames()
        {
            return new Query("select Id, Bowler, DateStarted from [TenPinBowling].Game");
        }

        public IQuery GetGame(Guid id)
        {
            return new Query("select Id, Bowler, DateStarted from [TenPinBowling].Game where Id = @Id")
                .AddParameter(GameColumns.Id, id);
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
               .AddParameter(FrameColumns.GameId, id)
               .AddParameter(FrameColumns.Frame, domainEvent.Frame)
               .AddParameter(FrameColumns.FrameRoll, domainEvent.FrameRoll)
               .AddParameter(FrameColumns.Pins, domainEvent.Pins)
               .AddParameter(FrameColumns.Roll, domainEvent.Roll)
               .AddParameter(FrameColumns.Score, domainEvent.Score)
               .AddParameter(FrameColumns.BonusRolls, domainEvent.BonusRolls)
               .AddParameter(FrameColumns.FrameFinished, domainEvent.FrameFinished)
               .AddParameter(FrameColumns.Strike, domainEvent.Strike)
               .AddParameter(FrameColumns.Spare, domainEvent.Spare)
               .AddParameter(FrameColumns.Open, domainEvent.Open)
               .AddParameter(FrameColumns.StandingPins, domainEvent.StandingPins)
               .AddParameter(FrameColumns.GameFinished, domainEvent.GameFinished);
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
                .AddParameter(FrameBonusColumns.GameId, id)
                .AddParameter(FrameBonusColumns.Frame, frame)
                .AddParameter(FrameBonusColumns.BonusFrame, bonusFrame)
                .AddParameter(FrameBonusColumns.BonusPins, pins);
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
                .AddParameter(FrameColumns.GameId, gameId);
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
                .AddParameter(FrameColumns.GameId, gameId);
        }
    }
}