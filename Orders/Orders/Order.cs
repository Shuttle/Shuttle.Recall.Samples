using Orders.Events.v1;
using Shuttle.Contract;

namespace Orders;

public class Order(Guid id)
{
    private readonly List<OrderItem> _items = [];
    public string CustomerName { get; private set; } = string.Empty;
    public DateTimeOffset DateRegistered { get; private set; }

    public Guid Id { get; } = id;

    public ItemAdded AddItem(string product, decimal quantity, decimal cost)
    {
        if (quantity < 1)
        {
            throw new ApplicationException($"Argument '{nameof(product)}' may not be less than 1.");
        }

        if (!(cost > 0))
        {
            throw new ApplicationException($"Argument '{nameof(cost)}' must be greater than 0.");
        }

        var result = new ItemAdded
        {
            Product = Guard.AgainstEmpty(product),
            Quantity = quantity,
            Cost = cost
        };

        return On(result);
    }

    private ItemAdded On(ItemAdded @event)
    {
        Guard.AgainstNull(@event);

        _items.Add(new()
        {
            Product = @event.Product,
            Quantity = @event.Quantity,
            Cost = @event.Cost
        });

        return @event;
    }

    private Registered On(Registered @event)
    {
        CustomerName = @event.CustomerName;
        DateRegistered = @event.DateRegistered;

        return @event;
    }

    public Registered Register(string customerName, DateTimeOffset dateRegistered)
    {
        if (dateRegistered > DateTimeOffset.UtcNow)
        {
            throw new ArgumentException($"Argument '{nameof(dateRegistered)}' may not be in the future.");
        }

        return On(new Registered
        {
            CustomerName = Guard.AgainstEmpty(customerName),
            DateRegistered = dateRegistered
        });
    }

    public decimal Total()
    {
        return _items.Sum(item => item.Total());
    }
}