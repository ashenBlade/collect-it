using System.ComponentModel.DataAnnotations;

namespace CollectIt.MVC.View.DTO.Account;

public class RegisterDTO
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}