using System.ComponentModel.DataAnnotations;

namespace CraftHouse.Web.Entities;

public class Option : EntityBase
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public ICollection<Product> Products { get; set; } = null!;

    public ICollection<OptionValue> OptionValues { get; set; } = null!;
}