using System.ComponentModel.DataAnnotations;

namespace CraftHouse.Web.Entities;

public class Product:EntityBase
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public bool IsAvailable { get; set; }
    
    [Required]
    public float Price { get; set; }

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public ICollection<Option> Options { get; set; } = null!;
}