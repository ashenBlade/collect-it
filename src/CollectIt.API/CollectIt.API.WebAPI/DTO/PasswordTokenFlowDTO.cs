using Microsoft.AspNetCore.Mvc;

namespace CollectIt.API.WebAPI.DTO;

public class PasswordTokenFlowDTO
{
    [FromForm(Name = "password")]
    public string Password { get; set; }

    [FromForm(Name = "username")]
    public string Username { get; set; }

    [FromForm(Name = "grant_type")]
    public string Grant_type { get; set; }
}