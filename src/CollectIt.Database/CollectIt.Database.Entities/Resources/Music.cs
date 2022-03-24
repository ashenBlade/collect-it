using System.ComponentModel.DataAnnotations;

namespace CollectIt.Database.Entities.Resources;

public class Music
{
    [Key]
    public int MusicId { get; set; }
        
    [Required]
    public Resource Resource { get; set; }
        
    [Required]
    public TimeSpan Duration { get; set; }
}