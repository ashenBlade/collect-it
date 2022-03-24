using CollectIt.Database.Entities.Account;

namespace CollectIt.Database.Abstractions.Account.Interfaces;

public interface IRepository<TItem, TId>
{
    Task<TId> AddAsync(TItem item);
    Task<TItem?> FindByIdAsync(TId id);
    Task UpdateAsync(TItem item);
    Task RemoveAsync(TItem item);
}