using CraftHouse.Web.Entities;

namespace CraftHouse.Web.DTOs;

public class ProductDto
{
    public string Name { get; set; } = null!;
    
    public bool IsAvailable { get; set; }
    
    public float Price { get; set; }
    
    public string Description { get; set; } = null!;
    
    public int CategoryId { get; set; }

    public Product MapToProduct()
    {
        return new Product()
        {
            Name = Name,
            IsAvailable = IsAvailable,
            Price = Price,
            Description = Description,
            CategoryId = CategoryId
        };
    }
}