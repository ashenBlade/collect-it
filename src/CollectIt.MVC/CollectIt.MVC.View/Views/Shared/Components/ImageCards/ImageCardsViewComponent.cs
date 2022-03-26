using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Views.Shared.Components.ImageCards;

public class ImageCardsViewComponent : ViewComponent
{
    public ImageCardsViewComponent()
    { }

    public IViewComponentResult Invoke()
    {
        return View();
    }
}