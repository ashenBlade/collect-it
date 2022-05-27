using CollectIt.Database.Abstractions.Account.Exceptions;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Infrastructure.Account;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.MVC.View.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Controllers;

[Authorize]
[Route("payment")]
public class PaymentController : Controller
{
    private readonly ILogger<PaymentController> _logger;
    private readonly IResourceAcquisitionService _resourceAcquisitionService;
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly ISubscriptionService _subscriptionService;
    private readonly UserManager _userManager;

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

    [HttpGet("subscriptions")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPageWithSubscriptionCards([FromQuery] ResourceType type)
    {
        var subscriptions = await _subscriptionManager.GetActiveSubscriptionsWithResourceTypeAsync(type);
        return View("Subscriptions", new SubscriptionsViewModel() {Subscriptions = subscriptions});
    }

    [HttpGet("subscriptions/{subscriptionId:int}")]
    public async Task<IActionResult> SubscribePage(int subscriptionId)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
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

    [HttpPost("subscriptions/{subscriptionId:int}")]
    public async Task<IActionResult> SubscribeLogic(int subscriptionId, bool declined)
    {
        if (declined)
        {
            return View("PaymentResult",
                        new PaymentResultViewModel {ErrorMessage = "Пользователь отменил оформление подписки"});
        }
        try
        {
            var userId = int.Parse(_userManager.GetUserId(User));
            var userSubscription = await _subscriptionService.SubscribeUserAsync(userId, subscriptionId);
            return View("PaymentResult", new PaymentResultViewModel() {UserSubscription = userSubscription});
        }
        catch (UserAlreadySubscribedException)
        {
            return View("PaymentResult",
                        new PaymentResultViewModel()
                        {
                            ErrorMessage = "Пользователь уже имеет активную подписку такого типа"
                        });
        }
        catch (UserSubscriptionException)
        {
            return View("PaymentResult",
                        new PaymentResultViewModel()
                        {
                            ErrorMessage = "Ошибка во время оформления подписки. Попробуйте позже."
                        });
        }
        catch (SubscriptionNotFoundException)
        {
            return View("PaymentResult", new PaymentResultViewModel() {ErrorMessage = "Данной подписки не существует"});
        }
        catch (SubscriptionIsNotActiveException)
        {
            return View("PaymentResult",
                        new PaymentResultViewModel() {ErrorMessage = "Подписка не доступна к оформлению"});
        }
    }
}