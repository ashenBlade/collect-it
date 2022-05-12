using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.MVC.View.ViewModels;
using CollectIt.MVC.View.Views.Shared.Components.ImageCards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Controllers;

[Route("images")]
public class ImagesController : Controller
{
    private static readonly int MaxPageSize = 5;

    private static readonly HashSet<string> SupportedImageExtensions = new()
    {
        "png",
        "jpeg",
        "jpg",
        "webp",
        "bmp"
    };

    private readonly ICommentManager _commentManager;
    private readonly IImageManager _imageManager;
    private readonly ILogger<ImagesController> _logger;
    private readonly UserManager _userManager;

    private readonly string address;

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
    public async Task<IActionResult> GetImagesByQuery([FromQuery(Name = "q")] [Required] string? query,
                                                      [FromQuery(Name = "p")] [Range(1, int.MaxValue)]
                                                      int pageNumber = 1)
    {
        var images = query is null
            ? await _imageManager.GetAllPagedAsync(MaxPageSize, pageNumber)
            : await _imageManager.QueryAsync(query, MaxPageSize, pageNumber);

        return View("Images",
                    new ImageCardsViewModel()
                    {
                        Images = images.Result.Select(i => new ImageViewModel()
                                                    {
                                                        DownloadAddress = Url.Action("DownloadImage", new {id = i.Id})!,
                                                        PreviewAddress =
                                                            Url.Action("DownloadImagePreview", new {id = i.Id})!,
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
                        MaxPagesCount = ( int ) Math.Ceiling(( double ) images.TotalCount / MaxPageSize),
                        Query = query ?? string.Empty
                    });
    }

    [HttpGet("{imageId:int}")]
    public async Task<IActionResult> Image(int imageId)
    {
        var source = await _imageManager.FindByIdAsync(imageId);
        if (source == null)
        {
            return View("Error");
        }

        var user = await _userManager.GetUserAsync(User);
        var comments = await _commentManager.GetResourcesComments(source.Id);

        var model = new ImageViewModel()
                    {
                        ImageId = imageId,
                        Comments =
                            comments.Select(c => new CommentViewModel()
                                                 {
                                                     Author = c.Owner.UserName,
                                                     PostTime = c.UploadDate,
                                                     Comment = c.Content
                                                 }),
                        Name = source.Name,
                        OwnerName = source.Owner.UserName,
                        UploadDate = source.UploadDate,
                        DownloadAddress = Url.Action("DownloadImage", new {id = imageId})!,
                        PreviewAddress = Url.Action("DownloadImagePreview", new {id = imageId})!,
                        Tags = source.Tags,
                        IsAcquired = user is not null && await _imageManager.IsAcquiredBy(user.Id, imageId)
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
    public async Task<IActionResult> UploadImage([Required] [FromForm] UploadImageViewModel model)
    {
        if (!TryGetExtension(model.Content.FileName, out var extension))
        {
            ModelState.AddModelError("Content",
                                     $"Поддерживаемые расширения изображений: {SupportedImageExtensions.Aggregate((s, n) => $"{s}, {n}")}");
            return View(model);
        }

        var (name, tags) = ( model.Name, model.Tags );
        var user = await _userManager.GetUserAsync(User);
        await using var stream = model.Content.OpenReadStream();
        try
        {
            await _imageManager.Create(user.Id, address, name, tags, stream, extension!);
            return RedirectToAction("Profile", "Account");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving image");
            ModelState.AddModelError("", "Error while uploading image");
            return View(model);
        }
    }


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

    [HttpGet("{id:int}/preview")]
    public async Task<IActionResult> DownloadImagePreview(int id)
    {
        var image = await _imageManager.FindByIdAsync(id);
        if (image is null)
        {
            return View("Error", new ErrorViewModel() {Message = "Image not found"});
        }

        var file = new FileInfo(Path.Combine(address, image.FileName));
        return file.Exists
                   ? PhysicalFile(file.FullName, $"image/{image.Extension}", image.FileName)
                   : BadRequest(new {Message = "Image content not found"});
    }


    [HttpGet("{id:int}/download")]
    [Authorize]
    public async Task<IActionResult> DownloadImage(int id)
    {
        var userId = int.Parse(_userManager.GetUserId(User));
        if (!await _imageManager.IsAcquiredBy(userId, id))
        {
            return View("Error", new ErrorViewModel {Message = "Image is not acquired by user"});
        }

        var image = await _imageManager.FindByIdAsync(id);
        if (image is null)
        {
            return View("Error", new ErrorViewModel {Message = "Image not found"});
        }

        var file = new FileInfo(Path.Combine(address, image.FileName));
        return file.Exists
                   ? PhysicalFile(file.FullName, $"image/{image.Extension}", image.FileName)
                   : BadRequest(new {Message = "Image content not found"});
    }


    [HttpPost("comment")]
    [Authorize]
    public async Task<IActionResult> LeaveComment([FromForm] LeaveCommentVewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        var imageId = model.ImageId;
        var comment = await _commentManager.CreateComment(imageId, user.Id, model.Content);
        return RedirectToAction("Image", new {id = model.ImageId});
    }
}