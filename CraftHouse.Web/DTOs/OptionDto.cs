using CraftHouse.Web.Entities;

namespace CraftHouse.Web.DTOs;

public class OptionDto
{
    public string Name { get; set; } = null!;
    public int MaxOccurs { get; set; }
    public int ProductId { get; set; }

    public Option MapToOption()
    {
        return new Option()
        {
            Name = Name,
            MaxOccurs = MaxOccurs,
            ProductId = ProductId
        };
    }
}