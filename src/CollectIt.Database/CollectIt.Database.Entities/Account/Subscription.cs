using System.ComponentModel.DataAnnotations;

namespace CollectIt.Database.Entities.Account;

public class Subscription
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int MonthDuration { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int Price { get; set; }

    [Required]
    public ResourceType AppliedResourceType { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int MaxResourcesCount { get; set; }
    public ICollection<User> Subscribers { get; set; }
}