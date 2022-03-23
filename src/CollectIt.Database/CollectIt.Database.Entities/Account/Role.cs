using Microsoft.AspNetCore.Identity;

namespace CollectIt.Database.Entities.Account;

public class Role : IdentityRole<int>
{
    public ICollection<User> Users { get; set; }
}