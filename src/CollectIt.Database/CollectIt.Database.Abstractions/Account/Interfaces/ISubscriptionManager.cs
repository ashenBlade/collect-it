using CollectIt.Database.Entities.Account;
using CollectIt.Database.Entities.Account.Restrictions;
using Microsoft.AspNetCore.Identity;

namespace CollectIt.Database.Abstractions.Account.Interfaces;

public interface ISubscriptionManager
{
    public Task<Subscription> CreateSubscriptionAsync(string name,
                                                      string description,
                                                      int monthDuration,
                                                      ResourceType appliedResourceType,
                                                      int price,
                                                      int maxResourcesCount,
                                                      Restriction? restriction,
                                                      bool active = false);

    public Task<List<Subscription>> GetSubscriptionsAsync(int pageNumber, int pageSize);
    public Task<Subscription?> FindSubscriptionByIdAsync(int id);
    public Task<List<Subscription>> GetActiveSubscriptionsWithResourceTypeAsync(ResourceType resourceType);

    public Task<List<Subscription>> GetActiveSubscriptionsWithResourceTypeAsync(
        ResourceType resourceType,
        int pageNumber,
        int pageSize);

    public Task<IdentityResult> ActivateSubscriptionAsync(int subscriptionId);
    public Task<IdentityResult> DeactivateSubscriptionAsync(int subscriptionId);
    public Task<IdentityResult> ChangeSubscriptionNameAsync(int subscriptionId, string newName);
    public Task<IdentityResult> ChangeSubscriptionDescriptionAsync(int subscriptionId, string newDescription);
}