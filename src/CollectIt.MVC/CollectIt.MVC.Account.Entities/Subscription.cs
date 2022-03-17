using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CollectIt.MVC.Account.IdentityEntities;

[Table("subscriptions", Schema = "accounts")]
public class Subscription
{
    [Key]
    [Column("subscription_id")]
    public int Id { get; set; }
    
    [Required]
    [Column("name")]
    public string Name { get; set; }
    [Column("description")]
    public string Description { get; set; }

    [Range(1, int.MaxValue)]
    [Column("duration")]
    public int Duration { get; set; }
    
    [Range(0, int.MaxValue)]
    [Column("price")]
    public int Price { get; set; }
    
    public ICollection<User> Subscribers { get; set; }
}