using System.Security.Claims;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.MVC.Account.Infrastructure;
using CollectIt.MVC.View.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
 
namespace CollectIt.MVC.View.Controllers;

public class PaymentController : Controller
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly UserManager _userManager;
    private readonly ISubscriptionManager _subscriptionManager;

    public PaymentController(ISubscriptionService subscriptionService,
                             UserManager userManager,
                             ISubscriptionManager subscriptionManager)
    {
        _subscriptionService = subscriptionService;
        _userManager = userManager;
        _subscriptionManager = subscriptionManager;
    }

    [HttpGet]
    [Route("subscriptions")]
    public async Task<IActionResult> GetPageWithSubscriptionCards()
    {
        var subscriptions = await _subscriptionManager.GetAllWithResourceTypeAsync(ResourceType.Image); 
        return View("Subscriptions", new SubscriptionsViewModel()
                                         {
                                             Subscriptions = subscriptions
                                         });
    }
    
    [HttpGet]
    [Authorize]
    [Route("subscribe")]
    public async Task<IActionResult> SubscribePage(int subscriptionId)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var subscription = await _subscriptionManager.FindSubscriptionByIdAsync(subscriptionId);
            if (user is null || subscription is null)
            {
                return BadRequest();
            }
            return View("Payment", new PaymentPageViewModel() {User = user, Subscription = subscription});
        }
        catch (UserSubscriptionException us)
        {
            return Content($"Error: {us.GetType()} userId: {us.UserId}, subscriptionId: {us.SubscriptionId}",
                           "text/plain");
        }
    }

    [HttpPost]
    [Route("subscribe")]
    [Authorize]
    public async Task<IActionResult> SubscribeLogic(int subscriptionId, bool declined)
    {
        if (declined)
        {
            return View("PaymentResult",
                        new PaymentResultViewModel() {ErrorMessage = "Пользователь отменил оформление подписки"});
        }

        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userSubscription = await _subscriptionService.SubscribeUserAsync(userId, subscriptionId);
            return View("PaymentResult", new PaymentResultViewModel() {UserSubscription = userSubscription});
        }
        catch (UserAlreadySubscribedException already)
        {
            return View("PaymentResult",
                        new PaymentResultViewModel()
                        {
                            ErrorMessage = "Пользователь уже имеет активную подписку такого типа"
                        });
        }
        catch (UserSubscriptionException us)
        {
            return View("PaymentResult",
                        new PaymentResultViewModel()
                        {
                            ErrorMessage = "Ошибка во время оформления подписки. Попробуйте позже."
                        });
        }
        catch (SubscriptionNotFoundException notFoundException)
        {
            return BadRequest();
        }
    }
}