using System.ComponentModel.DataAnnotations;
using CollectIt.API.DTO;
using CollectIt.API.DTO.Mappers;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace CollectIt.API.WebAPI.Controllers.Account;

/// <summary>
/// Manage subscriptions in system
/// </summary>
[ApiController]
[Route("api/v1/subscriptions")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionManager _subscriptionManager;

    public SubscriptionsController(ISubscriptionManager subscriptionManager)
    {
        _subscriptionManager = subscriptionManager;
    }

    /// <summary>
    /// Get subscriptions list 
    /// </summary>
    /// <response code="200">Array of subscriptions ordered by id with max size of <paramref name="pageSize"/> </response>
    [HttpGet("")]
    public async Task<IActionResult> GetSubscriptionsPaged(
        [FromQuery(Name = "page_number")] [Range(1, int.MaxValue)] [Required]
        int pageNumber,
        [FromQuery(Name = "page_size")] [Range(1, int.MaxValue)] [Required]
        int pageSize)
    {
        var subscriptions = await _subscriptionManager.GetSubscriptionsAsync(pageNumber, pageSize);
        return Ok(subscriptions.Select(AccountMappers.ToReadSubscriptionDTO)
                               .ToArray());
    }


    /// <summary>
    /// Get active subscriptions list 
    /// </summary>
    /// <response code="200">Array of active subscriptions</response>
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveSubscriptions([FromQuery(Name = "type")] [Required] ResourceType type,
                                                            [FromQuery(Name = "page_size")] [Required]
                                                            int pageSize,
                                                            [FromQuery(Name = "page_number")] [Required]
                                                            int pageNumber)
    {
        var subscriptions =
            await _subscriptionManager.GetActiveSubscriptionsWithResourceTypeAsync(type, pageNumber, pageSize);
        return Ok(subscriptions.Select(AccountMappers.ToReadSubscriptionDTO)
                               .ToArray());
    }

    /// <summary>
    /// Find subscription by id
    /// </summary>
    /// <response code="404">Subscription not found</response>
    /// <response code="200">Subscription found</response>
    [HttpGet("{subscriptionId:int}")]
    public async Task<IActionResult> GetSubscriptionById(int subscriptionId)
    {
        var subscription = await _subscriptionManager.FindSubscriptionByIdAsync(subscriptionId);
        return subscription is null
                   ? NotFound()
                   : Ok(AccountMappers.ToReadSubscriptionDTO(subscription));
    }

    /// <summary>
    /// Create Subscription
    /// </summary>
    /// <response code="201">Subscription created</response>
    [HttpPost("")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CreateSubscription([FromForm] [Required] AccountDTO.CreateSubscriptionDTO dto,
                                                        [FromForm(Name = "active")] bool active = false)
    {
        var restriction = dto.Restriction is null
                              ? null
                              : AccountMappers.ToRestrictionFromCreateRestrictionDTO(dto.Restriction);
        var subscription = await _subscriptionManager.CreateSubscriptionAsync(dto.Name,
                                                                              dto.Description,
                                                                              dto.MonthDuration,
                                                                              dto.AppliedResourceType,
                                                                              dto.Price,
                                                                              dto.MaxResourcesCount,
                                                                              restriction,
                                                                              active);
        return CreatedAtAction("GetSubscriptionById", new {subscriptionId = subscription.Id},
                               AccountMappers.ToReadSubscriptionDTO(subscription));
    }


    /// <summary>
    /// Change name for subscription
    /// </summary>
    /// <response code="400">Something went wrong</response>
    /// <response code="204">Name changed</response>
    [HttpPost("{subscriptionId:int}/name")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangeSubscriptionName(int subscriptionId,
                                                            [FromForm(Name = "name")] [Required] string name)
    {
        var result = await _subscriptionManager.ChangeSubscriptionNameAsync(subscriptionId, name);
        return result.Succeeded
                   ? NoContent()
                   : BadRequest();
    }

    /// <summary>
    /// Change name for subscription
    /// </summary>
    /// <response code="400">Something went wrong</response>
    /// <response code="204">Description changed</response>
    [HttpPost("{subscriptionId:int}/description")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangeSubscriptionDescription(int subscriptionId,
                                                                   [FromForm(Name = "description")] [Required]
                                                                   string description)
    {
        var result = await _subscriptionManager.ChangeSubscriptionDescriptionAsync(subscriptionId, description);
        return result.Succeeded
                   ? NoContent()
                   : BadRequest();
    }

    /// <summary>
    /// Change name for subscription
    /// </summary>
    /// <response code="400">Something went wrong</response>
    /// <response code="204">Subscription activated</response>
    [HttpPost("{subscriptionId:int}/activate")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ActivateSubscription(int subscriptionId)
    {
        var result = await _subscriptionManager.ActivateSubscriptionAsync(subscriptionId);
        return result.Succeeded
                   ? NoContent()
                   : BadRequest();
    }

    /// <summary>
    /// Change name for subscription
    /// </summary>
    /// <response code="400">Something went wrong</response>
    /// <response code="204">Subscription deactivated</response>
    [HttpPost("{subscriptionId:int}/deactivate")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> DeactivateSubscription(int subscriptionId)
    {
        var result = await _subscriptionManager.DeactivateSubscriptionAsync(subscriptionId);
        return result.Succeeded
                   ? NoContent()
                   : BadRequest();
    }
}