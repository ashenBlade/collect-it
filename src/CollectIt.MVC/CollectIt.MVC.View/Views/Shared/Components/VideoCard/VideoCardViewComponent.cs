using CollectIt.Database.Entities.Resources;
using CollectIt.MVC.View.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Views.Shared.Components.VideoCard;

public class VideoCardViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(VideoViewModel video)
    {
        return View(video);
    }
}