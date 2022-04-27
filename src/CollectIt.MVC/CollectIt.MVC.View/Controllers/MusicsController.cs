using System.ComponentModel.DataAnnotations;
using CollectIt.Database.Abstractions.Resources;
using CollectIt.Database.Infrastructure.Account.Data;
using CollectIt.Database.Infrastructure.Resources.FileManagers;
using CollectIt.MVC.View.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CollectIt.MVC.View.Controllers;

[Route("musics")]
public class MusicsController : Controller
{
    private readonly IMusicManager _musicManager;
    private readonly UserManager _userManager;

    public MusicsController(IMusicManager musicManager, UserManager userManager)
    {
        _musicManager = musicManager;
        _userManager = userManager;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetQueriedMusics([FromQuery(Name = "q")] string query, 
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
    public async Task<IActionResult> GetMusicsPageAsync(int id)
    {
        var image = await _musicManager.FindByIdAsync(id);
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
    public async Task<IActionResult> UploadMusic()
    {
        throw new NotImplementedException();
    }

    [HttpPost("upload")]
    [Authorize]
    public async Task<IActionResult> UploadMusic(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)]
        [Required] 
        UploadMusicViewModel model)
    {
        var userId = int.Parse(_userManager.GetUserId(User));
        if (!TryGetExtension(model.Content.FileName, out var extension))
        {
            // ModelState.AddModelError("FormFile", $"Поддерживаемые расширения музыки: {SupportedVideoFormats.Aggregate((s, n) => $"{s}, {n}")}");
            return View("Error",
                        new ErrorViewModel()
                        {
                            Message =
                                $"Поддерживаемые расширения видео: {SupportedMusicExtensions.Aggregate((s, n) => $"{s}, {n}")}"
                        });
        }
        try
        {
            await using var stream = model.Content.OpenReadStream();
            var music = await _musicManager.CreateAsync(model.Name, userId, model.Tags.Split(' ', StringSplitOptions.RemoveEmptyEntries), stream, extension,
                                                        model.Duration);

            return CreatedAtAction("GetMusicsPage", new {id = music.Id}, new { });
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }

    private bool TryGetExtension(string filename, out string extension)
    {
        extension = null!;
        if (filename is null)
        {
            throw new ArgumentNullException(nameof(filename));
        }

        var array = filename.Split('.');
        if (array.Length < 2)
        {
            return false;
        }

        return SupportedMusicExtensions.Contains(extension = array[^1].ToLower());
    }

    private static HashSet<string> SupportedMusicExtensions = new HashSet<string>() {"mp3", "ogg", "wav"};

    [HttpGet("download/{id:int}")]
    [Authorize]
    public async Task<IActionResult> DownloadMusicContent(int id)
    {
        var userId = int.Parse(_userManager.GetUserId(User));
        if (await _musicManager.IsAcquiredBy(id, userId))
        {
            var stream = await _musicManager.GetContentAsync(id);
            return File(stream, "music/*");
        }

        return BadRequest();
    }
}