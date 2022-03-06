using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace CollectIt.MVC.Account.IdentityEntities;

[Table("active_users_subscriptions", Schema = "accounts")]
public class ActiveUserSubscription
{
    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey("user_id")]
    public User User { get; set; }
    [Column("subscription_id")]
    public int SubscriptionId { get; set; }

    [ForeignKey("subscription_id")]
    public Subscription Subscription { get; set; }

    [Column("used_resources_count")]
    public int UsedResourcesCount { get; set; }

    [Column("during")]
    public NpgsqlRange<DateTime> During { get; set; }

    [Column("max_resources_count")]
    public int MaxResourcesCount { get; set; }
    
    
}