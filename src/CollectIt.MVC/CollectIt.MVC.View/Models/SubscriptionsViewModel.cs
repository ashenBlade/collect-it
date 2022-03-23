using CollectIt.MVC.Entities.Account;

namespace CollectIt.MVC.View.Models;

public class SubscriptionsViewModel
{
    public IEnumerable<Subscription> Subscriptions { get; set; }
}