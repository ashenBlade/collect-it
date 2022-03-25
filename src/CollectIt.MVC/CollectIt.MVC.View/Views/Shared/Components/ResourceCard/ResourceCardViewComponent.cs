//using CollectIt.Database.Entities.Resources;
using CollectIt.MVC.View.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;

namespace CollectIt.MVC.View.Views.Shared.Components.ResourceCard;

public class ResourceCardViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}