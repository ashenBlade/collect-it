using System.ComponentModel.DataAnnotations;

namespace CollectIt.MVC.View.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Укажите почту")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Минимальная длина пароля - 6 символов")]
    public string? Password { get; set; }
    
    public bool RememberMe { get; set; }
}