using System.ComponentModel.DataAnnotations;

namespace CollectIt.Database.Entities.Resources;

public class Image
{
    [Key]
    public int ImageId { get; set; }
        
    [Required]
    public Resource Resource { get; set; }
}