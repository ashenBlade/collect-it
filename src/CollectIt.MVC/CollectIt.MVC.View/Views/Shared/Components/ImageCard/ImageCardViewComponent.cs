using CollectIt.Database.Entities.Resources;
using CollectIt.MVC.View.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Views.Shared.Components.ImageCard;

public class ImageCardViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(ImageViewModel image)
    {
        return View(image);
    }
} 