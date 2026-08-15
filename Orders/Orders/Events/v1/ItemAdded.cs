namespace Orders.Events.v1;

public class ItemAdded
{
    public decimal Cost { get; set; }
    public string Product { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}