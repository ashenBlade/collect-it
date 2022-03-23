using CollectIt.MVC.Account.IdentityEntities;

namespace CollectIt.MVC.Account.Abstractions.Interfaces;

public interface IRepository<TItem, TId>
{
    Task<TId> AddAsync(TItem item);
    Task<TItem?> FindByIdAsync(TId id);
    Task UpdateAsync(TItem item);
    Task RemoveAsync(TItem item);
}