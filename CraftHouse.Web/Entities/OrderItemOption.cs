using System.ComponentModel.DataAnnotations;

namespace CraftHouse.Web.Entities;

public class OrderItemOption : EntityBase
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;
    
    [Required]
    public float Value { get; set; } 

    public int OrderItemId { get; set; }
    public OrderItem OrderItem { get; set; } = null!;

    public int OptionId { get; set; }
    public Option Option { get; set; } = null!;
}