using CollectIt.MVC.Account.Abstractions.Exceptions;
using CollectIt.MVC.Account.Abstractions.Interfaces;
using CollectIt.MVC.Account.Infrastructure;
using Microsoft.AspNetCore.DataProtection.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CollectIt.MVC.View.Controllers;

public class PaymentController : Controller
{
    private readonly ISubscriptionService _subscriptionService;

    public PaymentController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
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
            return Content(builder.ToString() ?? string.Empty, "text/html");
        }
        catch (UserSubscriptionException e)
        {
            var builder = new TagBuilder("div");
            builder.InnerHtml.Append("<p>Error " + e.UserId + " " + e.SubscriptionId + "</p>");
            return Content(builder.ToString() ?? string.Empty, "text/html");
        }
    }
}