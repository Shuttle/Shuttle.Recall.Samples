using System;
using System.Collections.Generic;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.TenPinBowling.Shell
{
    public class BowlingQuery : IBowlingQuery
    {
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IBowlingQueryFactory _bowlingQueryFactory;

        public BowlingQuery(IDatabaseGateway databaseGateway, IBowlingQueryFactory bowlingQueryFactory)
        {
            _databaseGateway = databaseGateway;
            _bowlingQueryFactory = bowlingQueryFactory;
        }

        public DataRow FindGame(Guid id)
        {
            return _databaseGateway.GetRow(_bowlingQueryFactory.GetGame(id));
        }

        public IEnumerable<DataRow> GameFrames(Guid gameId)
        {
            return _databaseGateway.GetRows(_bowlingQueryFactory.GameFrames(gameId));
        }

        public IEnumerable<DataRow> AllGames()
        {
            return _databaseGateway.GetRows(_bowlingQueryFactory.AllGames());
        }

        public IEnumerable<DataRow> GameFrameBonuses(Guid gameId)
        {
            return _databaseGateway.GetRows(_bowlingQueryFactory.GameFrameBonuses(gameId));
        }
    }
}