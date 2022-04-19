using CollectIt.MVC.Entities.Account;

namespace CollectIt.MVC.View.ViewModels;

public class AccountResourcesViewModel
{
    public IReadOnlyList<AccountUserResource> Resources { get; set; }
}