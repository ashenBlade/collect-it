using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.API.WebAPI.DTO;

public class PasswordTokenFlowDTO
{
    [Required]
    [FromForm(Name = "password")]
    public string Password { get; set; }

    [FromForm(Name = "username")]
    [Required]
    [MinLength(6)]
    [MaxLength(20)]
    public string Username { get; set; }

    [Required]
    [FromForm(Name = "grant_type")]
    public string Grant_type { get; set; }
}