using System.ComponentModel.DataAnnotations;

namespace CollectIt.MVC.View.ViewModels;

public class UploadImageViewModel
{
    [Required]
    [MinLength(6)]
    public string Name { get; set; }
    
    [Required]
    [DataType(DataType.Text)]
    public string Tags { get; set; }

    [Required]
    public IFormFile Content { get; set; }
}