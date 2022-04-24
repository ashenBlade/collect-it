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

    public VideosController(IVideoManager videoManager, UserManager userManager)
    {
        _videoManager = videoManager;
        _userManager = userManager;
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

    [HttpPost("")]
    [Authorize]
    public async Task<IActionResult> UploadNewVideo(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)]
        [Required] 
        CreateVideoDTO dto)
    {
        var userId = int.Parse(_userManager.GetUserId(User));
        try
        {
            await using var stream = dto.FormFile.OpenReadStream();
            var video = await _videoManager.CreateAsync(dto.Name, userId, dto.Tags, stream, dto.Extension,
                                                        dto.Duration);

            return CreatedAtAction("GetVideoPage", new {id = video.Id}, new { });
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
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