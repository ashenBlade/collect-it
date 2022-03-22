using CollectIt.MVC.Account.IdentityEntities;

namespace CollectIt.MVC.View.Models;

public class PaymentPageViewModel
{
    public User User { get; set; }
    public Subscription Subscription { get; set; }
}