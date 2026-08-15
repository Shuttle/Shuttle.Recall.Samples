using System.ComponentModel.DataAnnotations;

namespace Orders.Data.Models;

public class Order
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(250)]
    public string CustomerName { get; set; } = string.Empty;

    public decimal Total { get; set; }

    public DateTimeOffset DateRegistered { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<OrderItem> Items { get; set; } = [];
}