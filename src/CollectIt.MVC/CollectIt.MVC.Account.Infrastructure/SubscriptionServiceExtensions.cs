using CollectIt.MVC.Account.Abstractions.Interfaces;
using CollectIt.MVC.Account.IdentityEntities;

namespace CollectIt.MVC.Account.Infrastructure;

public static class SubscriptionServiceExtensions
{
    public static Task<UserSubscription> SubscribeUserAsync(this ISubscriptionService service,
                                                            int userId,
                                                            int subscriptionId)
    {
        return service.SubscribeUserAsync(userId, subscriptionId, DateTime.Now);
    }
}