using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Views.Shared.Components.VideoCards;

public class VideoCardsViewComponent : ViewComponent
{
    public VideoCardsViewComponent()
    { }

    public IViewComponentResult Invoke()
    {
        return View();
    }
}