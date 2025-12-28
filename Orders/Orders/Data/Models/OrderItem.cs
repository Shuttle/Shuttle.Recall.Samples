using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Orders.Data.Models;

[Index(nameof(OrderId), nameof(Product), IsUnique = true, Name = $"IX_{nameof(Orders.OrderItem)}_{nameof(OrderId)}_{nameof(Product)}")]
public class OrderItem
{
    [Key]
    public Guid Id { get; set; }   

    [Required]
    public Guid OrderId { get; set; }
    
    [Required]
    [StringLength(250)]
    public string Product { get; set; } = string.Empty;

    [Required]
    public decimal Quantity { get; set; }

    [Required]
    public decimal Cost { get; set; }
}