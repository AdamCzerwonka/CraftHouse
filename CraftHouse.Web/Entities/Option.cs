using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CraftHouse.Web.Entities;

public class Option : EntityBase
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;
    
    [Required]
    public int MaxOccurs { get; set; }

    public bool Required { get; set; }
    
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public ICollection<OptionValue> OptionValues { get; set; } = null!;
}