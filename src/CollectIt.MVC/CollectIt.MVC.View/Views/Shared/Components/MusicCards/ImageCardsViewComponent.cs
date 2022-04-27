using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Views.Shared.Components.MusicCards;

public class MusicCardsViewComponent : ViewComponent
{
    public MusicCardsViewComponent()
    { }

    public IViewComponentResult Invoke()
    {
        return View();
    }
}