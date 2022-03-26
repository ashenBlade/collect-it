using CollectIt.MVC.Account.IdentityEntities;

namespace CollectIt.MVC.Account.Abstractions.Interfaces;

public interface ISubscriptionService
{
    public Task<UserSubscription> SubscribeUserAsync(int userId, int subscriptionId, DateTime startDate);
}