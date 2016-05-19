using System;
using System.Collections.Generic;
using System.Data;

namespace Shuttle.TenPinBowling.Shell
{
    public interface IBowlingQuery
    {
        DataRow GetGame(Guid id);
        IEnumerable<DataRow> GameFrames(Guid gameId);
        IEnumerable<DataRow> AllGames();
        IEnumerable<DataRow> GameFrameBonuses(Guid gameId);
    }
}