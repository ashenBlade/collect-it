using CollectIt.MVC.Entities.Account;

namespace CollectIt.MVC.View.Models;

public class AccountResourcesViewModel
{
    public IReadOnlyList<AccountUserResources> Resources { get; set; }
}