using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CraftHouse.Web.Entities;

public class OptionValue
{
    [Required]
    public string Value { get; set; } = null!;

    [Required]
    public float Price { get; set; }

    [Required]
    public int OptionId { get; set; }

    public Option Option { get; set; } = null!;
}