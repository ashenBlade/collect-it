using Microsoft.AspNetCore.Identity;

namespace CollectIt.Database.Entities.Account;

public class Role : IdentityRole<int>
{
    public static string AdminRoleName => "Admin";
    public static int AdminRoleId => 1;
    public static string TechSupportRoleName => "Technical Support";
    public static int TechSupportRoleId => 2;
    public static string UserRoleName => "User";
    public static int UserRoleId => 3;
    public ICollection<User> Users { get; set; }
}