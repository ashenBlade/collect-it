using CollectIt.Database.Entities.Account;

namespace CollectIt.MVC.View.ViewModels;

public class SubscriptionsViewModel
{
    public IReadOnlyList<Subscription> Subscriptions { get; set; }
}