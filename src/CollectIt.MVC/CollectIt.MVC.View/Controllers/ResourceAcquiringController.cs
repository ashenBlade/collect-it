using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure.Account.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace CollectIt.MVC.View.Controllers;

[Authorize]
[Route("acquire")]
public class ResourceAcquiringController : Controller
{
    private readonly IResourceAcquisitionService _resourceAcquisitionService;
    private readonly ILogger<ResourceAcquiringController> _logger;

    public ResourceAcquiringController(IResourceAcquisitionService resourceAcquisitionService,
                                       ILogger<ResourceAcquiringController> logger)
    {
        _resourceAcquisitionService = resourceAcquisitionService;
        _logger = logger;
    }

    [HttpPost("image/{imageId:int}")]
    public async Task<IActionResult> BuyImage([Required]int imageId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        try
        {
            var acquired = await _resourceAcquisitionService.AcquireImageAsync(userId, imageId);
            _logger.LogInformation("User (Id = {UserId}) successfully acquired image (Id = {ImageId})", userId, imageId);
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