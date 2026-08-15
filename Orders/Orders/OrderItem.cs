namespace Orders;

public class OrderItem
{
    public decimal Cost { get; set; }
    public string Product { get; set; } = string.Empty;
    public decimal Quantity { get; set; }

    public decimal Total()
    {
        return Quantity * Cost;
    }
}