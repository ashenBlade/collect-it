using CollectIt.Database.Entities.Resources;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Views.Shared.Components.ImageCard;

public class ImageCardViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Image image)
    {
        return View(image);
    }
}