using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Entities.Resources;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.MVC.View.ViewModels;
using CollectIt.MVC.View.Views.Shared.Components.ImageCards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CollectIt.MVC.View.Controllers;

[Route("images")]
public class ImagesController : Controller
{
    private readonly IImageManager _imageManager;
    private readonly ICommentManager _commentManager;
    private readonly ILogger<ImagesController> _logger;
    private readonly UserManager _userManager;

    private readonly string address;

    private static readonly int MaxPageSize = 5;

    public ImagesController(IImageManager imageManager,
                            UserManager userManager,
                            ICommentManager commentManager,
                            ILogger<ImagesController> logger)
    {
        _imageManager = imageManager;
        _commentManager = commentManager;
        _logger = logger;
        _userManager = userManager;
        address = Path.Combine(Directory.GetCurrentDirectory(), "content", "images");
    }

    [HttpGet("")]
    public async Task<IActionResult> GetImagesByName([FromQuery(Name = "q")] [Required] string query,
                                                     [FromQuery(Name = "p")] [Range(1, int.MaxValue)]
                                                     int pageNumber = 1)
    {
        var images = new List<Image>();
        await foreach (var image in _imageManager.GetAllByQuery(query, pageNumber, MaxPageSize))
        {
            images.Add(image);
        }

        return View("Images",
                    new ImageCardsViewModel()
                    {
                        Images = images.Select(i => new ImageViewModel()
                                                    {
                                                        Address = $"/imagesFromDb/{i.FileName}",
                                                        Name = i.Name,
                                                        ImageId = i.Id,
                                                        Comments = Array.Empty<CommentViewModel>(),
                                                        Tags = i.Tags,
                                                        OwnerName = i.Owner.UserName,
                                                        UploadDate = i.UploadDate,
                                                        IsAcquired = false
                                                    })
                                       .ToList(),
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
                                                        {
                                                            Author = c.Owner.UserName,
                                                            PostTime = c.UploadDate,
                                                            Comment = c.Content
                                                        }),
                        Name = source.Name,
                        OwnerName = source.Owner.UserName,
                        UploadDate = source.UploadDate,
                        Address = $"/imagesFromDb/{source.FileName}",
                        Tags = source.Tags,
                        IsAcquired = await _imageManager.IsAcquiredBy(source.OwnerId, id)
                    };
        return View(model);
    }

    [HttpGet("upload")]
    [Authorize]
    public IActionResult UploadImage()
    {
        return View();
    }

    [HttpPost("upload")]
    [Authorize]
    public async Task<IActionResult> UploadImage([Required] 
                                                 [FromForm]
                                                 UploadImageViewModel model)
    {
        if (!TryGetExtension(model.Content.FileName, out var extension))
        {
            ModelState.AddModelError("Content", $"Поддерживаемые расширения изображений: {SupportedImageExtensions.Aggregate((s, n) => $"{s}, {n}")}");
            return View(model);
        }

        var (name, tags) = ( model.Name, model.Tags.Split(' ', StringSplitOptions.RemoveEmptyEntries) );
        var user = await _userManager.GetUserAsync(User);
        await using var stream = model.Content.OpenReadStream();
        try
        {
            await _imageManager.Create(user.Id, name, tags, stream, extension, address);
            return RedirectToAction("Profile", "Account");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving image");
            ModelState.AddModelError("", "Error while uploading image");
            return View(model);
        }
    }

    private static readonly HashSet<string> SupportedImageExtensions = new HashSet<string>() {"png", "jpeg", "jpg", "webp", "bmp"};

    
    private static bool TryGetExtension(string filename, out string? extension)
    {
        if (filename is null)
        {
            throw new ArgumentNullException(nameof(filename));
        }
        
        extension = null;
        var array = filename.Split('.');
        if (array.Length < 2)
        {
            return false;
        }

        extension = array[^1].ToLower();
        return SupportedImageExtensions.Contains(extension);
    }

    [Route("download/{id:int}")]
    public async Task<IActionResult> DownloadImage(int id)
    {
        var source = await _imageManager.FindByIdAsync(id);
        if (source == null)
        {
            return View("Error");
        }

        var file = new FileInfo(Path.Combine(address, source.FileName));

        return file.Exists
                   ? PhysicalFile(file.FullName, $"image/jpg")
                   : BadRequest(new {Message = "File not found"});
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