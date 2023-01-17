using System.ComponentModel.DataAnnotations;

namespace CraftHouse.Web.Entities;

public class EntityBase
{
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public DateTime? DeletedAt { get; set; } = null;
}