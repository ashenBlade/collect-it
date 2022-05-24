using System.ComponentModel.DataAnnotations;
using CollectIt.MVC.Entities.Account;

namespace CollectIt.MVC.View.ViewModels;

public class AccountViewModel
{
    [Required]
    public string UserName { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    public IEnumerable<AccountUserSubscription> Subscriptions { get; set; }

    public IEnumerable<AccountUserResource> AcquiredResources { get; set; }

    public IEnumerable<AccountUserResource> UsersResources { get; set; }
    public IEnumerable<string> Roles { get; set; }
    public bool EmailConfirmed { get; set; }
}