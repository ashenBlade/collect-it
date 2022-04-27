using CollectIt.Database.Entities.Resources;
using CollectIt.MVC.View.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Views.Shared.Components.MusicCard;

public class MusicCardViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(MusicViewModel music)
    {
        return View(music);
    }
} 