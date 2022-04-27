using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Infrastructure;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.MVC.View.DTO;
using CollectIt.MVC.View.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CollectIt.MVC.View.Controllers;

[Route("videos")]
public class VideosController : Controller
{
    private readonly IVideoManager _videoManager;
    private readonly UserManager _userManager;
    private readonly ILogger<VideosController> _logger;

    public VideosController(IVideoManager videoManager, 
                            UserManager userManager,
                            ILogger<VideosController> logger)
    {
        _videoManager = videoManager;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetQueriedVideos([FromQuery(Name = "q")] string query, 
                                                      [Range(1, int.MaxValue)]
                                                      [FromQuery(Name = "page_number")]
                                                      int pageNumber,
                                                      [Range(1, int.MaxValue)]
                                                      [FromQuery(Name = "page_size")]
                                                      int pageSize)
    {
        return View("Error", new ErrorViewModel()
                             {
                                 Message = "Videos page is not implemented yet"
                             });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetVideoPageAsync(int id)
    {
        var image = await _videoManager.FindByIdAsync(id);
        if (image is null)
        {
            return NotFound();
        }

        return View("Error", new ErrorViewModel()
                             {
                                 Message = "Videos page is not implemented yet"
                             });
    }

    [HttpGet("upload")]
    [Authorize]
    public async Task<IActionResult> UploadView()
    {
        throw new NotImplementedException();
    }

    [HttpPost("upload")]
    [Authorize]
    public async Task<IActionResult> UploadVideo(
        [FromForm]
        [Required] 
        UploadVideoViewModel viewModel)
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
                                                        viewModel.Tags.Split(' ', StringSplitOptions.RemoveEmptyEntries), 
                                                        stream, 
                                                        extension!,
                                                        viewModel.Duration);

            _logger.LogInformation("Video (VideoId = {VideoId}) was created by user (UserId = {UserId})", userId, video.Id);
            return RedirectToAction("Profile", "Account");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving video");
            ModelState.AddModelError("", "Error while saving video on server side");
            return View("Error", new ErrorViewModel()
                                 {
                                     Message = "Error while saving video on server"
                                 });
        }
    }

    private static readonly HashSet<string> SupportedVideoFormats = new(){"mpeg", "mpg", "avi", "mkv", "webm"};

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