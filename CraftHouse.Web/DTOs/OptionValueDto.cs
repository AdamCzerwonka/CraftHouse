using CraftHouse.Web.Entities;

namespace CraftHouse.Web.DTOs;

public class OptionValueDto
{
    public string Value { get; set; } = null!;

    public float Price { get; set; }

    public int OptionId { get; set; }

    public OptionValue MapToOptionValue()
    {
        return new OptionValue()
        {
            Value = Value,
            Price = Price,
            OptionId = OptionId
        };
    }
}