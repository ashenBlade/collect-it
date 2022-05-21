using System.ComponentModel.DataAnnotations;

namespace CollectIt.MVC.View.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Адрес почты обязателен")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Пароль обязателен")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Минимальная длина пароля - 6 символов")]
    public string? Password { get; set; }

    [Required]
    [DataType(DataType.Text)]
    [MinLength(6, ErrorMessage = "Минимальная длина имени пользователя - 6 символов")]
    [MaxLength(20, ErrorMessage = "Максимальная длина имени пользователя - 20 символов")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Подтвердите пароль")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
    public string? ConfirmPassword { get; set; } 
}