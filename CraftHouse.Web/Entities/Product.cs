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

    /*0
    [Required]
    public string MeatType { get; set; } = null!;
    // TODO CHANGE MEAT TYPE
    
    [Required]
    public string Sauce { get; set; } = null!;
    // TODO CHANGE SAUCE TYPE
            
    [Required]
    public int Weight { get; set; }
    
    [Required]
    public string Size { get; set; } = null!;
    // TODO CHANGE SIZE TYPE
    
    public List<string> Addons { get; set; } = new();
    // TODO CHANGE ADDONS TYPE
        
    public List<string> Ingredients { get; set; } = new();
    // TODO CHANGE INGREDIENTS TYPE
    */
}