using CollectIt.Database.Entities.Account;

namespace CollectIt.Database.Abstractions.Account.Interfaces;

public interface ISubscriptionRepository : IRepository<Subscription, int>
{
    public Task<List<Subscription>> GetAllWithResourceType(ResourceType type);
    public Task<List<Subscription>> GetAll();
}