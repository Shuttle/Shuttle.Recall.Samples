using Bogus;
using Microsoft.Extensions.Logging;
using Orders.Messages.v1;
using Shuttle.Hopper;
using Shuttle.Recall;

namespace Orders.Server.MessageHandlers;

public class CreateOrderHandler(ILogger<CreateOrderHandler> logger, IEventStore eventStore) : IDirectMessageHandler<CreateOrder>
{
    public async Task ProcessMessageAsync(CreateOrder message, CancellationToken cancellationToken = default)
    {
        var person = new Person();
        var order = new Order(Guid.NewGuid());
        var stream = await eventStore.GetAsync(order.Id, cancellationToken: cancellationToken);

        stream.Add(order.Register($"{person.FirstName} {person.LastName}", DateTimeOffset.UtcNow));
        stream.Add(order.AddItem("item-1", 1, 100));
        stream.Add(order.AddItem("item-2", 2, 200));
        stream.Add(order.AddItem("item-3", 3, 300));

        await eventStore.SaveAsync(stream, cancellationToken: cancellationToken);

        logger.LogInformation("[order/created] : id = '{OrderId}' / customer name = '{CustomerName}'", order.Id, order.CustomerName);
    }
}