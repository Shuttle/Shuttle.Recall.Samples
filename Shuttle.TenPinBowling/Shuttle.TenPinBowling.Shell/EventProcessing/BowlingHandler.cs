using Shuttle.Core.Data;
using Shuttle.Recall;
using Shuttle.TenPinBowling.Events.v1;

namespace Shuttle.TenPinBowling.Shell
{
    public class BowlingHandler:
        IEventHandler<GameStarted>,
        IEventHandler<Pinfall>
    {
        private readonly IBowlingQueryFactory _bowlingQueryFactory;
        private readonly DatabaseGateway _databaseGateway;

        public BowlingHandler(DatabaseGateway databaseGateway,
            IBowlingQueryFactory bowlingQueryFactory)
        {
            _databaseGateway = databaseGateway;
            _bowlingQueryFactory = bowlingQueryFactory;
        }

        public void ProcessEvent(IEventHandlerContext<GameStarted> context)
        {
            _databaseGateway.ExecuteUsing(_bowlingQueryFactory.GameStarted(context.ProjectionEvent.Id,
                context.DomainEvent));
        }

        public void ProcessEvent(IEventHandlerContext<Pinfall> context)
        {
            _databaseGateway.ExecuteUsing(_bowlingQueryFactory.AddFrame(context.ProjectionEvent.Id, context.DomainEvent));

            foreach (var frameBonus in context.DomainEvent.FrameBonuses)
            {
                _databaseGateway.ExecuteUsing(_bowlingQueryFactory.AddFrameBonus(context.ProjectionEvent.Id,
                    context.DomainEvent.Frame, frameBonus, context.DomainEvent.Pins));
            }
        }
    }
}