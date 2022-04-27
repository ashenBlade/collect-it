using System.Diagnostics;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Entities.Resources;
using CollectIt.MVC.View.ViewModels;
using Microsoft.AspNetCore.Mvc;
using CollectIt.MVC.View.Views.Shared.Components.ImageCards;

namespace CollectIt.MVC.View.Controllers;

[Route("upload")]
public class UploadController : Controller
{
    [Route("imageUpload")]
    public IActionResult ImageUpload()
    {
        return View();
    }
    
    [Route("videoUpload")]
    public IActionResult VideoUpload()
    {
        return View();
    }
    
    [Route("musicUpload")]
    public IActionResult MusicUpload()
    {
        return View();
    }
    
    [HttpPost]
    [Route("1")]
    public void UploadImage(UploadImageViewModel model)
    {
        
    }
    
    [HttpPost]
    [Route("2")]
    public void UploadVideo(UploadVideoViewModel model)
    {
        
    }
    
    [HttpPost]
    [Route("3")]
    public void UploadMusic(UploadMusicViewModel model)
    {
        
    }
}