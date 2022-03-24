using System.Security.Claims;
using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.MVC.Account.Infrastructure;
using CollectIt.MVC.View.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
 
namespace CollectIt.MVC.View.Controllers;

public class PaymentController : Controller
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public PaymentController(ISubscriptionService subscriptionService,
                             IUserRepository userRepository,
                             ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionService = subscriptionService;
        _userRepository = userRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    public IActionResult Subscriptions()
    {
        return View();
    }
    
    [HttpGet]
    [Authorize]
    [Route("subscribe")]
    public async Task<IActionResult> SubscribePage(int subscriptionId)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _userRepository.FindByIdAsync(userId);
            var subscription = await _subscriptionRepository.FindByIdAsync(subscriptionId);
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