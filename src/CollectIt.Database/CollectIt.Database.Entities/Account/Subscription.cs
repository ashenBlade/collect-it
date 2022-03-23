using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CollectIt.MVC.Account.IdentityEntities;

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