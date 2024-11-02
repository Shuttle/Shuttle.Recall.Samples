using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Shuttle.TenPinBowling;

public interface IBowlingQuery
{
    Task<IEnumerable<DataRow>> AllGamesAsync();
    Task<DataRow?> FindGameAsync(Guid id);
    Task<IEnumerable<DataRow>> GameFrameBonusesAsync(Guid gameId);
    Task<IEnumerable<DataRow>> GameFramesAsync(Guid gameId);
}