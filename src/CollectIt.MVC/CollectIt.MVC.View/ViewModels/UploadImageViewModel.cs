using System.ComponentModel.DataAnnotations;

namespace CollectIt.MVC.View.ViewModels;

public class UploadImageViewModel
{
    [Required]
    [MinLength(6, ErrorMessage = "Минимальная длина названия изображения - 6 символов")]
    [MaxLength(20, ErrorMessage = "Максимальная длина названия изображения - 20 символов")]
    public string Name { get; set; }
    
    [Required]
    [DataType(DataType.Text)]
    public string Tags { get; set; }

    [Required]
    public IFormFile Content { get; set; }
}