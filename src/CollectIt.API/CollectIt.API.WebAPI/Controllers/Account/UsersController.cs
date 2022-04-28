using System.ComponentModel.DataAnnotations;
using CollectIt.API.DTO.Mappers;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure.Account.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using static CollectIt.API.DTO.AccountDTO;

namespace CollectIt.API.WebAPI.Controllers.Account;

/// <summary>
/// Manage users in system
/// </summary>
[ApiController]
[Route("api/v1/users")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly UserManager _userManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserManager userManager,
                           ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Find user by id
    /// </summary>
    /// <response code="404">User not found</response>
    /// <response code="200">User found</response>
    [HttpGet("{userId:int}")]
    [ProducesResponseType(typeof(ReadUserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(int userId)
    {
        var user = await _userManager.FindUserByIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }
        var roles = await _userManager.GetRolesAsync(user);
        return Ok(AccountMappers.ToReadUserDTO(user, roles.ToArray()));
    }

    /// <summary>
    /// Get users list 
    /// </summary>
    /// <response code="200">Array of users ordered by id with max size of <paramref name="pageSize"/> </response>
    [HttpGet("")]
    [ProducesResponseType(typeof(ReadUserDTO[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUsersPaged([FromQuery(Name = "page_number")][Range(1, int.MaxValue)]
                                                   int pageNumber = 1,
                                                   [FromQuery(Name = "page_size")] [Range(1, int.MaxValue)]
                                                   int pageSize = 10)
    {
        var users = ( await _userManager.GetUsersPaged(pageNumber, pageSize) )
           .Select(u => AccountMappers.ToReadUserDTO(u, u.Roles
                                                         .Select(r => r.Name)
                                                         .ToArray()));
        return Ok(users);
    }

    /// <summary>
    /// Get subscriptions for user (active and ended)  
    /// </summary>
    /// <response code="404">User not found</response>
    /// <response code="200">Array of subscriptions</response>
    [HttpGet("{userId:int}/subscriptions")]
    [ProducesResponseType(typeof(ReadUserSubscriptionDTO[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUsersSubscriptions(int userId)
    {
        try
        {
            var subscriptions = await _userManager.GetSubscriptionsForUserByIdAsync(userId);
            return Ok(subscriptions.Select(AccountMappers.ToReadUserSubscriptionDTO)
                                   .ToArray());
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Get subscriptions that in active state
    /// </summary>
    /// <response code="404">User not found</response>
    /// <response code="200">Array of subscriptions for user</response>
    [HttpGet("{userId:int}/active-subscriptions")]
    [ProducesResponseType(typeof(ReadUserSubscriptionDTO[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetActiveUserSubscription(int userId)
    {
        try
        {
            var activeSubscriptions = await _userManager.GetActiveSubscriptionsForUserByIdAsync(userId);
            return Ok(activeSubscriptions.Select(AccountMappers.ToReadUserSubscriptionDTOFromActiveUserSubscription));

        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Get roles of user
    /// </summary>
    /// <response code="404">User not found</response>
    /// <response code="200">Returns array of roles</response>
    [HttpGet("{userId:int}/roles")]
    [ProducesResponseType(typeof(ReadRoleDTO[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserRoles(int userId)
    {
        var roles = await _userManager.GetRolesAsync(userId);
        return Ok(roles.Select(AccountMappers.ToReadRoleDTO)
                       .ToArray());
    }
    
    /// <summary>
    /// Change username for user
    /// </summary>
    /// <response code="404">User not found</response>
    /// <response code="204">Username changed</response>
    /// <response code="403">Requester is not admin or user itself</response>
    /// <response code="400">Error occurred while changing email</response>
    [HttpPost("{userId:int}/username")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeUsername([FromForm(Name = "username")]
                                                    [Required]
                                                    string username, 
                                                    int userId)
    {
        try
        {
            var user = await _userManager.FindUserByIdAsync(userId);
            if (user is null)
            {
                return NotFound();
            }

            var requester = await _userManager.GetUserAsync(User);
            if (!( user.Id == requester.Id || await _userManager.IsInRoleAsync(requester, "ADMIN") ))
            {
                return Forbid();
            }

            await _userManager.ChangeUsernameAsync(userId, username);
            return NoContent();
        }
        catch (AccountException accountException)
        {
            return BadRequest(accountException.Message);
        }
    }

    /// <summary>
    /// Get acquired images for user
    /// </summary>
    /// <response code="404">User not found</response>
    /// <response code="200">Returns array of acquired images</response>
    [HttpGet("{userId:int}/images")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(ReadAcquiredUserResourceDTO[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAcquiredImages(int userId)
    {
        try
        {
            var acquiredImages = await _userManager.GetAcquiredUserImagesAsync(userId);
            return Ok(acquiredImages.Select(AccountMappers.ToReadAcquiredUserResourceDTO)
                                    .ToArray());
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Change email for user
    /// </summary>
    /// <response code="404">User not found</response>
    /// <response code="204">Email changed</response>
    /// <response code="403">Requester is not admin or user itself</response>
    /// <response code="400">Error occurred while changing email</response>
    [HttpPost("{userId:int}/email")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeUserEmail(int userId, 
                                                     [FromForm(Name = "email")]
                                                     [Required]
                                                     [DataType(DataType.EmailAddress)]
                                                     string email)
    {
        var user = await _userManager.FindUserByIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        var requester = await _userManager.GetUserAsync(User);
        if (!(requester.Id == user.Id || await _userManager.IsInRoleAsync(requester, Role.AdminRoleName)))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }
        var result = await _userManager.SetEmailAsync(user, email);
        return result.Succeeded
                   ? NoContent()
                   : BadRequest(result.Errors
                                      .Select(e => e.Description)
                                      .ToArray());
    }

    /// <summary>
    /// Assign role to user
    /// </summary>
    /// <response code="404">User not found</response>
    /// <response code="204">Role assigned to user</response>
    /// <response code="403">Requester does not have admin role</response>
    /// <response code="400">Error occurred while assigning role</response>
    [HttpPost("{userId:int}/roles")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignRoleToUser(int userId, 
                                                      [FromForm(Name = "role_name")]
                                                      [Required]
                                                      string role)
    {
        var user = await _userManager.FindUserByIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }
        var result = await _userManager.AddToRoleAsync(user, role);
        return result.Succeeded
                   ? NoContent()
                   : BadRequest(result.Errors
                                      .Select(err => err.Description)
                                      .ToArray());
    }
 
    /// <summary>
    /// Remove role from user
    /// </summary>
    /// <response code="404">User not found</response>
    /// <response code="204">Role removed from user</response>
    /// <response code="403">Requester does not have admin role</response>
    /// <response code="400">Error occurred while removing role</response>
    [HttpDelete("{userId:int}/roles")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveRoleFromUser(int userId, 
                                                        [FromForm(Name = "role_name")]
                                                        string role)
    {
        var user = await _userManager.FindUserByIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }
        var result = await _userManager.RemoveFromRoleAsync(user, role);
        return result.Succeeded
                   ? NoContent()
                   : BadRequest(result.Errors
                                      .Select(err => err.Description)
                                      .ToArray());
    }

    /// <summary>
    /// Activate user account so he can login
    /// </summary>
    /// <response code="404">User not found</response>
    /// <response code="204">Account was activated</response>
    /// <response code="403">Requester does not have admin role</response>
    /// <response code="400">Error occurred while activating account</response>
    [HttpPost("{userId:int}/activate")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActivateAccount(int userId)
    {
        var user = await _userManager.FindUserByIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }
        var result = await _userManager.SetLockoutEnabledAsync(user, false);
        return result.Succeeded
                   ? NoContent()
                   : BadRequest(result.Errors
                                      .Select(err => err.Description)
                                      .ToArray());
    }
    
    /// <summary>
    /// Deactivate user account so he can not login
    /// </summary>
    /// <response code="404">User not found</response>
    /// <response code="204">Account was deactivated</response>
    /// <response code="403">Requester does not have admin role</response>
    /// <response code="400">Error occurred while deactivating account</response>
    [HttpPost("{userId:int}/deactivate")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeactivateAccount(int userId)
    {
        var user = await _userManager.FindUserByIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }
        var result = await _userManager.SetLockoutEnabledAsync(user, true);
        return result.Succeeded
                   ? NoContent()
                   : BadRequest(result.Errors
                                      .Select(err => err.Description)
                                      .ToArray());
    }

    /// <summary>
    /// Find user by username
    /// </summary>
    /// <response code="200">User found</response>
    /// <response code="404">User with provided username does not exist</response>
    [HttpGet("with-username/{username}")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(ReadUserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FindUserByUsername([Required] string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user is null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Ok( AccountMappers.ToReadUserDTO(user, roles.ToArray()));
    }

    /// <summary>
    /// Find user by email
    /// </summary>
    /// <response code="200">User found</response>
    /// <response code="404">User with provided email does not exist</response>
    [HttpGet("with-email/{email}")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [ProducesResponseType(typeof(ReadUserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FindUserByEmail([EmailAddress][Required] string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return NotFound();
        }
    
        return Ok(AccountMappers.ToReadUserDTO(user, (await _userManager.GetRolesAsync(user)).ToArray()));
    }
}