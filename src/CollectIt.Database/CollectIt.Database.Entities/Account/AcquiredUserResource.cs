using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollectIt.Database.Entities.Resources;
using NodaTime;

namespace CollectIt.Database.Entities.Account;

public class AcquiredUserResource
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [Required]
    public int ResourceId { get; set; }

    [ForeignKey(nameof(ResourceId))]
    public Resource? Resource { get; set; }
    
    [Required]
    public DateTime AcquiredDate { get; set; }
}