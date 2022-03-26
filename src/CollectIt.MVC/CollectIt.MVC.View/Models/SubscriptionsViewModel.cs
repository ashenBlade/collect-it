using CollectIt.Database.Entities.Account;

namespace CollectIt.MVC.View.Models;

public class SubscriptionsViewModel
{
    public IReadOnlyList<Subscription> Subscriptions { get; set; }
}