namespace Orders.Events.v1;

public class Registered
{
    public string CustomerName { get; set; } = string.Empty;
    public DateTimeOffset DateRegistered { get; set; }
}