using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Views.Shared.Components.SearchBar;

[ViewComponent(Name = "SearchBar")]
public class SearchBarViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}