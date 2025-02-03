using System;
using Shuttle.Core.Data;
using Shuttle.TenPinBowling.Events.v1;

namespace Shuttle.TenPinBowling;

public interface IBowlingQueryFactory
{
    IQuery AddFrame(Guid id, Pinfall domainEvent);
    IQuery AddFrameBonus(Guid id, int frame, int bonusFrame, int pins);
    IQuery AllGames();
    IQuery GameFrameBonuses(Guid gameId);
    IQuery GameFrames(Guid gameId);
    IQuery GameStarted(Guid id, GameStarted gameStarted);
    IQuery GetGame(Guid id);
}