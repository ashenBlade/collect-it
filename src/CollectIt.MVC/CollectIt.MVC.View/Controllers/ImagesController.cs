using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Abstractions.Account.Interfaces;
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
    private readonly IResourceAcquisitionService _resourceAcquisitionService;

    public ImagesController(IImageManager imageManager,
                            UserManager userManager,
                            ICommentManager commentManager,
                            ILogger<ImagesController> logger,
                            IResourceAcquisitionService resourceAcquisitionService)
    {
        _imageManager = imageManager;
        _commentManager = commentManager;
        _logger = logger;
        _resourceAcquisitionService = resourceAcquisitionService;
        _userManager = userManager;
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
                                                               DownloadAddress =
                                                                   Url.Action("DownloadImage", new {id = i.Id})!,
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
            return View("Error", new ErrorViewModel() {Message = "Изображения не существует"});
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
    public async Task<IActionResult> UploadImage([FromForm] [Required] UploadImageViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var userId = int.Parse(_userManager.GetUserId(User));
        if (!TryGetExtension(viewModel.Content.FileName, out var extension))
        {
            return View("Error",
                        new ErrorViewModel()
                        {
                            Message =
                                $"Поддерживаемые расширения изображений: {SupportedImageExtensions.Aggregate((s, n) => $"{s}, {n}")}"
                        });
        }

        try
        {
            await using var stream = viewModel.Content.OpenReadStream();
            var image = await _imageManager.CreateAsync(viewModel.Name,
                                                        userId,
                                                        viewModel.Tags
                                                                 .Split(' ', StringSplitOptions.RemoveEmptyEntries),
                                                        stream,
                                                        extension!);

            _logger.LogInformation("Image (ImageId = {ImageId}) was created by user (UserId = {UserId})", userId,
                                   image.Id);
            var acquired = await _resourceAcquisitionService.AcquireImageWithoutSubscriptionAsync(userId, image.Id);
            return RedirectToAction("Profile", "Account");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving image");
            ModelState.AddModelError("", "Error while saving image on server side");
            return View("Error", new ErrorViewModel() {Message = "Error while saving image on server"});
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
        try
        {
            var image = await _imageManager.FindByIdAsync(id);
            if (image is null)
            {
                return View("Error", new ErrorViewModel() {Message = "Image not found"});
            }

            var stream = await _imageManager.GetContentAsync(id);
            return File(stream, $"image/{image.Extension}", $"{image.Name}.{image.Extension}");
        }
        catch (IOException)
        {
            return View("Error", new ErrorViewModel() {Message = "Файл с изображением не найден"});
        }
    }


    [HttpGet("{id:int}/download")]
    [Authorize]
    public async Task<IActionResult> DownloadImage(int id)
    {
        try
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

            var content = await _imageManager.GetContentAsync(id);
            return File(content, $"image/{image.Extension}", $"{image.Name}.{image.Extension}");
        }
        catch (IOException)
        {
            return View("Error", new ErrorViewModel() {Message = "Файл с изображением не найден"});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while downloading image");
            return View("Error", new ErrorViewModel() {Message = "Ошибка при загрузке изображения"});
        }
    }


    [HttpPost("comment")]
    [Authorize]
    public async Task<IActionResult> LeaveComment([FromForm] LeaveCommentVewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var comment = await _commentManager.CreateComment(model.ImageId, user.Id, model.Content);
                return RedirectToAction("Image", new {imageId = model.ImageId});
            }

            return RedirectToAction("Image", new {imageId = model.ImageId});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while LeavingComment");
            return View("Error", new ErrorViewModel() {Message = "Ошибка при добавлении комментария"});
        }
    }
}