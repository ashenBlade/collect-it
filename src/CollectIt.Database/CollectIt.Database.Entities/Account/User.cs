using Microsoft.AspNetCore.Identity;

namespace CollectIt.Database.Entities.Account;

public class User : IdentityUser<int>
{ 
   public ICollection<Subscription> Subscriptions { get; set; }
}