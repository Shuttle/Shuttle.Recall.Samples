using Shuttle.Core.Data;
using Shuttle.Recall;
using Shuttle.TenPinBowling.Events.v1;

namespace Shuttle.TenPinBowling.Shell
{
    public class BowlingHandler :
        IEventHandler<GameStarted>,
        IEventHandler<Pinfall>
    {
        private readonly IBowlingQueryFactory _bowlingQueryFactory;
        private readonly IDatabaseGateway _databaseGateway;

        public BowlingHandler(IDatabaseGateway databaseGateway, IBowlingQueryFactory bowlingQueryFactory)
        {
            _databaseGateway = databaseGateway;
            _bowlingQueryFactory = bowlingQueryFactory;
        }

        public void ProcessEvent(IEventHandlerContext<GameStarted> context)
        {
            _databaseGateway.ExecuteUsing(_bowlingQueryFactory.GameStarted(context.PrimitiveEvent.Id, context.Event));
        }

        public void ProcessEvent(IEventHandlerContext<Pinfall> context)
        {
            _databaseGateway.ExecuteUsing(_bowlingQueryFactory.AddFrame(context.PrimitiveEvent.Id, context.Event));

            foreach (var frameBonus in context.Event.FrameBonuses)
            {
                _databaseGateway.ExecuteUsing(_bowlingQueryFactory.AddFrameBonus(context.PrimitiveEvent.Id,
                    context.Event.Frame, frameBonus, context.Event.Pins));
            }
        }
    }
}