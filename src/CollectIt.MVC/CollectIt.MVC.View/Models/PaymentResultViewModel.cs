using CollectIt.MVC.Account.IdentityEntities;

namespace CollectIt.MVC.View.Models;

public class PaymentResultViewModel
{
    public UserSubscription? UserSubscription { get; set; }
    public bool Success => UserSubscription is not null;
    public string? ErrorMessage { get; set; }
}