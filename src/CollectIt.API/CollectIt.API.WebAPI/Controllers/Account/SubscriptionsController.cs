using System.ComponentModel.DataAnnotations;
using CollectIt.API.DTO.Mappers;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account;
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
        var subscriptions = await _subscriptionManager.GetActiveSubscriptionsWithResourceTypeAsync(resourceType, pageNumber, pageSize);
        return Ok(subscriptions.Select(AccountMappers.ToReadSubscriptionDTO)
                               .ToArray());
    }


    [HttpGet("active")]
    public async Task<IActionResult> GetActiveSubscriptions([FromQuery(Name = "type")][Required] ResourceType type)
    {
        var subscriptions = await _subscriptionManager.GetActiveSubscriptionsWithResourceTypeAsync(type);
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
}