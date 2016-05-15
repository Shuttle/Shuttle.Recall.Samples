using System;
using Shuttle.Core.Data;

namespace Shuttle.TenPinBowling.Shell
{
    public class BowlingQueryFactory : IBowlingQueryFactory
    {
        public IQuery GameStarted(Guid id, string bowler)
        {
            return RawQuery.Create("insert into Game (Id, Bowler, DateStarted) values (@Id, @Bowler, @DateStarted)")
                .AddParameterValue(GameColumns.Id, id)
                .AddParameterValue(GameColumns.Bowler, bowler)
                .AddParameterValue(GameColumns.DateStarted, DateTime.Now);
        }

        public IQuery All()
        {
            return RawQuery.Create("select Id, Bowler, DateStarted from Game");
        }
    }
}