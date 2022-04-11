using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Resources;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.MVC.View.Models;
using CollectIt.MVC.View.Views.Shared.Components.ImageCards;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Controllers;

[Route("images")]
public class ImagesController : Controller
{
    private readonly IImageManager _imageManager;
    private readonly ICommentManager _commentManager;
    private IWebHostEnvironment appEnvironment;
    private readonly UserManager _userManager;

    public ImagesController(IImageManager imageManager, IWebHostEnvironment appEnvironment, UserManager userManager, ICommentManager commentManager)
    {
        _imageManager = imageManager;
        _commentManager = commentManager;
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
        //var comments = await _commentManager.GetResourcesComments(source.Id);

       // var commentViewModels = new List<CommentViewModel>();

        var model = new ImageViewModel()
        {
            ImageId = id,
            Owner = source.Owner,
            UploadDate = source.UploadDate,
            Path = source.Address,
            Tags = source.Tags
        };
        return View(model);
    }

    [Route("postView")]
    public IActionResult GetPostView()
    {
        return View("ImagePostPage");
    }

    [HttpPost]
    [Route("post")]
    public async Task<IActionResult> PostImage(string tags, string name, IFormFile uploadedFile)
    {
        var address = appEnvironment.WebRootPath + "/imagesFromDb/";
        await _imageManager.Create(address, uploadedFile.FileName, name, tags, uploadedFile);
        return View("ImagePostPage");
    }

    [HttpPost("comment")]
    [Authorize]
    public async Task<IActionResult> LeaveComment([FromForm]LeaveCommentVewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        // var comment = await _imageManager.AddCommentAsync(user.Id, imageId, content);
        // if (comment is not null)
        // {
        //     var image = await _imageManager.FindByIdAsync(imageId);
        //     var imageViewModel = new ImageViewModel() ......
        //     return View("Image", imageViewModel);
        // }
        return BadRequest($"This feature is not implemented yet\nImage Id: {model.ImageId}\nComment: {model.Content}");
    }
}