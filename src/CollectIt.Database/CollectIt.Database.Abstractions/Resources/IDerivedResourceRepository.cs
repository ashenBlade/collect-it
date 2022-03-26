using CollectIt.Database.Entities.Resources;

namespace CollectIt.Database.Abstractions.Resources;

public interface IDerivedResourceRepository<TItem, TId>
{
    Task<TId> AddAsync(TItem item, Resource resource);
    Task<TItem> FindByIdAsync(TId id);
    Task UpdateAsync(TItem item);
    Task RemoveAsync(TItem item, Resource resource);
    IAsyncEnumerable<TItem> GetAllByName(string name);
}