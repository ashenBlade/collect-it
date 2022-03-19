using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace CollectIt.MVC.Account.IdentityEntities;

public class UserSubscription
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    
    [Required]
    public int SubscriptionId { get; set; }
    [ForeignKey(nameof(SubscriptionId))]
    public Subscription Subscription { get; set; }

    [Required]
    public NpgsqlRange<DateTime> During { get; set; }
    
    [Range(0, int.MaxValue)]
    public int LeftResourcesCount { get; set; }
}