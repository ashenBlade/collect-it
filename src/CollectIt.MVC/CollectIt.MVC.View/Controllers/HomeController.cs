using System.Diagnostics;
using CollectIt.Database.Entities.Account;
using CollectIt.MVC.View.ViewModels;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("")]
    [IgnoreAntiforgeryToken]
    public IActionResult Index(IndexViewModel model)
    {
        return model.ResourceType switch
               {
                   ResourceType.Image => RedirectToAction("GetImagesByQuery", "Images", new {q = model.Query, p = 1}),
                   ResourceType.Music => RedirectToAction("GetQueriedMusics", "Musics", new {q = model.Query, p = 1}),
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