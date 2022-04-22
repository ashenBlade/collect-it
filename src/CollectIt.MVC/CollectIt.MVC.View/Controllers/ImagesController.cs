using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Resources;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.MVC.View.ViewModels;
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

    private readonly string address;

    private static readonly int MaxPageSize = 5;

    public ImagesController(IImageManager imageManager, IWebHostEnvironment appEnvironment, UserManager userManager,
        ICommentManager commentManager)
    {
        _imageManager = imageManager;
        _commentManager = commentManager;
        this.appEnvironment = appEnvironment;
        _userManager = userManager;
        address = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagesFromDb");
    }

    [HttpGet("")]
    public async Task<IActionResult> GetImagesByName([FromQuery(Name = "q")]
                                                     [Required] 
                                                     string query, 
                                                     [FromQuery(Name = "p")]
                                                     [Range(1, int.MaxValue)]
                                                     int pageNumber = 1)
    {
        var images = new List<Image>();
        await foreach (var image in _imageManager.GetAllByQuery(query, pageNumber, MaxPageSize))
        {
            images.Add(image);
        }

        return View("Images", new ImageCardsViewModel() 
                              { 
                                  Images = images, 
                                  PageNumber = pageNumber,
                                  MaxImagesCount = MaxPageSize,
                                  Query = query
                              });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Image(int id)
    {
        var source = await _imageManager.FindByIdAsync(id);
        if (source == null)
        {
            return View("Error");
        }

        var comments = await _commentManager.GetResourcesComments(source.Id);

        var model = new ImageViewModel()
        {
            ImageId = id,
            Comments = comments.Select(c => new CommentViewModel()
                { Author = c.Owner.UserName, PostTime = c.UploadDate, Comment = c.Content }),
            OwnerName = source.Owner.UserName,
            UploadDate = source.UploadDate,
            Address = $"images/download/{id}",
            Tags = source.Tags,
            IsAcquired = await _imageManager.IsAcquiredBy(source.OwnerId, id)
        };
        return View(model);
    }

    [HttpGet("postView")]
    public IActionResult GetPostView()
    {
        return View("ImagePostPage");
    }

    [HttpPost("post")]
    public async Task<IActionResult> PostImage(string tags, string name, IFormFile uploadedFile)
    {
        var ext = GetExtension(uploadedFile.FileName);
        if (ext is null)
            return View("Error");
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return RedirectToAction("Login", "Account");
        await using var stream = uploadedFile.OpenReadStream();
        await _imageManager.Create(user.Id, address, name, tags, stream, ext);
        return View("ImagePostPage");
    }
    
    [NonAction]
    private string? GetExtension(string fileName)
    {
        var ext = fileName.Split(".").Last();
        if (ext != "jpg" && ext != "png")
            return null;
        return fileName.Split(".").Last() == "jpg" ? "jpeg" : "png";
    }

    [Route("download/{id:int}")]
    public async Task<IActionResult> DownloadImage(int id)
    {
        var source = await _imageManager.FindByIdAsync(id);
        if (source == null)
        {
            return View("Error");
        }

        var fi = new FileInfo(Path.Combine(address, source.FileName));

        await using var fileStream = fi.OpenRead();
        var contentType = $"image/{source.Extension}";
        return new FileStreamResult(fileStream, contentType)
        {
            FileDownloadName = fi.Name
        };
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
        return RedirectToAction("Image", new { id = model.ImageId });
    }
}