using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace CollectIt.MVC.Account.IdentityEntities;

[Table("roles", Schema = "accounts")]
public class Role : IdentityRole<int>
{
    [Key]
    [Column("role_id")]
    public override int Id { get; set; }

    [Column("name")]
    public override string Name { get; set; }

    [Column("normalized_name")]
    public override string NormalizedName { get; set; }
    
    public ICollection<User> Users { get; set; }
}