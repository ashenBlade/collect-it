using System.ComponentModel.DataAnnotations;

namespace CollectIt.MVC.Resources.Entities;

public class Music
{
    [Key]
    public int MusicId { get; set; }
        
    [Required]
    public Resource Resource { get; set; }
        
    [Required]
    public TimeSpan Duration { get; set; }
}