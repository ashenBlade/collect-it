using CollectIt.MVC.Resources.Entities;

namespace CollectIt.MVC.Resources.Abstractions;

public interface IDerivedResourceRepository<TItem, TId>
{
    Task<TId> AddAsync(TItem item, Resource resource);
    Task<TItem> FindByIdAsync(TId id);
    Task UpdateAsync(TItem item);
    Task RemoveAsync(TItem item, Resource resource);
}