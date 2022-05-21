using System.ComponentModel.DataAnnotations;

namespace CollectIt.MVC.View.ViewModels;

public class UploadMusicViewModel
{
    [Required]
    [MinLength(6, ErrorMessage = "Минимальная длина названия музыки - 6 символов")]
    [MaxLength(20, ErrorMessage = "Максимальная длина названия музыки - 20 символов")]
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