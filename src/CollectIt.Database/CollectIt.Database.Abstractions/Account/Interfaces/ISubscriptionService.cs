using CollectIt.Database.Entities.Account;

namespace CollectIt.Database.Abstractions.Account.Interfaces;

public interface ISubscriptionService
{
    public Task<UserSubscription> SubscribeUserAsync(int userId, int subscriptionId, DateTime startDate);
}