using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace CollectIt.MVC.Account.IdentityEntities;

public class UserSubscription
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    
    public int SubscriptionId { get; set; }
    [ForeignKey(nameof(SubscriptionId))]
    public Subscription Subscription { get; set; }

    public NpgsqlRange<DateTime> During { get; set; } 
}