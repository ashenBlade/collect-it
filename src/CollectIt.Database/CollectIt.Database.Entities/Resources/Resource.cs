using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollectIt.Database.Entities.Account;

namespace CollectIt.Database.Entities.Resources;

public class Resource
{
    [Key]
    public int ResourceId { get; set; }
        
    [Required]
    public User ResourceOwner { get; set; }

    [ForeignKey(nameof(ResourceOwner))]
    public int ResourceOwnerId { get; set; }
    [Required]
    public string ResourcePath { get; set; }
        
    [Required]
    public DateTime UploadDate { get; set; }
}