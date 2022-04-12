using CollectIt.MVC.Entities.Account;

namespace CollectIt.MVC.View.Models;

public class AccountResourcesViewModel
{
    public IReadOnlyList<AccountUserResource> Resources { get; set; }
}