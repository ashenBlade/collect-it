using System.Diagnostics;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Entities.Resources;
using Microsoft.AspNetCore.Mvc;
using CollectIt.MVC.View.Models;
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
            return RedirectToAction("GetImagesByName", "Images", new {q = model.Query});
        }

        model.Query = "Данный тип пока не поддерживается";
        return View(model);
    }
    

    // [HttpGet]
    // [Route("images")]
    // public IActionResult Images(ImageCardsViewModel model)
    // {
    //     return View("ResourcesPage", model);
    // }
    
    // [HttpGet]
    // [Route("images/{imageId:int}")]
    // public IActionResult Image(int imageId)
    // {
    //     return View("Image", new Image()
    //                          {
    //                              ImageId = imageId
    //                          });
    // }

    [HttpGet]
    [Route("privacy")]
    public IActionResult Privacy()
    {
        return View();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }

    // [HttpGet]
    // [Route("images")]
    // public IActionResult FindImagesByName([FromQuery(Name = "q")]string pattern)
    // {
    //     var viewModel = GetImageCardsViewModel();
    //     return View("ResourcesPage", viewModel);
    // }
}