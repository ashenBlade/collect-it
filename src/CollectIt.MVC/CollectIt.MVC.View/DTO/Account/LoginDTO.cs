using System.ComponentModel.DataAnnotations;

namespace CollectIt.MVC.View.DTO.Account;

public class LoginDTO
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Password { get; set; }
}