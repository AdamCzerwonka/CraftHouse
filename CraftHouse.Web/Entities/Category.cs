using System.ComponentModel.DataAnnotations;

namespace CraftHouse.Web.Entities;

public class Category:EntityBase
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = null!;
}
