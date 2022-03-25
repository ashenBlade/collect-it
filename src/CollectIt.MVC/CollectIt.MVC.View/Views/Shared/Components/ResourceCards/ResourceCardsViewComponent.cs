using CollectIt.MVC.View.Views.Shared.Components.SubscriptionCards;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Views.Shared.Components.ResourceCards;

public class ResourceCardsViewComponent : ViewComponent
{
    public ResourceCardsViewComponent()
    { }
    
    public IViewComponentResult Invoke(ResourceCardsViewModel resource)
    {
         return View(resource);
    }
    
    public IViewComponentResult Invoke()
    {
        return View();
    }
}