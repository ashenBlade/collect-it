using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualBasic;
using NpgsqlTypes;
using DateInterval = NodaTime.DateInterval;

namespace CollectIt.Database.Entities.Account;

public class ActiveUserSubscription
{
    [Key]
    public int Id { get; set; }
    
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    
    public int SubscriptionId { get; set; }
    [ForeignKey(nameof(SubscriptionId))]
    public Subscription Subscription { get; set; }
    
    public int LeftResourcesCount { get; set; }
    
    public DateInterval During { get; set; }
}