using System.ComponentModel.DataAnnotations;

namespace CollectIt.MVC.View.Models;

public class ProfileAccountViewModel
{
    [Required]
    public string Username { get; set; }
    
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
}