using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CollectIt.MVC.Account.IdentityEntities;

[Table("users", Schema = "accounts")]
public class User : IdentityUser<int>
{
    [Key]
    [Column("user_id")]
    public override int Id { get; set; }

    [Column("email")]
    public override string Email { get; set; }
    [Column("normalized_email")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public override string NormalizedEmail { get; set; }

    [Column("username")]
    public override string UserName { get; set; }
    [Column("normalized_username")]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public override string NormalizedUserName { get; set; }

    [Column("password_hash")]
    public override string PasswordHash { get; set; }
    
    public ICollection<Subscription> Subscriptions { get; set; }
}