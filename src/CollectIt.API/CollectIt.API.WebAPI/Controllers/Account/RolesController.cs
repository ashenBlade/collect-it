using CollectIt.API.DTO.Mappers;
using CollectIt.Database.Infrastructure.Account.Data;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.API.WebAPI.Controllers.Account;

/// <summary>
/// Manage roles in system
/// </summary>
[ApiController]
[Route("api/v1/roles")]
public class RolesController : ControllerBase
{
    private readonly RoleManager _roleManager;

    public RolesController(RoleManager roleManager)
    {
        _roleManager = roleManager;
    }


    /// <summary>
    /// Get roles list 
    /// </summary>
    /// <response code="200">Array of roles</response>
    [HttpGet("")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleManager.GetAllRoles();
        return Ok(roles.Select(AccountMappers.ToReadRoleDTO)
                       .ToArray());
    }

    /// <summary>
    /// Find role by id
    /// </summary>
    /// <response code="404">role not found</response>
    /// <response code="200">role found</response>
    [HttpGet("{roleId:int}")]
    public async Task<IActionResult> GetRoleById(int roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        return role is null
                   ? NotFound()
                   : Ok(AccountMappers.ToReadRoleDTO(role));
    }
}