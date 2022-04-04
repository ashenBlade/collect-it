using CollectIt.Database.Entities.Account;

namespace CollectIt.Database.Abstractions.Account.Interfaces;

public interface ISubscriptionManager
{
    public Task<Subscription> CreateSubscriptionAsync(string name,
                                                      string description,
                                                      int monthDuration,
                                                      ResourceType appliedResourceType,
                                                      int maxResourcesCount,
                                                      int? restrictionId);

    public Task<Subscription?> FindSubscriptionByIdAsync(int id);

    public Task DeleteSubscriptionAsync(int id);
    public Task<List<Subscription>> GetAllWithResourceTypeAsync(ResourceType resourceType);
}