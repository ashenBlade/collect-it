using CollectIt.MVC.Account.Abstractions.Exceptions;
using CollectIt.MVC.Account.Abstractions.Interfaces;
using CollectIt.MVC.Account.Infrastructure;
using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.WebEncoders.Testing;

namespace CollectIt.MVC.View.Controllers;

public class PaymentController : Controller
{
    private readonly ISubscriptionService _subscriptionService;

    public PaymentController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    public IActionResult Subscriptions()
    {
        return View();
    }
    
    [HttpGet]
    [Route("subscribe")]
    public async Task<IActionResult> Subscribe(int userId, int subscriptionId)
    {
        try
        {
            var us = await _subscriptionService.SubscribeUserAsync(userId, subscriptionId);
            var builder = new TagBuilder("div");
            builder.InnerHtml.Append($"<p>Successfully: Id is {us.Id}</p>");
            return Ok();
        }
        catch (UserSubscriptionException e)
        {
            return Content($"Error: {e.GetType()} userId: {e.UserId}, subscriptionId: {e.SubscriptionId}", "text/plain");
        }
    }
}