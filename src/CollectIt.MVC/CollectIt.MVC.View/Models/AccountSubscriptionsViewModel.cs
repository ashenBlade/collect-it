using CollectIt.MVC.Entities.Account;

namespace CollectIt.MVC.View.Models;

public class AccountSubscriptionsViewModel
{
    public IReadOnlyList<AccountUserSubscription> Subscriptions { get; set; }
}