using System.Diagnostics;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Entities.Resources;
using CollectIt.MVC.View.ViewModels;
using Microsoft.AspNetCore.Mvc;
using CollectIt.MVC.View.Views.Shared.Components.ImageCards;

namespace CollectIt.MVC.View.Controllers;

[Route("")]
[Controller]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [Route("")]
    [IgnoreAntiforgeryToken]
    public IActionResult Index(IndexViewModel model)
    {
        return model.ResourceType switch
               {
                   ResourceType.Image => RedirectToAction("GetImagesByQuery", "Images", new {q = model.Query}),
                   ResourceType.Music => RedirectToAction("GetQueriedMusics", "Musics", new {q = model.Query}),
                   _ => View(new IndexViewModel()
                             {
                                 Query = "Данный тип пока не поддерживается", ResourceType = ResourceType.Image
                             })
               };
    }
    
    [HttpGet]
    [Route("privacy")]
    public IActionResult Privacy()
    {
        return View();
    }
    
    [HttpGet]
    [Route("error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}