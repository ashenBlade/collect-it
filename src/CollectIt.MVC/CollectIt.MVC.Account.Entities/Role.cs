using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CollectIt.MVC.Account.IdentityEntities;

public class Role : IdentityRole<int>
{
    public ICollection<User> Users { get; set; }
}