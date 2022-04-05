using CollectIt.Database.Entities.Account;

namespace CollectIt.Database.Abstractions.Account.Interfaces;

public interface ISubscriptionManager
{
    public Task<Subscription> CreateSubscriptionAsync(string name,
                                                      string description,
                                                      int monthDuration,
                                                      ResourceType appliedResourceType,
                                                      int maxResourcesCount,
                                                      int? restrictionId,
                                                      bool active = false);

    public Task<List<Subscription>> GetSubscriptionsAsync(int pageNumber, int pageSize);
    public Task<List<Subscription>> GetActiveSubscriptionsAsync(int pageNumber, int pageSize);
    public Task<Subscription?> FindSubscriptionByIdAsync(int id);
    public Task DeleteSubscriptionAsync(int id);
    public Task<List<Subscription>> GetActiveSubscriptionsWithResourceTypeAsync(ResourceType resourceType);
    public Task<List<Subscription>> GetActiveSubscriptionsWithResourceTypeAsync(ResourceType resourceType, int pageNumber, int pageSize);
    public Task ActivateSubscriptionAsync(int subscriptionId);
    public Task DeactivateSubscriptionAsync(int subscriptionId);
}