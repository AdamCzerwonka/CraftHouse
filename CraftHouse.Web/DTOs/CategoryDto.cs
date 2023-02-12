using CraftHouse.Web.Entities;

namespace CraftHouse.Web.DTOs;

public class CategoryDto
{
    public string Name { get; set; } = null!;

    public Category MapToCategory()
    {
        return new Category()
        {
            Name = Name
        };
    }
}