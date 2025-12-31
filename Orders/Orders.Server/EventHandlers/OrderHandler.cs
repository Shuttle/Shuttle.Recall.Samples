using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orders.Data;
using Orders.Events.v1;
using Shuttle.Recall;

namespace Orders.Server.EventHandlers;

public class OrderHandler(ILogger<OrderHandler> logger, OrderDbContext dbContext) : 
    IEventHandler<Registered>,
    IEventHandler<ItemAdded>
{
    public async Task ProcessEventAsync(IEventHandlerContext<Registered> context, CancellationToken cancellationToken = default)
    {
        dbContext.Orders.Add(new()
        {
            Id = context.PrimitiveEvent.Id,
            CustomerName = context.Event.CustomerName
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("[OrderHandler/Registered] : order id = '{OrderId}' / customer name = '{CustomerName}'", context.PrimitiveEvent.Id, context.Event.CustomerName);
    }

    public async Task ProcessEventAsync(IEventHandlerContext<ItemAdded> context, CancellationToken cancellationToken = default)
    {
        var model = await dbContext.Orders
            .Include(order => order.Items)
            .FirstOrDefaultAsync(item => item.Id == context.PrimitiveEvent.Id, cancellationToken: cancellationToken);

        if (model == null)
        {
            throw new ApplicationException($"Could not find order with id '{context.PrimitiveEvent.Id}'");
        }

        var @event = context.Event;

        dbContext.OrderItems.Add(new()
        {
            Id = Guid.NewGuid(),
            OrderId = context.PrimitiveEvent.Id,
            Product = @event.Product,
            Cost = @event.Cost,
            Quantity = @event.Quantity,
            Total = @event.Cost * @event.Quantity
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.Entry(model).ReloadAsync(cancellationToken);

        model.Total = model.Items.Sum(item => item.Total);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("[OrderHandler/ItemAdded] : order id = '{OrderId}' / product = '{Product}' / total = '{Total}'", context.PrimitiveEvent.Id, context.Event.Product, model.Total.ToString("C"));
    }
}