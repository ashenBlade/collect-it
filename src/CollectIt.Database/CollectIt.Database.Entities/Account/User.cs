using System.Collections;
using CollectIt.Database.Entities.Resources;
using Microsoft.AspNetCore.Identity;

namespace CollectIt.Database.Entities.Account;

public class User : IdentityUser<int>
{
   public ICollection<Resource> ResourcesAuthorOf { get; set; }
   public ICollection<Resource> AcquiredResources { get; set; }
   public ICollection<Subscription> Subscriptions { get; set; }
}