using Orders.Events.v1;
using Shuttle.Recall;

namespace Orders;

public class OrderHandler : IEventHandler<Registered>
{
    public async Task ProcessEventAsync(IEventHandlerContext<Registered> context, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}