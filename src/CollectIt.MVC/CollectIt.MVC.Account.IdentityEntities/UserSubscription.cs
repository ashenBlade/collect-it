using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace CollectIt.MVC.Account.IdentityEntities;

[Table("users_subscriptions", Schema = "accounts")]
public class UserSubscription
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("user_id")]
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    
    [Column("subscription_id")]
    public int SubscriptionId { get; set; }

    [ForeignKey(nameof(SubscriptionId))]
    public Subscription Subscription { get; set; }
    
    [Column("during")]
    public NpgsqlRange<DateTime> During { get; set; } 
}