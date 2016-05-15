using System;
using Shuttle.Core.Data;

namespace Shuttle.TenPinBowling.Shell
{
    public interface IBowlingQueryFactory
    {
        IQuery GameStarted(Guid id, string bowler);
        IQuery All();
    }
}