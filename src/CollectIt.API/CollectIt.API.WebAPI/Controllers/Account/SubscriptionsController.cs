using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using CollectIt.API.DTO;
using CollectIt.API.DTO.Mappers;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Entities.Account.Restrictions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;

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
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> CreateSubscription([FromForm]
                                                        [Required] 
                                                        AccountDTO.CreateSubscriptionDTO dto, 
                                                        [FromForm(Name = "active")]
                                                        bool active = false)
    {
        Restriction? restriction;
        try
        {
            restriction = dto.Restriction switch
                          {
                              AccountDTO.CreateAuthorRestrictionDTO createAuthorRestrictionDTO =>
                                  new AuthorRestriction() {AuthorId = createAuthorRestrictionDTO.AuthorId},
                              null => null,
                              var createRestrictionDTO => throw new UnsupportedContentTypeException($"Restriction type '{createRestrictionDTO.RestrictionType}' is unsupported"),
                          };
        }
        catch (SwitchExpressionException switchExpressionException)
        {
            return BadRequest(new {Error = $"Unknown restriction type: {dto.Restriction.RestrictionType}"});
        }
        catch (UnsupportedContentTypeException unsupportedContentTypeException)
        {
            return UnprocessableEntity($"Restriction type is not supported");
        }
        
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

    [HttpPost("{subscriptionId:int}/name")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangeSubscriptionName(int subscriptionId, 
                                                            [FromForm(Name = "name")]
                                                            [Required]
                                                            string name)
    {
        var result = await _subscriptionManager.ChangeSubscriptionNameAsync(subscriptionId, name);
        return result.Succeeded
                   ? NoContent()
                   : BadRequest();
    }
    
    [HttpPost("{subscriptionId:int}/description")]
    [Authorize(Roles = "Admin",  AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ChangeSubscriptionDescription(int subscriptionId,[FromForm(Name = "description")][Required] string description)
    {
        var result = await _subscriptionManager.ChangeSubscriptionDescriptionAsync(subscriptionId, description);
        return result.Succeeded
                   ? NoContent()
                   : BadRequest();
    }

    [HttpPost("{subscriptionId:int}/activate")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public async Task<IActionResult> ActivateSubscription(int subscriptionId)
    {
        var result = await _subscriptionManager.ActivateSubscriptionAsync(subscriptionId);
        return result.Succeeded
                   ? NoContent()
                   : BadRequest();
    }
    
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