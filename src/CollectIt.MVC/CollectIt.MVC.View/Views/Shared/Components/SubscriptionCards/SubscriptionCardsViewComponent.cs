using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Views.Shared.Components.SubscriptionCards;


public class SubscriptionCardsViewComponent : ViewComponent
{
    public SubscriptionCardsViewComponent()
    { }
    
    public IViewComponentResult Invoke(SubscriptionCardsViewModel model)
    {
        return View(model);
    }
}