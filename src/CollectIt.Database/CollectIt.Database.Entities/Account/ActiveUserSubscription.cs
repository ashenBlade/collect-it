using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace CollectIt.Database.Entities.Account;

public class ActiveUserSubscription
{
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    
    public int SubscriptionId { get; set; }
    [ForeignKey(nameof(SubscriptionId))]
    public Subscription Subscription { get; set; }
    
    public int LeftResourcesCount { get; set; }
    
    public NpgsqlRange<DateTime> During { get; set; }
    
    public int MaxResourcesCount { get; set; }
}