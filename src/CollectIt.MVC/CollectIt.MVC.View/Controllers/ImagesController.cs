using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Account;
using CollectIt.Database.Entities.Resources;
using CollectIt.Database.Infrastructure;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.Database.Infrastructure.Resources.Repositories;
using CollectIt.MVC.View.Models;
using CollectIt.MVC.View.Views.Shared.Components.ImageCards;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Controllers;

[Route("images")]
public class ImagesController : Controller
{
    private readonly IImageManager _imageManager;
    private IWebHostEnvironment appEnvironment;
    private readonly UserManager _userManager;

    public ImagesController(IImageManager imageManager, IWebHostEnvironment appEnvironment, UserManager userManager)
    {
        _imageManager = imageManager;
        this.appEnvironment = appEnvironment;
        _userManager = userManager;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetImagesByName([FromQuery(Name = "q")] [Required] string query)
    {
        var images = new List<Image>();
        await foreach (var image in _imageManager.GetAllByQuery(query))
        {
            images.Add(image);
        }

        return View("Images", new ImageCardsViewModel() { Images = images });
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> Image(int id)
    {
        var source = await _imageManager.FindByIdAsync(id);
        if (source == null)
        {
            return View("Error");
        }

        var model = new ImageViewModel()
        {
            Owner = source.Owner,
            UploadDate = source.UploadDate,
            Path = source.Address,
            Tags = source.Tags
        };
        return View(model);
    }

    [Route("postView")]
    public async Task<IActionResult> GetPostView()
    {
        return View("ImagePostTest");
    }

    [HttpPost]
    [Route("post")]
    public async Task<IActionResult> PostImage(string tags, string name, IFormFile uploadedFile)
    {
        var address = appEnvironment.WebRootPath + "/imagesFromDb/";
        await _imageManager.Create(address, uploadedFile.FileName, name, tags, uploadedFile);
        return View("ImagePostTest");
    }

    [HttpPost("comment")]
    [Authorize(Roles = CookieAuthenticationDefaults.AuthenticationScheme)]
    public async Task<IActionResult> LeaveComment(int imageId, string content)
    {
        var user = await _userManager.GetUserAsync(User);
        // var comment = await _imageManager.AddCommentAsync(user.Id, imageId, content);
        // if (comment is not null)
        // {
        //     var image = await _imageManager.FindByIdAsync(imageId);
        //     var imageViewModel = new ImageViewModel() ......
        //     return View("Image", imageViewModel);
        // }
        return BadRequest("This feature is not implemented yet");
    }
}