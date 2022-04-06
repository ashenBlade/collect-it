using CollectIt.API.DTO.Mappers;
using CollectIt.Database.Infrastructure.Account.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollectIt.API.WebAPI.Controllers.Account;

[ApiController]
[Route("api/v1/roles")]
public class RolesController : ControllerBase
{
    private readonly RoleManager _roleManager;
    private readonly ILogger<RolesController> _logger;

    public RolesController(RoleManager roleManager,
                           ILogger<RolesController> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleManager.GetAllRoles();
        return Ok(roles.Select(AccountMappers.ToReadRoleDTO)
                       .ToArray());
    }

    [HttpGet("{roleId:int}")]
    public async Task<IActionResult> GetRoleById(int roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        return role is null
                   ? NotFound()
                   : Ok(AccountMappers.ToReadRoleDTO(role));
    }
}