using CraftHouse.Web.Entities;

namespace CraftHouse.Web.Repositories;

public interface IOptionValueRepository
{
    Task<OptionValue?> GetOptionValueAsync(int optionId, string value, CancellationToken cancellationToken);
    Task<List<OptionValue>> GetOptionValuesByOptionAsync(Option option, CancellationToken cancellationToken);
    Task<List<OptionValue>> GetOptionValuesByOptionIdAsync(int optionId, CancellationToken cancellationToken);
    Task AddNewOptionValueAsync(OptionValue optionValue, CancellationToken cancellationToken);
    Task RemoveOptionValueAsync(int optionId, string value, CancellationToken cancellationToken);

    Task UpdateOptionValueFieldsAsync(int optionId, string oldValue, string newValue, float newPrice,
        CancellationToken cancellationToken);
}