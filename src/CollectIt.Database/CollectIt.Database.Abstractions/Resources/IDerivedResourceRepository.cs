using CollectIt.Database.Entities.Resources;

namespace CollectIt.Database.Abstractions.Resources;

public interface IDerivedResourceRepository<TItem, TId>
{
    Task<TId> AddAsync(TItem item);
    Task<TItem> FindByIdAsync(TId id);
    Task UpdateAsync(TItem item);
    Task RemoveAsync(TItem item);
}