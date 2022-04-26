using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CollectIt.API.WebAPI.DTO;

public class RegisterDTO
{
    [Required]
    [MinLength(6)]
    public string Username { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
}