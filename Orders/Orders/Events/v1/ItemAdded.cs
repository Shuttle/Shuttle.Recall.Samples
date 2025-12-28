namespace Orders.Events.v1;

public class ItemAdded
{
    public double Cost { get; set; }
    public string Product { get; set; } = string.Empty;
    public double Quantity { get; set; }
}