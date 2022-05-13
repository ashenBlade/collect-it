using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Abstractions.Resources.Exceptions;
using CollectIt.MVC.View.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Controllers;

[Authorize]
[Route("acquire")]
public class ResourceAcquiringController : Controller
{
    private readonly ILogger<ResourceAcquiringController> _logger;
    private readonly IResourceAcquisitionService _resourceAcquisitionService;

    public ResourceAcquiringController(IResourceAcquisitionService resourceAcquisitionService,
                                       ILogger<ResourceAcquiringController> logger)
    {
        _resourceAcquisitionService = resourceAcquisitionService;
        _logger = logger;
    }

    [HttpPost("image/{imageId:int}")]
    public async Task<IActionResult> BuyImage([Required] int imageId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        _logger.LogInformation("User with id = {UserId} wants to acquire image with id = {ImageId}", userId, imageId);
        try
        {
            var acquired = await _resourceAcquisitionService.AcquireImageAsync(userId, imageId);
            _logger.LogInformation("User (Id = {UserId}) successfully acquired image (Id = {ImageId})", userId,
                                   imageId);
            return RedirectToAction("Image", "Images", new {imageId = imageId});
        }
        catch (UserAlreadyAcquiredResourceException alreadyAcquiredResourceException)
        {
            return View("PaymentResult",
                        new PaymentResultViewModel() {ErrorMessage = "Вы уже приобрели данный ресурс"});
        }
        catch (NoSuitableSubscriptionFoundException noSuitableSubscriptionFoundException)
        {
            return View("PaymentResult",
                        new PaymentResultViewModel() {ErrorMessage = "Нет подходящей подписки для покупки"});
        }
        catch (ResourceNotFoundException resourceNotFoundException)
        {
            return View("PaymentResult",
                        new PaymentResultViewModel() {ErrorMessage = "Запрашиваемый ресурс не найден"});
        }
    }

    [HttpPost("music/{musicId:int}")]
    public async Task<IActionResult> BuyMusic([Required] int musicId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        _logger.LogInformation("User with id = {UserId} wants to acquire image with id = {ImageId}", userId, musicId);
        try
        {
            var acquired = await _resourceAcquisitionService.AcquireMusicAsync(userId, musicId);
            _logger.LogInformation("User (Id = {UserId}) successfully acquired music (Id = {MusicId})", userId,
                                   musicId);
            return RedirectToAction("Music", "Musics", new {id = musicId});
        }
        catch (UserAlreadyAcquiredResourceException alreadyAcquiredResourceException)
        {
            return View("PaymentResult",
                        new PaymentResultViewModel() {ErrorMessage = "Вы уже приобрели данный ресурс"});
        }
        catch (NoSuitableSubscriptionFoundException noSuitableSubscriptionFoundException)
        {
            return View("PaymentResult",
                        new PaymentResultViewModel() {ErrorMessage = "Нет подходящей подписки для покупки"});
        }
        catch (ResourceNotFoundException resourceNotFoundException)
        {
            return View("PaymentResult",
                        new PaymentResultViewModel() {ErrorMessage = "Запрашиваемый ресурс не найден"});
        }
    }

    [HttpPost("video/{videoId:int}")]
    public async Task<IActionResult> BuyVideo([Required] int videoId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        _logger.LogInformation("User with id = {UserId} wants to acquire image with id = {VideoId}", userId, videoId);
        try
        {
            var acquired = await _resourceAcquisitionService.AcquireVideoAsync(userId, videoId);
            _logger.LogInformation("User (Id = {UserId}) successfully acquired image (Id = {VideoId})", userId,
                                   videoId);
            return RedirectToAction("Video", "Videos", new {id = videoId});
        }
        catch (UserAlreadyAcquiredResourceException alreadyAcquiredResourceException)
        {
            return View("PaymentResult",
                        new PaymentResultViewModel() {ErrorMessage = "Вы уже приобрели данный ресурс"});
        }
        catch (NoSuitableSubscriptionFoundException noSuitableSubscriptionFoundException)
        {
            return View("PaymentResult",
                        new PaymentResultViewModel() {ErrorMessage = "Нет подходящей подписки для покупки"});
        }
        catch (ResourceNotFoundException resourceNotFoundException)
        {
            return View("PaymentResult",
                        new PaymentResultViewModel() {ErrorMessage = "Запрашиваемый ресурс не найден"});
        }
    }
}