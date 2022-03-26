using CollectIt.Database.Entities.Account;

namespace CollectIt.MVC.View.Views.Shared.Components.SubscriptionCards;

public class SubscriptionCardsViewModel
{
    public IReadOnlyList<Subscription> Subscriptions { get; set; }
}