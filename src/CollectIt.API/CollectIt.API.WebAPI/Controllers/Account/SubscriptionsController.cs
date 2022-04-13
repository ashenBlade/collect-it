using System.ComponentModel.DataAnnotations;
using CollectIt.API.DTO;
using CollectIt.API.DTO.Mappers;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.API.WebAPI.Controllers.Account;

[ApiController]
[Route("api/v1/subscriptions")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly ILogger<SubscriptionsController> _logger;

    public SubscriptionsController(ISubscriptionManager subscriptionManager,
                                   ILogger<SubscriptionsController> logger)
    {
        _subscriptionManager = subscriptionManager;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetSubscriptionsPaged([FromQuery(Name = "page_number")] 
                                                           [Range(1, int.MaxValue)]
                                                           [Required]
                                                           int pageNumber,
                                                           
                                                           [FromQuery(Name = "page_size")] 
                                                           [Range(1, int.MaxValue)]
                                                           [Required]
                                                           int pageSize,
                                                           
                                                           [FromQuery(Name = "type")]
                                                           [Required(ErrorMessage = "Please, specify resource type")]
                                                           ResourceType resourceType)
    {
        var subscriptions = await _subscriptionManager.GetSubscriptionsAsync(pageNumber, pageSize);
        return Ok(subscriptions.Select(AccountMappers.ToReadSubscriptionDTO)
                               .ToArray());
    }


    [HttpGet("active")]
    public async Task<IActionResult> GetActiveSubscriptions([FromQuery(Name = "type")][Required] ResourceType type,
                                                            [FromQuery(Name = "page_size")][Required]int pageSize,
                                                            [FromQuery(Name = "page_number")][Required]int pageNumber)
    {
        var subscriptions = await _subscriptionManager.GetActiveSubscriptionsWithResourceTypeAsync(type, pageNumber, pageSize);
        return Ok(subscriptions.Select(AccountMappers.ToReadSubscriptionDTO)
                               .ToArray());
    }

    [HttpGet("{subscriptionId:int}")]
    public async Task<IActionResult> GetSubscriptionById(int subscriptionId)
    {
        var subscription = await _subscriptionManager.FindSubscriptionByIdAsync(subscriptionId);
        
        return subscription is null
                   ? NotFound()
                   : Ok(AccountMappers.ToReadSubscriptionDTO(subscription));
    }

    [HttpPost("")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateSubscription([FromForm][Required] AccountDTO.CreateSubscriptionDTO dto, 
                                                        [FromForm(Name = "active")]bool? active)
    {
        var subscription = await _subscriptionManager.CreateSubscriptionAsync(dto.Name, 
                                                                              dto.Description, 
                                                                              dto.MonthDuration,
                                                                              dto.AppliedResourceType, 
                                                                              dto.MaxResourcesCount,
                                                                              dto.RestrictionId, 
                                                                              active ?? false);
        return CreatedAtAction("GetSubscriptionById", new {subscriptionId = subscription.Id},
                               AccountMappers.ToReadSubscriptionDTO(subscription));
    }

    [HttpPut("{subscriptionId:int}/name")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeSubscriptionName(int subscriptionId, string name)
    {
        var result = await _subscriptionManager.ChangeSubscriptionNameAsync(subscriptionId, name);
        return result.Succeeded
                   ? NoContent()
                   : BadRequest();
    }
    
    [HttpPut("{subscriptionId:int}/description")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangeSubscriptionDescription(int subscriptionId, string description)
    {
        var result = await _subscriptionManager.ChangeSubscriptionDescriptionAsync(subscriptionId, description);
        return result.Succeeded
                   ? NoContent()
                   : BadRequest();
    }

    [HttpPost("{subscriptionId:int}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ActivateSubscription(int subscriptionId)
    {
        var result = await _subscriptionManager.ActivateSubscriptionAsync(subscriptionId);
        return result.Succeeded
                   ? NoContent()
                   : BadRequest();
    }
    
    [HttpPost("{subscriptionId:int}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateSubscription(int subscriptionId)
    {
        var result = await _subscriptionManager.DeactivateSubscriptionAsync(subscriptionId);
        return result.Succeeded
                   ? NoContent()
                   : BadRequest();
    }
}