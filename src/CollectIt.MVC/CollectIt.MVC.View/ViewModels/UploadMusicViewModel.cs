using System.ComponentModel.DataAnnotations;

namespace CollectIt.MVC.View.DTO;

public class UploadMusicViewModel
{
    [Required]
    [MinLength(6)]
    public string Name { get; set; }
    
    [Required]
    [DataType(DataType.Text)]
    public string Tags { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int Duration { get; set; }

    [Required]
    public IFormFile Content { get; set; }
}