
using CollectIt.Database.Entities.Account;

namespace CollectIt.MVC.View.ViewModels;

public class PaymentResultViewModel
{
    public UserSubscription? UserSubscription { get; set; }
    public bool Success => UserSubscription is not null;
    public string? ErrorMessage { get; set; }
}