using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure.Account.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Controllers;

[Authorize]
[Route("acquire")]
public class ResourceAcquiringController : Controller
{
    private readonly IImageManager _imageManager;
    private readonly UserManager _userManager;
    private readonly IResourceAcquisitionService _resourceAcquisitionService;
    private readonly ILogger<ResourceAcquiringController> _logger;

    public ResourceAcquiringController(IImageManager imageManager,
                                       UserManager userManager,
                                       IResourceAcquisitionService resourceAcquisitionService,
                                       ILogger<ResourceAcquiringController> logger)
    {
        _imageManager = imageManager;
        _userManager = userManager;
        _resourceAcquisitionService = resourceAcquisitionService;
        _logger = logger;
    }

    [HttpPost("image")]
    public async Task<IActionResult> BuyImage([FromQuery(Name = "image_id")][Required]int imageId)
    {
        var user = await _userManager.GetUserAsync(User);
        Console.WriteLine(imageId);
        var image = await _imageManager.FindByIdAsync(imageId);
        if (image is null)
        {
            throw new ResourceNotFoundException(imageId, "Image not found");
        }
        try
        {
            var acquired = await _resourceAcquisitionService.AcquireImageAsync(user.Id, imageId);
            _logger.LogInformation("User (Id = {UserId}) acquired image (Id = {ImageId})", user.Id, imageId);
            return NoContent();
        }
        catch (UserAlreadyAcquiredResourceException alreadyAcquiredResourceException)
        {
            // _logger.LogError(alreadyAcquiredResourceException, "User attempted to acquire already aquired resource");
            return BadRequest("User already acquired this image");
        }
        catch (NoSuitableSubscriptionFoundException noSuitableSubscriptionFoundException)
        {
            return BadRequest("No suitable subscriptions found to acquire image");
        }
    }
}