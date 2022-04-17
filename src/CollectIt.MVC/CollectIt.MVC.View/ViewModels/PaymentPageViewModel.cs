using CollectIt.Database.Entities.Account;

namespace CollectIt.MVC.View.ViewModels;

public class PaymentPageViewModel
{
    public User User { get; set; }
    public Subscription Subscription { get; set; }
}