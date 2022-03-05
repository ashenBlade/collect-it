namespace CollectIt.MVC.Account.Abstractions;

public interface IRepository<TItem, TId>
{
    Task<TId> AddAsync(TItem item);
    Task RemoveAsync(TItem item);
    Task<TItem> FindByIdAsync(TId id);
    Task UpdateAsync(TItem item);
}