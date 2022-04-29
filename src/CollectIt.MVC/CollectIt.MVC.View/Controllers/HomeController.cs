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
        if (model.ResourceType == ResourceType.Image)
        {
            return RedirectToAction("GetImagesByQuery", "Images", new {q = model.Query});
        }

        model.Query = "Данный тип пока не поддерживается";
        _logger.LogTrace("Queried unsupported resource type: {ResourceType}", model.ResourceType);
        return View(model);
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