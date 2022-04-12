using System.Security.Claims;
using CollectIt.API.DTO.Mappers;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.MVC.Account.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.API.WebAPI.Controllers.Account;


[Authorize]
[Route("payment")]
public class PaymentController : Controller
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly UserManager _userManager;
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly ILogger<PaymentController> _logger;
    private readonly IResourceAcquisitionService _resourceAcquisitionService;

    public PaymentController(ISubscriptionService subscriptionService,
                             UserManager userManager,
                             ISubscriptionManager subscriptionManager,
                             ILogger<PaymentController> logger,
                             IResourceAcquisitionService resourceAcquisitionService,
                             IImageManager imageManager)
    {
        _subscriptionService = subscriptionService;
        _userManager = userManager;
        _subscriptionManager = subscriptionManager;
        _logger = logger;
        _resourceAcquisitionService = resourceAcquisitionService;
    }
    
    [HttpPost]
    [Route("subscribe")]
    public async Task<IActionResult> SubscribeUser(int subscriptionId)
    {
        var user = await _userManager.GetUserAsync(User);
        try
        {
            var subscription = await _subscriptionService.SubscribeUserAsync(user.Id, subscriptionId);
            return Ok(AccountMappers.ToReadUserSubscriptionDTO(subscription));
        }
        catch (NullReferenceException nullReferenceException)
        {
            return NotFound("User with provided id not found");
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

    [HttpPost("acquire/image/{imageId:int}")]
    public async Task<IActionResult> AcquireImage(int imageId)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var acquired = await _resourceAcquisitionService.AcquireImageAsync(userId, imageId);
            _logger.LogInformation("User (Id = {UserId}) successfully acquired image (Id = {ImageId}). AcquiredUserResource Id = {AquiredUserResourceId}", userId,
                                   imageId, acquired.Id);
            return NoContent();
        }
        catch (FormatException formatException)
        {
            _logger.LogError(formatException, "Could not parse user id from user");
            await HttpContext.SignOutAsync();
            return UnprocessableEntity();
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