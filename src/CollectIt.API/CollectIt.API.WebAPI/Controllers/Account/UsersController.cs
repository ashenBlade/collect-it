using System.ComponentModel.DataAnnotations;
using CollectIt.API.DTO;
using CollectIt.API.DTO.Mappers;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure.Account.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CollectIt.API.WebAPI.Controllers.Account;

[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly UserManager _userManager;
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserManager userManager, 
                           ISubscriptionManager subscriptionManager,
                           ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _subscriptionManager = subscriptionManager;
        _logger = logger;
    }

    [HttpGet("{userId:int}")]
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

    [HttpGet("")]
    public async Task<IActionResult> GetUsersPaged([FromQuery(Name = "page_number")][Range(1, int.MaxValue)]int pageNumber = 1,
                                                   [FromQuery(Name = "page_size")][Range(1, int.MaxValue)] int pageSize = 10)
    {
        var users = ( await _userManager.GetUsersPaged(pageNumber, pageSize) )
           .Select(u => AccountMappers.ToReadUserDTO(u, u.Roles
                                                         .Select(r => r.Name)
                                                         .ToArray()));
        return Ok(users);
    }

    [HttpGet("{userId:int}/subscriptions")]
    public async Task<IActionResult> GetUsersSubscriptions(int userId)
    {
        var subscriptions = await _userManager.GetSubscriptionsForUserByIdAsync(userId);
        return Ok(subscriptions);
    }

    [HttpGet("{userId:int}/active-subscriptions")]
    public async Task<IActionResult> GetActiveUserSubscription(int userId)
    {
        var activeSubscriptions = await _userManager.GetActiveSubscriptionsForUserByIdAsync(userId);
        return Ok(activeSubscriptions.Select(AccountMappers.ToReadUserSubscriptionDTOFromActiveUserSubscription));
    }

    [HttpGet("{userId:int}/roles")]
    public async Task<IActionResult> GetUserRoles(int userId)
    {
        var roles = await _userManager.GetRolesAsync(userId);
        return Ok(roles.Select(AccountMappers.ToReadRoleDTO)
                       .ToArray());
    }

    [HttpPut("{userId:int}/username")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeUsername([FromQuery(Name = "username")]string username, 
                                                    int userId)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
            {
                return NotFound();
            }
            await _userManager.ChangeUsernameAsync(userId, username);
            return NoContent();
        }
        catch (UserNotFoundException notFoundException)
        {
            return NotFound();
        }
        catch (AccountException accountException)
        {
            return BadRequest("User with provided username already exists");
        }
    }
}