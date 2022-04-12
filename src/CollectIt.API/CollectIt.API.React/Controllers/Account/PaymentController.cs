using System.Security.Claims;
using CollectIt.API.DTO.Mappers;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.MVC.Account.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.API.React.Controllers.Account;


    [Authorize]
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
    [Route("/subscribe")]
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
}