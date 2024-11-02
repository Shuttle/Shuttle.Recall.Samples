using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Recall;
using Shuttle.TenPinBowling.Events.v1;

namespace Shuttle.TenPinBowling.Projection;

public class BowlingHandler :
    IEventHandler<GameStarted>,
    IEventHandler<Pinfall>
{
    private readonly IDatabaseContextService _databaseContextService;
    private readonly IBowlingQueryFactory _bowlingQueryFactory;

    public BowlingHandler(IDatabaseContextService databaseContextService, IBowlingQueryFactory bowlingQueryFactory)
    {
        _databaseContextService = Guard.AgainstNull(databaseContextService);
        _bowlingQueryFactory = Guard.AgainstNull(bowlingQueryFactory);
    }

    public async Task ProcessEventAsync(IEventHandlerContext<GameStarted> context)
    {
        await _databaseContextService.Active.ExecuteAsync(_bowlingQueryFactory.GameStarted(context.PrimitiveEvent.Id, context.Event));
    }

    public async Task ProcessEventAsync(IEventHandlerContext<Pinfall> context)
    {
        await _databaseContextService.Active.ExecuteAsync(_bowlingQueryFactory.AddFrame(context.PrimitiveEvent.Id, context.Event));

        foreach (var frameBonus in context.Event.FrameBonuses)
        {
            await _databaseContextService.Active.ExecuteAsync(_bowlingQueryFactory.AddFrameBonus(context.PrimitiveEvent.Id, context.Event.Frame, frameBonus, context.Event.Pins));
        }
    }
}