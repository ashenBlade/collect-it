using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure.Account.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Controllers;

[Authorize]
public class AcquireImageController : Controller
{
    private readonly IImageManager _imageManager;
    private readonly UserManager _userManager;
    private readonly IResourceAcquisitionService _resourceAcquisitionService;
    private readonly ILogger<AcquireImageController> _logger;

    public AcquireImageController(IImageManager imageManager,
                                  UserManager userManager,
                                  IResourceAcquisitionService resourceAcquisitionService,
                                  ILogger<AcquireImageController> logger)
    {
        _imageManager = imageManager;
        _userManager = userManager;
        _resourceAcquisitionService = resourceAcquisitionService;
        _logger = logger;
    }

    [HttpPost("acquire-image")]
    public async Task<IActionResult> BuyImage([FromQuery(Name = "image_id")]int imageId)
    {
        var user = await _userManager.GetUserAsync(User);
        var subscriptions = await _userManager.GetActiveSubscriptionsForUserByIdAsync(user.Id);
        var image = await _imageManager.FindByIdAsync(imageId);
        var affordable =
            subscriptions.FirstOrDefault(s => s.Subscription.Restriction is not null
                                           || s.Subscription.Restriction.IsSatisfiedBy(image));
        if (affordable is null)
        {
            return BadRequest("No suitable subscription to acquire resource found");
        }

        try
        {
            var acquired = await _resourceAcquisitionService.AcquireImageAsync(user.Id, imageId);
            _logger.LogInformation("User (Id = {UserId}) acquired image (Id = {ImageId})", user.Id, image.Id);
            return NoContent();
        }
        catch (UserAlreadyAcquiredResourceException alreadyAcquiredResourceException)
        {
            _logger.LogError(alreadyAcquiredResourceException, "User attempted to acquire already aquired resource");
            return BadRequest();
        }
        catch (NoSuitableSubscriptionFoundException noSuitableSubscriptionFoundException)
        {
            return BadRequest("No suitable subscriptions found to acquire image");
        }
    }
}