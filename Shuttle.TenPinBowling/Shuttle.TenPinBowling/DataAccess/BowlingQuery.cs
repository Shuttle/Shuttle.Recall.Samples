using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;

namespace Shuttle.TenPinBowling;

public class BowlingQuery : IBowlingQuery
{
    private readonly IDatabaseContextService _databaseContextService;
    private readonly IBowlingQueryFactory _bowlingQueryFactory;

    public BowlingQuery(IDatabaseContextService databaseContextService, IBowlingQueryFactory bowlingQueryFactory)
    {
        _databaseContextService = Guard.AgainstNull(databaseContextService);
        _bowlingQueryFactory = Guard.AgainstNull(bowlingQueryFactory);
    }

    public async Task<DataRow?> FindGameAsync(Guid id)
    {
        return await _databaseContextService.Active.GetRowAsync(_bowlingQueryFactory.GetGame(id));
    }

    public async Task<IEnumerable<DataRow>> GameFramesAsync(Guid gameId)
    {
        return await _databaseContextService.Active.GetRowsAsync(_bowlingQueryFactory.GameFrames(gameId));
    }

    public async Task<IEnumerable<DataRow>> AllGamesAsync()
    {
        return await _databaseContextService.Active.GetRowsAsync(_bowlingQueryFactory.AllGames());
    }

    public async Task<IEnumerable<DataRow>> GameFrameBonusesAsync(Guid gameId)
    {
        return await _databaseContextService.Active.GetRowsAsync(_bowlingQueryFactory.GameFrameBonuses(gameId));
    }
}