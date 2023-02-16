using System.ComponentModel.DataAnnotations;

namespace CraftHouse.Web.Entities;

public class OrderItem : EntityBase
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int OrderId { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public float Value { get; set; }
    
    public Order Order { get; set; } = null!;

    public Product Product { get; set; } = null!;

    public ICollection<OrderItemOption> OrderItemOptions { get; set; } = null!;
}