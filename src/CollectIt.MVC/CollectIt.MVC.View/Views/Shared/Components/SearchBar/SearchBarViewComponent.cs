using CollectIt.Database.Entities.Account;
using CollectIt.MVC.View.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Views.Shared.Components.SearchBar;

[ViewComponent(Name = "SearchBar")]
public class SearchBarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(string? action = null,
                                       string? controller = null,
                                       string? query = null,
                                       ResourceType chosenType = ResourceType.Image)
    {
        var type = controller switch
                   {
                       "Musics" => ResourceType.Music,
                       "Videos" => ResourceType.Video,
                       _        => ResourceType.Image
                   };
        return View(new SearchBarViewModel()
                    {
                        Action = action ?? "Index",
                        Controller = controller ?? "Home",
                        Query = query ?? string.Empty,
                        ResourceType = type
                    });
    }
}