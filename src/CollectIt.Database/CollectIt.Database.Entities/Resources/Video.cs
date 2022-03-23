using System.ComponentModel.DataAnnotations;

namespace CollectIt.Database.Entities.Resources;

public class Video 
{
    [Key]
    public int VideoId { get; set; }
        
    [Required]
    public Resource Resource { get; set; }
        
    [Required]
    public TimeSpan Duration { get; set; }
}