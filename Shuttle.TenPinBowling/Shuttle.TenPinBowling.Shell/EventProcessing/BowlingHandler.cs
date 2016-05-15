using Shuttle.Core.Data;
using Shuttle.Recall;
using Shuttle.TenPinBowling.Events.v1;

namespace Shuttle.TenPinBowling.Shell
{
    public class BowlingHandler:
        IEventHandler<GameStarted>
    {
        private readonly IBowlingQueryFactory _bowlingQueryFactory;
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly DatabaseGateway _databaseGateway;

        public BowlingHandler(IDatabaseContextFactory databaseContextFactory, DatabaseGateway databaseGateway,
            IBowlingQueryFactory bowlingQueryFactory)
        {
            _databaseContextFactory = databaseContextFactory;
            _databaseGateway = databaseGateway;
            _bowlingQueryFactory = bowlingQueryFactory;
        }

        public void ProcessEvent(IEventHandlerContext<GameStarted> context)
        {
            _databaseGateway.ExecuteUsing(_bowlingQueryFactory.GameStarted(context.ProjectionEvent.Id,
                context.DomainEvent.Bowler));
        }
    }
}