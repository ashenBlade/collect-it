using System.ComponentModel.DataAnnotations;

namespace CollectIt.MVC.View.DTO;

public class CreateMusicDTO
{
    [Required]
    [MinLength(6)]
    public string Name { get; set; }
    
    [Required]
    public string[] Tags { get; set; }
    
    [Range(1, int.MaxValue)]
    [Required]
    public int Duration { get; set; }

    [Required]
    public IFormFile FormFile { get; set; }

    [Required]
    public string Extension { get; set; }
}