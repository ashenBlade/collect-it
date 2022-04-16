using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Resources;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.MVC.View.Models;
using CollectIt.MVC.View.Views.Shared.Components.ImageCards;
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

    public ImagesController(IImageManager imageManager, IWebHostEnvironment appEnvironment, UserManager userManager,
        ICommentManager commentManager)
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

        var comments = await _commentManager.GetResourcesComments(source.Id);

        // var commentViewModels = new List<CommentViewModel>();
        var model = new ImageViewModel()
        {
            ImageId = id,
            Comments = comments.Select(c => new CommentViewModel()
                { Author = c.Owner.UserName, PostTime = c.UploadDate, Comment = c.Content }),
            Owner = source.Owner,
            UploadDate = source.UploadDate,
            Path = source.Address,
            Tags = source.Tags,
            IsAcquired = await _imageManager.IsAcquiredBy(source.OwnerId, id)
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
        var user = await _userManager.GetUserAsync(User);
        var address = appEnvironment.WebRootPath + "/imagesFromDb/";
        await using var stream = uploadedFile.OpenReadStream();
        await _imageManager.Create(user.Id, address, uploadedFile.FileName, name, tags, stream);
        return View("ImagePostPage");
    }

    [HttpPost("comment")]
    [Authorize]
    public async Task<IActionResult> LeaveComment([FromForm] LeaveCommentVewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        var imageId = model.ImageId;
        var comment = await _commentManager.CreateComment(imageId, user.Id, model.Content);
        if (comment is null)
            return BadRequest($"This feature is not implemented yet\nImage Id: {imageId}\nComment: {model.Content}");
        // var image = await _imageManager.FindByIdAsync(imageId);
        // var comments = await _commentManager.GetResourcesComments(imageId);
        // var imageViewModel = new ImageViewModel()
        //                      {
        //                          ImageId = imageId,
        //                          UploadDate = image.UploadDate,
        //                          Path = image.Address,
        //                          Tags = image.Tags,
        //                          Owner = image.Owner,
        //                          Comments = comments.Select(c => new CommentViewModel()
        //                                                          {
        //                                                              Author = c.Owner.UserName,
        //                                                              Comment = c.Content,
        //                                                              PostTime = c.UploadDate
        //                                                          })
        //                      };
        return RedirectToAction("Image", new { id = model.ImageId });
    }
}