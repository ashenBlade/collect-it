using System.Security.Claims;
using CollectIt.API.DTO;
using CollectIt.API.DTO.Mappers;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Infrastructure.Account;
using CollectIt.Database.Infrastructure.Account.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace CollectIt.API.WebAPI.Controllers.Account;


/// <summary>
/// Manage purchase
/// </summary>
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
[Route("api/v1/purchase")]
[ApiController]
public class PurchaseController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly UserManager _userManager;
    private readonly ILogger<PurchaseController> _logger;
    private readonly IResourceAcquisitionService _resourceAcquisitionService;

    public PurchaseController(ISubscriptionService subscriptionService,
                              UserManager userManager,
                              ILogger<PurchaseController> logger,
                              IResourceAcquisitionService resourceAcquisitionService)
    {
        _subscriptionService = subscriptionService;
        _userManager = userManager;
        _logger = logger;
        _resourceAcquisitionService = resourceAcquisitionService;
    }
    
    /// <summary>
    /// Subscribe User
    /// </summary>
    /// <response code="404">Subscription with id =  <paramref name="subscriptionId"/> not found</response>
    /// <response code="400">User already has such subscription active</response>
    /// <response code="200">User Subscribed</response>
    [HttpPost("subscription/{subscriptionId:int}")]
    public async Task<IActionResult> SubscribeUser(int subscriptionId)
    {
        var user = await _userManager.GetUserAsync(User);
        try
        {
            var subscription = await _subscriptionService.SubscribeUserAsync(user.Id, subscriptionId);
            _logger.LogInformation("User (UserId = {UserId}) successfully subscribed (SubscriptionId = {SubscriptionId}). Created user subscription id: {UserSubscriptionId}", user.Id, subscriptionId, subscription.Id);
            return Ok(AccountMappers.ToReadUserSubscriptionDTO(subscription));
        }
        catch (UserAlreadySubscribedException userAlreadySubscribedException)
        {
            return BadRequest("User already has such subscription active");
        }
        catch (SubscriptionNotFoundException subscriptionNotFoundException)
        {
            return NotFound($"Subscription with id = {subscriptionId} not found");
        }
    }

    /// <summary>
    /// Acquire Image
    /// </summary>
    /// <response code="404">Image with id = <paramref name="imageId"/> not found</response>
    /// <response code="400">No suitable subscriptions found to acquire image or user already acquired this image</response>
    /// <response code="204">Image acquired</response>
    [HttpPost("image/{imageId:int}")]
    public async Task<IActionResult> AcquireImage(int imageId)
    {
        var userId = (await _userManager.GetUserAsync(User)).Id;
        try
        {
            var acquired = await _resourceAcquisitionService.AcquireImageAsync(userId, imageId);
            _logger.LogInformation("User (Id = {UserId}) successfully acquired image (Id = {ImageId}). AcquiredUserResource Id = {AquiredUserResourceId}", userId, imageId, acquired.Id);
            return NoContent();
        }
        catch (UserAlreadyAcquiredResourceException alreadyAcquiredResourceException)
        {
            return BadRequest("User already acquired this image");
        }
        catch (NoSuitableSubscriptionFoundException noSuitableSubscriptionFoundException)
        {
            return BadRequest("No suitable subscriptions found to acquire image");
        }
        catch (ResourceNotFoundException resourceNotFoundException)
        {
            return NotFound("Image with provided id not found");
        }
    }
}