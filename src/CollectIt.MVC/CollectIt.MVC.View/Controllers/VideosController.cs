using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Abstractions.Account.Interfaces;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.MVC.View.ViewModels;
using CollectIt.MVC.View.Views.Shared.Components.VideoCards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollectIt.MVC.View.Controllers;

[Route("videos")]
public class VideosController : Controller
{
    private static readonly HashSet<string> SupportedVideoFormats = new()
    {
        "mpeg",
        "mpg",
        "avi",
        "mkv",
        "webm"
    };

    private readonly ICommentManager _commentManager;
    private readonly IResourceAcquisitionService _resourceAcquisitionService;

    private readonly ILogger<VideosController> _logger;
    private readonly UserManager _userManager;
    private readonly IVideoManager _videoManager;
    private readonly int DefaultPageSize = 15;

    public VideosController(IVideoManager videoManager,
        UserManager userManager,
        ILogger<VideosController> logger,
        ICommentManager commentManager, IResourceAcquisitionService resourceAcquisitionService)
    {
        _videoManager = videoManager;
        _userManager = userManager;
        _logger = logger;
        _commentManager = commentManager;
        _resourceAcquisitionService = resourceAcquisitionService;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetQueriedVideos([FromQuery(Name = "q")] [Required] string? query,
        [Range(1, int.MaxValue)] [FromQuery(Name = "p")]
        int pageNumber = 1)
    {
        var videos = query is null
            ? await _videoManager.GetAllPagedAsync(pageNumber, DefaultPageSize)
            : await _videoManager.QueryAsync(query, pageNumber, DefaultPageSize);
        return View("Videos",
            new VideoCardsViewModel()
            {
                Videos = videos.Result.Select(v => new VideoViewModel()
                    {
                        DownloadAddress =
                            Url.Action("DownloadVideoContent", new {id = v.Id})!,
                        PreviewAddress =
                            Url.Action("DownloadVideoPreview", new {id = v.Id})!,
                        Name = v.Name,
                        VideoId = v.Id,
                        Comments = Array.Empty<CommentViewModel>(),
                        Tags = v.Tags,
                        OwnerName = v.Owner.UserName,
                        UploadDate = v.UploadDate,
                        IsAcquired = false
                    })
                    .ToList(),
                PageNumber = pageNumber,
                MaxVideosCount = videos.TotalCount,
                Query = query
            });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Video(int id)
    {
        var source = await _videoManager.FindByIdAsync(id);
        if (source == null)
        {
            return View("Error", new ErrorViewModel() {Message = "Видео не существует"});
        }

        var user = await _userManager.GetUserAsync(User);
        var model = new VideoViewModel()
        {
            VideoId = id,
            Name = source.Name,
            OwnerName = source.Owner.UserName,
            UploadDate = source.UploadDate,
            DownloadAddress = Url.Action("DownloadVideoContent", new {id = id})!,
            PreviewAddress = Url.Action("DownloadVideoPreview", new {id = id})!,
            Tags = source.Tags,
            IsAcquired = user is not null && await _videoManager.IsAcquiredBy(id, user.Id),
            Comments = (await _commentManager.GetResourcesComments(id)).Select(c => new CommentViewModel()
            {
                Author = c.Owner
                    .UserName,
                Comment =
                    c.Content,
                PostTime = c
                    .UploadDate
            })
        };

        return View(model);
    }

    [HttpGet("upload")]
    [Authorize]
    public async Task<IActionResult> UploadVideo()
    {
        return View();
    }

    [HttpPost("upload")]
    [Authorize]
    public async Task<IActionResult> UploadVideo(
        [FromForm] [Required] UploadVideoViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);
        var userId = int.Parse(_userManager.GetUserId(User));
        if (!TryGetExtension(viewModel.Content.FileName, out var extension))
        {
            // ModelState.AddModelError("FormFile", $"Поддерживаемые расширения видео: {SupportedVideoFormats.Aggregate((s, n) => $"{s}, {n}")}");
            return View("Error",
                new ErrorViewModel()
                {
                    Message =
                        $"Поддерживаемые расширения видео: {SupportedVideoFormats.Aggregate((s, n) => $"{s}, {n}")}"
                });
        }
        try
        { 
            await using var stream = viewModel.Content.OpenReadStream();
            var video = await _videoManager.CreateAsync(viewModel.Name, userId,
                viewModel.Tags
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries),
                stream,
                extension!,
                viewModel.Duration);

            _logger.LogInformation("Video (VideoId = {VideoId}) was created by user (UserId = {UserId})", userId,
                video.Id);
            
            var acquired = await _resourceAcquisitionService.AcquireVideoWithoutSubscriptionAsync(userId, video.Id);
            
            return RedirectToAction("Profile", "Account");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving video");
            ModelState.AddModelError("", "Error while saving video on server side");
            return View("Error", new ErrorViewModel() {Message = "Ошибка при загрузке видео"});
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
        return SupportedVideoFormats.Contains(extension);
    }

    [HttpGet("{id:int}/preview")]
    public async Task<IActionResult> DownloadVideoPreview(int id)
    {
        var video = await _videoManager.FindByIdAsync(id);
        if (video is null)
        {
            return NotFound();
        }

        var content = await _videoManager.GetContentAsync(id);
        return File(content, $"video/{video.Extension}", $"{video.Name}.{video.Extension}");
    }

    [HttpGet("download/{id:int}")]
    [Authorize]
    public async Task<IActionResult> DownloadVideoContent(int id)
    {
        try
        {
            var userId = int.Parse(_userManager.GetUserId(User));
            if (!await _videoManager.IsAcquiredBy(id, userId))
                return BadRequest();
            var stream = await _videoManager.GetContentAsync(id);
            var video = await _videoManager.FindByIdAsync(id);
            return File(stream, $"video/{video.Extension}", $"{video.Name}.{video.Extension}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while downloading video");
            return View("Error", new ErrorViewModel() {Message = "Ошибка при загрузке видео"});
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
                return RedirectToAction("Video", new {id = model.ImageId});
            }

            return RedirectToAction("Video", new {id = model.ImageId});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while leaving comment");
            return View("Error", new ErrorViewModel() {Message = "Ошибка при добавлении комментария"});
        }
    }
}