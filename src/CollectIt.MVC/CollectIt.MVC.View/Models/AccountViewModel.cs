using System.ComponentModel.DataAnnotations;

namespace CollectIt.MVC.View.Models;

public class AccountViewModel
{
    [Required]
    public string UserName { get; set; }
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required]
    public IEnumerable<Subscription> Subscriptions { get; set; }
}