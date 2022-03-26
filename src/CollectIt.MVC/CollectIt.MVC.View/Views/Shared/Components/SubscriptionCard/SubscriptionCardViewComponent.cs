using CollectIt.Database.Entities.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;

namespace CollectIt.MVC.View.Views.Shared.Components.SubscriptionCard;

public class SubscriptionCardViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Subscription subscription)
    {
        return View(subscription);
    }
}