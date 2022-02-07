using System;
using Shuttle.Core.Data;
using Shuttle.TenPinBowling.Events.v1;

namespace Shuttle.TenPinBowling.Shell
{
    public interface IBowlingQueryFactory
    {
        IQuery GameStarted(Guid id, GameStarted gameStarted);
        IQuery AllGames();
        IQuery GetGame(Guid id);
        IQuery AddFrame(Guid id, Pinfall domainEvent);
        IQuery AddFrameBonus(Guid id, int frame, int bonusFrame, int pins);
        IQuery GameFrames(Guid gameId);
        IQuery GameFrameBonuses(Guid gameId);
    }
}