using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account;

namespace CollectIt.Database.Infrastructure.Account;

public static class SubscriptionServiceExtensions
{
    public static Task<UserSubscription> SubscribeUserAsync(this ISubscriptionService service,
                                                            int userId,
                                                            int subscriptionId)
    {
        return service.SubscribeUserAsync(userId, subscriptionId, DateTime.Now);
    }
}