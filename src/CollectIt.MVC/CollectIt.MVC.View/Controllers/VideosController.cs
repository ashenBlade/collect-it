using System.ComponentModel.DataAnnotations;
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

    private readonly ILogger<VideosController> _logger;
    private readonly UserManager _userManager;
    private readonly ICommentManager _commentManager;
    private readonly IVideoManager _videoManager;
    private readonly int DefaultPageSize = 15;

    public VideosController(IVideoManager videoManager,
                            UserManager userManager,
                            ILogger<VideosController> logger, 
                            ICommentManager commentManager)
    {
        _videoManager = videoManager;
        _userManager = userManager;
        _logger = logger;
        _commentManager = commentManager;
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
                        Videos = videos.Result.Select(i => new VideoViewModel()
                                                    {
                                                        Address = Url.Action("DownloadVideoContent", new {id = i.Id})!,
                                                        Name = i.Name,
                                                        VideoId = i.Id,
                                                        Comments = Array.Empty<CommentViewModel>(),
                                                        Tags = i.Tags,
                                                        OwnerName = i.Owner.UserName,
                                                        UploadDate = i.UploadDate,
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
            return View("Error");
        }
        
        var user = await _userManager.GetUserAsync(User);
        var model = new VideoViewModel()
        {
            VideoId = id,
            Name = source.Name,
            OwnerName = source.Owner.UserName,
            UploadDate = source.UploadDate,
            Address = Url.Action("DownloadVideoContent", new {id = id})!,
            Tags = source.Tags,
            IsAcquired = user is not null && await _videoManager.IsAcquiredBy(id, user.Id),
            Comments = ( await _commentManager.GetResourcesComments(id) ).Select(c => new CommentViewModel() 
            {
                Author = c.Owner.UserName,
                Comment = c.Content,
                PostTime = c.UploadDate
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
            return RedirectToAction("Profile", "Account");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving video");
            ModelState.AddModelError("", "Error while saving video on server side");
            return View("Error", new ErrorViewModel() {Message = "Error while saving video on server"});
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

    [HttpGet("download/{id:int}")]
    [Authorize]
    public async Task<IActionResult> DownloadVideoContent(int id)
    {
        var userId = int.Parse(_userManager.GetUserId(User));
        if (await _videoManager.IsAcquiredBy(id, userId))
        {
            var stream = await _videoManager.GetContentAsync(id);
            return File(stream, "video/*");
        }

        return BadRequest();
    }
}