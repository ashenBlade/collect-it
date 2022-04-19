using CollectIt.MVC.Entities.Account;

namespace CollectIt.MVC.View.ViewModels;

public class AccountSubscriptionsViewModel
{
    public IReadOnlyList<AccountUserSubscription> Subscriptions { get; set; }
}