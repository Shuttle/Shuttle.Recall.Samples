using Shuttle.Core.Data;

namespace Shuttle.TenPinBowling.Shell
{
    public interface ITenPinBowlingDatabaseContextFactory : IDatabaseContextFactory
    {
        IDatabaseContext Create();
    }
}