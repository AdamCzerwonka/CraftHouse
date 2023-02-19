using System.ComponentModel.DataAnnotations;

namespace CraftHouse.Web.Entities;

public class Order : EntityBase
{
    [Key]
    public int Id { get; set; }

    [Required]
    public float Value { get; set; }

    [Required]
    public OrderStatus OrderStatus { get; set; } = OrderStatus.Created;

    [Required]
    public string Telephone { get; set; } = null!;

    [Required]
    public string City { get; set; } = null!;

    [Required]
    public string PostalCode { get; set; } = null!;

    [Required]
    public string AddressLine { get; set; } = null!;

    public string? Comment { get; set; }

    [Required]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = null!;
}

public enum OrderStatus {
    Created,
    Pending,
    Preparation,
    Shipping,
    Finished,
    Canceled
}