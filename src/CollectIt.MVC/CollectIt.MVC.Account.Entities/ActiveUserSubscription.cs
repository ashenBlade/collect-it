using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace CollectIt.MVC.Account.IdentityEntities;

public class ActiveUserSubscription
{
    public int UserId { get; set; }
    
    public User User { get; set; }
    public int SubscriptionId { get; set; }
    [ForeignKey(nameof(SubscriptionId))]
    public Subscription Subscription { get; set; }
    
    public int UsedResourcesCount { get; set; }
    
    public NpgsqlRange<DateTime> During { get; set; }
    
    public int MaxResourcesCount { get; set; }
}