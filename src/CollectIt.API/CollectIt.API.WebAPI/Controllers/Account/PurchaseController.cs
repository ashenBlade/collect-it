using CollectIt.API.DTO.Mappers;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Abstractions.Resources.Exceptions;
using CollectIt.Database.Infrastructure.Account;
using CollectIt.Database.Infrastructure.Account.Data;
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
    private readonly ILogger<PurchaseController> _logger;
    private readonly IResourceAcquisitionService _resourceAcquisitionService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly UserManager _userManager;

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
            _logger.LogInformation("User (UserId = {UserId}) successfully subscribed (SubscriptionId = {SubscriptionId}). Created user subscription id: {UserSubscriptionId}",
                                   user.Id, subscriptionId, subscription.Id);
            return Ok(AccountMappers.ToReadUserSubscriptionDTO(subscription));
        }
        catch (UserAlreadySubscribedException)
        {
            return BadRequest("User already has such subscription active");
        }
        catch (SubscriptionNotFoundException)
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
        var userId = ( await _userManager.GetUserAsync(User) ).Id;
        try
        {
            var acquired = await _resourceAcquisitionService.AcquireImageAsync(userId, imageId);
            _logger.LogInformation("User (Id = {UserId}) successfully acquired image (Id = {ImageId}). AcquiredUserResource Id = {AquiredUserResourceId}",
                                   userId, imageId, acquired.Id);
            return NoContent();
        }
        catch (UserAlreadyAcquiredResourceException)
        {
            return BadRequest("User already acquired this image");
        }
        catch (NoSuitableSubscriptionFoundException)
        {
            return BadRequest("No suitable subscriptions found to acquire image");
        }
        catch (ResourceNotFoundException)
        {
            return NotFound("Image with provided id not found");
        }
    }

    /// <summary>
    /// Acquire music
    /// </summary>
    /// <response code="404">Music with provided <paramref name="musicId">id</paramref> not found</response>
    /// <response code="400">No suitable subscriptions found to acquire music or user already acquired this music</response>
    /// <response code="204">Music acquired</response>
    [HttpPost("music/{musicId:int}")]
    public async Task<IActionResult> AcquireMusic(int musicId)
    {
        var userId = ( await _userManager.GetUserAsync(User) ).Id;
        try
        {
            var acquired = await _resourceAcquisitionService.AcquireMusicAsync(userId, musicId);
            _logger.LogInformation("User (Id = {UserId}) successfully acquired music (Id = {MusicId}). AcquiredUserResource Id = {AcquiredUserResourceId}",
                                   userId, musicId, acquired.Id);
            return NoContent();
        }
        catch (UserAlreadyAcquiredResourceException)
        {
            return BadRequest("User already acquired this music");
        }
        catch (NoSuitableSubscriptionFoundException)
        {
            return BadRequest("No suitable subscriptions found to acquire music");
        }
        catch (ResourceNotFoundException)
        {
            return NotFound("Music with provided id not found");
        }
    }

    /// <summary>
    /// Acquire video
    /// </summary>
    /// <response code="404">Video with provided <paramref name="videoId">id</paramref> not found</response>
    /// <response code="400">No suitable subscriptions found to acquire video or user already acquired this video</response>
    /// <response code="204">Video acquired</response>
    [HttpPost("video/{videoId:int}")]
    public async Task<IActionResult> AcquireVideo(int videoId)
    {
        var userId = ( await _userManager.GetUserAsync(User) ).Id;
        try
        {
            var acquired = await _resourceAcquisitionService.AcquireMusicAsync(userId, videoId);
            _logger.LogInformation("User (Id = {UserId}) successfully acquired video (Id = {MusicId}). AcquiredUserResource Id = {AcquiredUserResourceId}",
                                   userId, videoId, acquired.Id);
            return NoContent();
        }
        catch (UserAlreadyAcquiredResourceException)
        {
            return BadRequest("User already acquired this video");
        }
        catch (NoSuitableSubscriptionFoundException)
        {
            return BadRequest("No suitable subscriptions found to acquire video");
        }
        catch (ResourceNotFoundException)
        {
            return NotFound("Video with provided id not found");
        }
    }
}