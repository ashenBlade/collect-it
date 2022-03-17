using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CollectIt.MVC.Account.IdentityEntities;

public class User : IdentityUser<int>
{ 
   public ICollection<Subscription> Subscriptions { get; set; }
}